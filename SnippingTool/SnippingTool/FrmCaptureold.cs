using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PLLibrary;
using PLControls;
using System.Runtime.InteropServices.ComTypes;

namespace SnippingTool
{
    public partial class FrmCaptureold : Form
    {
        bool _bStartCapture = false;
        enCaptureType _enCaptureType;
        List<Point> _Pts = new List<Point>();
        Rectangle _rc;
        bool _isCapture = false;
        object[] _objOption;
        int _iNumberOfSides = 3;
        PointF[] vertices = null;
        Point dragPoint = new Point();
        GraphicsPath path;
        Matrix matrix;
        bool _bEdit = false;
        int _iSelectedRegion = 0;
        internal FrmCaptureold(enCaptureType captureType, object[] options)
        {
            InitializeComponent();
            _objOption = options;
            _enCaptureType = captureType;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None; 
            Rectangle rc = Rectangle.Empty;
            Screen[] all = Screen.AllScreens;
            path = new GraphicsPath();
            matrix = new Matrix();
            foreach (Screen screen in all)
            {
                rc = Rectangle.Union(screen.Bounds, rc);
            }

            this.Bounds = rc;
            BackColor = Color.Black;
            Opacity = .5;
            if (options.Length > 0)
            {
                if (options[0] is int side) _iNumberOfSides = side;
                if (_enCaptureType == enCaptureType.PolyGonES) vertices = new PointF[_iNumberOfSides];
                else if (_enCaptureType == enCaptureType.Star) vertices = new PointF[_iNumberOfSides*2];
            }
            
            ShowDialog();
        }
        private Region _rPolygon = null;
        private Region _rSelected = null;
        private List<Region> _regions = new List<Region>();
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                if (_enCaptureType == enCaptureType.Polygon)
                {
                    if (_bEdit) return;
                    _bStartCapture = true;
                    _Pts.Add(e.Location);
                    int count = _Pts.Count;
                    if (count > 2)
                    {
                        if (_rPolygon != null)
                        {
                            _rPolygon.Dispose();
                            foreach (Region r in _regions) r.Dispose();
                            _regions.Clear();
                        }

                        CreatePolygonRegion();

                        for (int i=0; i<count; i++)
                        {
                            CreateEllipticRegion(i);
                        }
                    }
                    Refresh();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (_enCaptureType == enCaptureType.Polygon)
                {
                    Hide();
                    if (_isCapture)
                    {
                        int count = _Pts.Count;
                        if (count < 3) return;
                        int xMin = int.MaxValue;
                        int yMin = int.MaxValue;
                        int xMax = int.MinValue;
                        int yMax = int.MinValue;
                        foreach (Point point in _Pts)
                        {
                            if (xMin > point.X) xMin = point.X;
                            if (yMin > point.Y) yMin = point.Y;
                            if (xMax < point.X) xMax = point.X;
                            if (yMax < point.Y) yMax = point.Y;
                        }
                        int width = xMax - xMin;
                        int height = yMax - yMin;

                        Image rectImage = new Rectangle(xMin, yMin, width, height).CaptureImage(PixelFormat.Format32bppArgb);
                        Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                        Graphics g = Graphics.FromImage(image);

                        g.Clear(Color.Transparent);
                        GraphicsPath path = new GraphicsPath();

                        Point[] array = new Point[count];
                        int index = 0;
                        foreach (Point point in _Pts)
                        {
                            array[index].X = point.X - xMin;
                            array[index].Y = point.Y - yMin;
                            index++;
                        }
                        path.Reset();
                        path.AddPolygon(array);
                        Region region = new Region(path);
                        g.SetClip(region, CombineMode.Replace);
                        g.DrawImage(rectImage, 0, 0);
                        Tag = image;

                        g.Dispose();
                        region.Dispose();
                    }
                }
            }
        }
        private void CreatePolygonRegion()
        {
            if (_rPolygon != null) _rPolygon.Dispose();
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(_Pts.ToArray());
                _rPolygon = new Region(path);
            }
        }
        private void CreateEllipticRegion(int index)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(_Pts[index].X - 3, _Pts[index].Y-3, 6, 6);
                _regions.Add(new Region(path));           
            }
        }
        private void EditEllipticRegion(int index)
        {
            _regions[index].Dispose();
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(_Pts[index].X - 3, _Pts[index].Y - 3, 6, 6);
                _regions[index] = new Region(path);
            }
        }
        Point _ptMouseDown;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (_enCaptureType != enCaptureType.Polygon)
                {
                    _Pts.Add(e.Location);
                    _bStartCapture = true;
                }
                else
                {
                    if (Cursor == Cursors.SizeAll)
                    {
                        _ptMouseDown = e.Location;
                        _bEdit = true;
                    }
                    else if (_Pts.Contains(e.Location))
                    {
                        _iSelectedRegion = _Pts.IndexOf(e.Location);
                        _bEdit = true;
                    }
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_bStartCapture)
            {
                if (_enCaptureType.Contains(enCaptureType.Rectangular))
                {
                    if (_Pts.Count == 2) _Pts[1] = e.Location;
                    else _Pts.Add(e.Location);
                    Refresh();
                }
                else if (_enCaptureType == enCaptureType.FreeForm)
                {
                    _Pts.Add(e.Location);
                    Refresh();
                }             
                else if (_enCaptureType == enCaptureType.Polygon)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (Cursor == Cursors.SizeAll)
                        {
                            int dx = e.X - _ptMouseDown.X;
                            int dy = e.Y - _ptMouseDown.Y;
                            _ptMouseDown = e.Location;
                            _rPolygon.Translate(dx, dy);
                            _Pts.Offset(dx, dy);
                            foreach (Region r in _regions) r.Translate(dx, dy);
                            Refresh();
                        }
                        else if (Cursor == Cursors.Hand)
                        {                            
                            _Pts[_iSelectedRegion] = e.Location;
                            CreatePolygonRegion();
                            EditEllipticRegion(_iSelectedRegion);
                            Refresh();
                        }
                    }
                    else if (e.Button == MouseButtons.None)
                    {
                        if (_rPolygon == null) return;
                        if (Cursor == Cursors.Default)
                        {
                            int count = _regions.Count;
                            for(int i=0; i<count; i++)
                            {
                                if (_regions[i].IsVisible(e.Location))
                                {
                                    Cursor = Cursors.Hand;
                                    _iSelectedRegion = i;
                                    return;
                                }
                            }
                            if (_rPolygon.IsVisible(e.Location))
                            {
                                Cursor = Cursors.SizeAll;
                            }
                        }
                        else
                        {
                            if (_rPolygon.IsVisible(e.Location))
                            {
                                return;
                            }

                            foreach (Region r in _regions)
                            {
                                if (r.IsVisible(e.Location))
                                {
                                    return;
                                }
                            }
                            Cursor = Cursors.Default;
                        }
                    }
                }
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_enCaptureType == enCaptureType.Rectangular)
            {
                _isCapture = _Pts.GetRect(out _rc);
                if (_isCapture)
                {
                    e.Graphics.FillRectangle(Brushes.White, _rc);
                }
            }
            else if (_enCaptureType == enCaptureType.Ellipse)
            {
                if (_isCapture = _Pts.GetRect(out _rc))
                {
                    e.Graphics.FillEllipse(Brushes.White, _rc);
                }
            }
            else if (_enCaptureType == enCaptureType.Circle)
            {
                if (_Pts.Count > 1)
                {
                    _isCapture = false;
                    int w = _Pts[1].X - _Pts[0].X;
                    int h = _Pts[1].Y - _Pts[0].Y;
                    if (w == 0 && h == 0) return;
                    _isCapture = true;
                    int radius = (int)Math.Sqrt(w * w + h * h);
                    int diameter = radius * 2;
                    _rc = new Rectangle(_Pts[0].X - radius, _Pts[0].Y - radius, diameter, diameter);
                    e.Graphics.FillEllipse(Brushes.White, _rc);
                }
            }
            else if (_enCaptureType == enCaptureType.FreeForm)
            {
                if (_Pts.Count > 2)
                {
                    e.Graphics.DrawLines(Pens.Red, _Pts.ToArray());
                    _isCapture = true;
                }
            }
            else if (_enCaptureType == enCaptureType.Polygon)
            {
                if (_rPolygon != null)
                {
                    e.Graphics.FillRegion(Brushes.White, _rPolygon);
                }
                foreach (Point pt in _Pts)
                {
                    _isCapture = true;
                    e.Graphics.FillEllipse(Brushes.Gray, pt.X - 3, pt.Y - 3, 6, 6);
                    e.Graphics.FillEllipse(Brushes.Red, pt.X - 1, pt.Y - 1, 2, 2);
                }
            }
            else if (_enCaptureType == enCaptureType.PolyGonES)
            {
                if (_Pts.Count > 1)
                {
                    //double w = _Pts[1].X - _Pts[0].X;
                    //double h = _Pts[1].Y - _Pts[0].Y;
                    //if (w == 0 && h == 0) return;
                    //_isCapture = true;

                    //PointF center = new PointF(_Pts[0].X, _Pts[0].Y);
                    //double angle = -Math.PI / 2; //calculate at the top of shape
                    //double radius = Math.Sqrt(w * w + h * h);
                    //double angleInc = 2 * Math.PI / _iNumberOfSides; 
                    //for (int i = 0; i < _iNumberOfSides; i++)
                    //{
                    //    vertices[i].X = center.X + (float)(radius * Math.Cos(angle));
                    //    vertices[i].Y = center.Y + (float)(radius * Math.Sin(angle));
                    //    angle += angleInc;
                    //}
                    //path.Reset();
                    //path.AddPolygon(vertices);

                    //matrix.Reset();
                    //float angleRotate = (float)(Math.Atan2(h, w) * 180.0 / Math.PI) + 90;
                    //matrix.RotateAt(angleRotate, center);
                    //path.Transform(matrix);
                    //vertices = path.PathPoints;
                    //e.Graphics.FillPath(Brushes.Red, path);

                    double w = _Pts[1].X - _Pts[0].X;
                    double h = _Pts[1].Y - _Pts[0].Y;
                    if (w == 0 && h == 0) return;
                    _isCapture = true;

                    PointF center = new PointF(_Pts[0].X, _Pts[0].Y);
                    double angle = Math.Atan2(h, w);
                    double radius = Math.Sqrt(w * w + h * h);
                    double angleInc = 2 * Math.PI / _iNumberOfSides;
                    for (int i = 0; i < _iNumberOfSides; i++)
                    {
                        vertices[i].X = center.X + (float)(radius * Math.Cos(angle));
                        vertices[i].Y = center.Y + (float)(radius * Math.Sin(angle));
                        angle += angleInc;
                    }
                    
                    e.Graphics.FillPolygon(Brushes.Red, vertices);
                }
            }
            else if (_enCaptureType == enCaptureType.Star)
            {
                if (_Pts.Count > 1)
                {
                    double w = _Pts[1].X - _Pts[0].X;
                    double h = _Pts[1].Y - _Pts[0].Y;
                    if (w == 0 && h == 0) return;
                    _isCapture = true;


                    // sin(β/2) * sinβ / (sinβ cosβ) 
                    double beta = Math.PI / _iNumberOfSides;



                    PointF center = new PointF(_Pts[0].X, _Pts[0].Y);
                    double angle = Math.Atan2(h, w);
                    double bigRadius = Math.Sqrt(w * w + h * h);

                    double smallRadius = 0.3;
                    if (_iNumberOfSides == 3) smallRadius = 0.25 * bigRadius;
                    else if (_iNumberOfSides == 4) smallRadius = 0.25 * bigRadius;
                    else
                    {
                        double ratio = Math.Cos(beta) - Math.Sin(beta) * Math.Tan(beta);
                        smallRadius = ratio * bigRadius;
                    }
                    
                    
                    double angleInc = Math.PI / _iNumberOfSides;
                    for (int i = 0; i < _iNumberOfSides*2; i++)
                    {
                        double radius = bigRadius;
                        if (i % 2 == 1) radius = smallRadius;
                        vertices[i].X = center.X + (float)(radius * Math.Cos(angle));
                        vertices[i].Y = center.Y + (float)(radius * Math.Sin(angle));
                        angle += angleInc;
                    }

                    //path.Reset();
                    //path.AddPolygon(vertices);
                    //float angleRotate = (float)( * 180.0 / Math.PI) + 90;

                    //matrix.Reset();
                    //matrix.RotateAt(angleRotate, center);
                    //path.Transform(matrix);
                    //vertices = path.PathPoints;
                    e.Graphics.FillPolygon(Brushes.Red, vertices);
                }
            }
        }

        

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_enCaptureType == enCaptureType.Polygon) return;
            //{
            //    if (_bEdit)
            //    {
            //        dragPoint = e.Location;
            //        _Pts[_iSelectedRegion] = dragPoint;
            //        _bEdit = false;
            //    }
            //    return;
            //}
            if (e.Button == MouseButtons.Left)
            {
                _bStartCapture = false;
                Hide();

                if (_enCaptureType == enCaptureType.Rectangular)
                {
                    if (_isCapture)
                    {
                        Tag = _rc.CaptureImage();
                    }
                }

                else if (_enCaptureType == enCaptureType.Ellipse || _enCaptureType == enCaptureType.Circle)
                {
                    if (_isCapture)
                    {
                        Image rectImage = _rc.CaptureImage(PixelFormat.Format32bppArgb);

                        Bitmap image = new Bitmap(_rc.Width, _rc.Height, PixelFormat.Format32bppArgb);
                        Graphics g = Graphics.FromImage(image);

                        g.Clear(Color.Transparent);
                        path.Reset();
                        path.AddEllipse(0, 0, _rc.Width, _rc.Height);
                        Region region = new Region(path);
                        g.SetClip(region, CombineMode.Replace);
                        g.DrawImage(rectImage, 0, 0);
                        Tag = image;

                        g.Dispose();
                        region.Dispose();
                    }
                }

                else if (_enCaptureType == enCaptureType.FreeForm)
                {
                    if (_isCapture)
                    {
                        int count = _Pts.Count;
                        if (count < 3) return;
                        int xMin = int.MaxValue;
                        int yMin = int.MaxValue;
                        int xMax = int.MinValue;
                        int yMax = int.MinValue;
                        foreach (Point point in _Pts)
                        {
                            if (xMin > point.X) xMin = point.X;
                            if (yMin > point.Y) yMin = point.Y;
                            if (xMax < point.X) xMax = point.X;
                            if (yMax < point.Y) yMax = point.Y;
                        }

                        int width = xMax - xMin;
                        int height = yMax - yMin;

                        Image rectImage = new Rectangle(xMin, yMin, width, height).CaptureImage(PixelFormat.Format32bppArgb);
                        Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                        Graphics g = Graphics.FromImage(image);

                        g.Clear(Color.Transparent);
                        path.Reset();
                        Point[] array = new Point[count];
                        int index = 0;
                        foreach (Point point in _Pts)
                        {
                            array[index].X = point.X - xMin;
                            array[index].Y = point.Y - yMin;
                            index++;
                        }
                        path.AddClosedCurve(array);
                        Region region = new Region(path);
                        g.SetClip(region, CombineMode.Replace);
                        g.DrawImage(rectImage, 0, 0);
                        Tag = image;

                        g.Dispose();
                        region.Dispose();
                    }
                }
                else if (_enCaptureType == enCaptureType.Star || _enCaptureType == enCaptureType.PolyGonES)
                {
                    if (_isCapture)
                    {
                        int count = vertices.Count();
                        float xMin = float.MaxValue;
                        float yMin = float.MaxValue;
                        float xMax = float.MinValue;
                        float yMax = float.MinValue;
                        foreach (PointF point in vertices)
                        {
                            if (xMin > point.X) xMin = point.X;
                            if (yMin > point.Y) yMin = point.Y;
                            if (xMax < point.X) xMax = point.X;
                            if (yMax < point.Y) yMax = point.Y;
                        }
                        float width = xMax - xMin;
                        float height = yMax - yMin;

                        Image rectImage = new Rectangle((int)xMin, (int)yMin, (int)width, (int)height).CaptureImage(PixelFormat.Format32bppArgb);
                        Bitmap image = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
                        Graphics g = Graphics.FromImage(image);

                        g.Clear(Color.Transparent);
                        PointF[] array = new PointF[count];
                        int index = 0;
                        foreach (PointF point in vertices)
                        {
                            array[index].X = point.X - xMin;
                            array[index].Y = point.Y - yMin;
                            index++;
                        }
                        path.Reset();
                        path.AddPolygon(array);
                        Region region = new Region(path);
                        g.SetClip(region, CombineMode.Replace);
                        g.DrawImage(rectImage, 0, 0);
                        Tag = image;

                        g.Dispose();
                        region.Dispose();
                    }
                }
            }
        }
        private void SwapColor(Bitmap image, Color oldColor, Color newColor)
        {
            int iColor = oldColor.ToArgb();
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color c = image.GetPixel(x, y);
                    if (c.ToArgb() == iColor)
                    {
                        image.SetPixel(x, y, newColor);
                    }
                }
            }
        }

        private void FrmCapture_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _bStartCapture = false;
                _isCapture = false;
                _enCaptureType = enCaptureType.None;
                Hide();
            }
             
        }
    }
    
}
