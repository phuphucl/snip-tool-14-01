using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnippingTool
{
    public partial class FrmCapture : Form, IShapeCB
    {
        CShape _shape;
        int _iNumberOfSides;
        PointF[] vertices;
        public FrmCapture()
        {
            InitializeComponent();
            base.MouseDown += FrmCapture_MouseDown;
            base.Paint += FrmCapture_Paint;
        }        

        public Image CaptureImage(Form caller, enCaptureType type, params object[] options)
        {
            if (options.Length > 0)
            {
                if (options[0] is int side) _iNumberOfSides = side;
                if (type == enCaptureType.PolyGonES) vertices = new PointF[_iNumberOfSides];
                else if (type == enCaptureType.Star) vertices = new PointF[_iNumberOfSides * 2];
            }
            if (type == enCaptureType.Rectangular) _shape = new RectShape(this);
            else if (type == enCaptureType.Ellipse) _shape = new EllipseShape(this);
            else if (type == enCaptureType.Circle) _shape = new OvalShape(this);
            else if (type == enCaptureType.FreeForm) _shape = new FreeFormShape(this);
            else if (type == enCaptureType.Polygon) _shape = new PolygonShape(this);
            else if (type == enCaptureType.PolyGonES) _shape = new PolygonESShape(this, vertices, _iNumberOfSides);
            else if (type == enCaptureType.Star) _shape = new Star(this, vertices, _iNumberOfSides);


            //Set form property
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            Opacity = 0.5;
            Rectangle _rcBound = Rectangle.Empty;
            Screen[] allScreen = Screen.AllScreens;
            foreach (Screen screen in allScreen)
            {
                _rcBound = Rectangle.Union(screen.Bounds, _rcBound);
            }
            Bounds = _rcBound;
            //TopMost = true;
            
            ShowDialog();
            return Tag as Image;
        }

        void IShapeCB.EndCapture(bool success)
        {
            Hide();
            if (success)
            {
                Tag = _shape.CreateImage();
            }
        }
        private void FrmCapture_MouseDown(object sender, MouseEventArgs e)
        {
            _shape.OnMouseDown(e.Button, e.X, e.Y);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _shape.OnMouseMove(e.Button, e.X, e.Y);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _shape.OnMouseUp(e.Button, e.X, e.Y);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            _shape.OnMouseClick(e.Button, e.X, e.Y);
        }
        private void FrmCapture_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _shape.OnPaint(g);
        }
    }
}
