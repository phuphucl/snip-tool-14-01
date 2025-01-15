using PLLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SnippingTool
{
    internal interface IShapeCB
    {
        void Refresh();
        void EndCapture(bool success);
    }
    internal abstract class CShape
    {
        protected List<Point> _Pts = new List<Point>();
        protected Rectangle _rc = new Rectangle();

        protected bool _isCapture = false;
        protected Point _ptMouseDown;
        protected bool _bEdit = false;
        protected int _iSelectedRegion = 0;
        protected Region _rPolygon = null;
        protected Region _rSelected = null;
        protected List<Region> _regions = new List<Region>();
        protected bool _bStartCapture = false;
        protected IShapeCB _shapeCB;
        protected int _iNumberOfSides = 3;
        protected PointF[] vertices = null;

        public CShape(IShapeCB cb)
        {
            _shapeCB = cb;
        }

        //public event EventHandler<bool> EndCapture;
        //public delegate void EndCaptureDelegate(CShape sender, bool success);
        //public event EndCaptureDelegate EndCapture;

        public virtual void OnMouseDown(MouseButtons buttons, int x, int y) { }
        public virtual void OnMouseMove(MouseButtons buttons, int x, int y) { }
        public virtual void OnMouseUp(MouseButtons buttons, int x, int y) { }

        public virtual void OnMouseClick(MouseButtons buttons, int x, int y) { }
        public virtual void OnPaint(Graphics graphics) { }
        public virtual Image CreateImage() => null;


        public void CreatePolygonRegion()
        {
            if (_rPolygon != null) _rPolygon.Dispose();
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(_Pts.ToArray());
                _rPolygon = new Region(path);
            }
        }
        public void CreateEllipticRegion(int index)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(_Pts[index].X - 3, _Pts[index].Y - 3, 6, 6);
                _regions.Add(new Region(path));
            }
        }
        public void EditEllipticRegion(int index)
        {
            _regions[index].Dispose();
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(_Pts[index].X - 3, _Pts[index].Y - 3, 6, 6);
                _regions[index] = new Region(path);
            }
        }

    }
    //==========================================================================
    internal class RectShape : CShape
    {
        public RectShape(IShapeCB cb) : base(cb)
        {

        }
        public override void OnMouseDown(MouseButtons buttons, int x, int y)
        {
            if (buttons == MouseButtons.Left)
            {
                _Pts.Add(new Point(x, y));
                _bStartCapture = true;
            }
        }
        public override void OnMouseMove(MouseButtons buttons, int x, int y)
        {
            if (_bStartCapture)
            {
                if (_Pts.Count == 2) _Pts[1] = new Point(x, y);
                else _Pts.Add(new Point(x, y));
                _shapeCB.Refresh();
            }
        }

        public override void OnPaint(Graphics graphics)
        {
            _isCapture = _Pts.GetRect(out _rc);
            if (_isCapture)
            {
                graphics.FillRectangle(Brushes.White, _rc);
            }
        }
        public override void OnMouseUp(MouseButtons buttons, int x, int y)
        {
            _shapeCB.EndCapture(_isCapture);
        }
        public override Image CreateImage()
        {
            return _isCapture ? _rc.CaptureImage() : null;
        }
    }
    //==========================================================================
    internal class EllipseShape : RectShape
    {
        public EllipseShape(IShapeCB cb) : base(cb) { }
        public override void OnPaint(Graphics graphics)
        {
            _isCapture = _Pts.GetRect(out _rc);
            if (_isCapture)
            {
                graphics.FillEllipse(Brushes.White, _rc);
            }
        }
        public override Image CreateImage()
        {
            Image rectImage = _rc.CaptureImage(PixelFormat.Format32bppArgb);

            Bitmap image = new Bitmap(_rc.Width, _rc.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, _rc.Width, _rc.Height);
            Region region = new Region(path);
            graphics.SetClip(region, CombineMode.Replace);
            graphics.DrawImage(rectImage, 0, 0);
            // _image = image;

            graphics.Dispose();
            region.Dispose();
            path.Dispose();
            return image;
        }
    }

    //==========================================================================
    internal class OvalShape : EllipseShape
    {
        public OvalShape(IShapeCB cb) : base(cb) { }
        public override void OnPaint(Graphics graphics)
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
                graphics.FillEllipse(Brushes.White, _rc);
            }
        }
    }

    //==========================================================================
    internal class FreeFormShape : OvalShape
    {
        public FreeFormShape(IShapeCB cb) : base(cb) { }
        public override void OnMouseMove(MouseButtons buttons, int x, int y)
        {
            if (_bStartCapture)
            {
                _Pts.Add(new Point(x, y));
                _shapeCB.Refresh();
            }
        }

        public override void OnPaint(Graphics graphics)
        {
            if (_Pts.Count > 2)
            {
                graphics.DrawLines(Pens.Red, _Pts.ToArray());
                _isCapture = true;
            }
        }

        public override Image CreateImage()
        {
            if (_isCapture)
            {
                int count = _Pts.Count;
                if (count < 3) return null;
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
                //path.Reset();
                Point[] array = new Point[count];
                int index = 0;
                foreach (Point point in _Pts)
                {
                    array[index].X = point.X - xMin;
                    array[index].Y = point.Y - yMin;
                    index++;
                }
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddClosedCurve(array);
                    Region region = new Region(path);
                    g.SetClip(region, CombineMode.Replace);
                    g.DrawImage(rectImage, 0, 0);
                    g.Dispose();
                    region.Dispose();
                }
                return image;
            }
            return null;

        }
    }

        //==========================================================================
    internal class PolygonShape : FreeFormShape
    {
        public PolygonShape(IShapeCB cb) : base(cb) { }

        public override void OnMouseUp(MouseButtons buttons, int x, int y)
        {
            return;
        }
        public override void OnMouseDown(MouseButtons buttons, int x, int y)
        {
            if (Cursor.Current == Cursors.SizeAll)
            {
                Cursor.Current = Cursors.SizeAll;
                _ptMouseDown = new Point(x,y);
                _bEdit = true;
            }
            else if (_Pts.Contains(new Point(x, y)))
            {
                _iSelectedRegion = _Pts.IndexOf(new Point(x, y));
                _bEdit = true;
            }
        }

        public override void OnMouseMove(MouseButtons buttons, int x, int y)
        {
            if (buttons == MouseButtons.Left)
            {
                if (Cursor.Current == Cursors.SizeAll)
                {
                    int dx = x - _ptMouseDown.X;
                    int dy = y - _ptMouseDown.Y;
                    _ptMouseDown.X = x;
                    _ptMouseDown.Y = y;
                    _rPolygon.Translate(dx, dy);
                    _Pts.Offset(dx, dy);
                    foreach (Region r in _regions) r.Translate(dx, dy);
                    _shapeCB.Refresh();
                }
                else if (Cursor.Current == Cursors.Hand)
                {
                    _Pts[_iSelectedRegion] = new Point(x, y);
                    CreatePolygonRegion();
                    EditEllipticRegion(_iSelectedRegion);
                    _shapeCB.Refresh();

                }
            }
            else if (buttons == MouseButtons.None)
            {
                if (_rPolygon == null) return;
                if (Cursor.Current == Cursors.Default)
                {
                    int count = _regions.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (_regions[i].IsVisible(new Point(x, y)))
                        {
                            Cursor.Current = Cursors.Hand;
                            _iSelectedRegion = i;
                            return;
                        }
                    }
                    if (_rPolygon.IsVisible(new Point(x, y)))
                    {
                        Cursor.Current = Cursors.SizeAll;
                    }
                }
                else
                {
                    if (_rPolygon.IsVisible(new Point(x, y)))
                    {
                        return;
                    }

                    foreach (Region r in _regions)
                    {
                        if (r.IsVisible(new Point(x, y)))
                        {
                            return;
                        }
                    }
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        public override void OnPaint(Graphics graphics)
        {
            if (_rPolygon != null)
            {
                graphics.FillRegion(Brushes.White, _rPolygon);
            }
            foreach (Point pt in _Pts)
            {
                _isCapture = true;
                graphics.FillEllipse(Brushes.Gray, pt.X - 3, pt.Y - 3, 6, 6);
                graphics.FillEllipse(Brushes.Red, pt.X - 1, pt.Y - 1, 2, 2);
            }
        }

        public override void OnMouseClick(MouseButtons buttons, int x, int y)
        {
            if (buttons == MouseButtons.Left)
            {
                if (_bEdit) return;
                _bStartCapture = true;
                _Pts.Add(new Point(x, y));
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

                    for (int i = 0; i < count; i++)
                    {
                        CreateEllipticRegion(i);
                    }
                }
                _shapeCB.Refresh();
            }
            else if (buttons == MouseButtons.Right)
            {
                if (_isCapture)
                {
                    _shapeCB.EndCapture(_isCapture);
                }
            }
        }

        public override Image CreateImage()
        {
            int count = _Pts.Count;
            if (count < 3) return null;
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
            path.AddPolygon(array);
            Region region = new Region(path);
            g.SetClip(region, CombineMode.Replace);
            g.DrawImage(rectImage, 0, 0);
            //image = image;

            g.Dispose();
            region.Dispose();
            return image;
        }
    }

    // =====================================================
    internal class PolygonESShape : RectShape
    {
        public PolygonESShape(IShapeCB cb, PointF[] vertices, int _iNumberOfSides) : base(cb)
        {
            this.vertices = vertices;
            this._iNumberOfSides = _iNumberOfSides;
        }

        public override void OnPaint(Graphics graphics)
        {
            if (_Pts.Count > 1)
            {
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
                graphics.FillPolygon(Brushes.Red, vertices);
            }
        }

        public override Image CreateImage()
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
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(array);
                    Region region = new Region(path);
                    g.SetClip(region, CombineMode.Replace);
                    g.DrawImage(rectImage, 0, 0);
                    g.Dispose();
                    region.Dispose();
                }
                return image;
            }
            return null;
        }
    }

    // =====================================================
    internal class Star : PolygonESShape
    {
        public Star(IShapeCB cb, PointF[] vertices, int _iNumberOfSides) : base(cb, vertices, _iNumberOfSides)
        {
            this.vertices = vertices;
            this._iNumberOfSides = _iNumberOfSides;
        }

        public override void OnPaint(Graphics graphics)
        {
            if (_Pts.Count > 1)
            {
                double w = _Pts[1].X - _Pts[0].X;
                double h = _Pts[1].Y - _Pts[0].Y;
                if (w == 0 && h == 0) return;
                _isCapture = true;

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
                for (int i = 0; i < _iNumberOfSides * 2; i++)
                {
                    double radius = bigRadius;
                    if (i % 2 == 1) radius = smallRadius;
                    vertices[i].X = center.X + (float)(radius * Math.Cos(angle));
                    vertices[i].Y = center.Y + (float)(radius * Math.Sin(angle));
                    angle += angleInc;
                }
                graphics.FillPolygon(Brushes.Red, vertices);
            }
        }
    }
}
