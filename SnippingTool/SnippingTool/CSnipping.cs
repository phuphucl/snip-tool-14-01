using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnippingTool
{
    [Flags]
    public enum enCaptureType
    {
        None = 0,
        [Description("Rectangular Snip")] Rectangular = 1,
        [Description("Oval Snip")] Ellipse = 2 | Rectangular,
        [Description("Circle Snip")] Circle = 4 | Rectangular,
        [Description("Polygon with equal sides")] PolyGonES = 8 | Rectangular,
        [Description("Polygon with equal sides")] Star = 16 | Rectangular,

        [Description("Polygon Snip")] Polygon = 2048,
        [Description("FreeForm Snip")] FreeForm = Polygon * 2,

        [Description("Window Snip")] Window = FreeForm * 16,
        [Description("Screen Snip")] Screen = Window * 2,
        [Description("AllScreens Snip")] AllScreens = Screen * 2
    }
    public static class CSnipping
    {
        public static bool Contains(this enCaptureType type, enCaptureType check)
        {
            return (type & check) == check;
        }
        public static void Offset(this List<Point> pts, int x, int y)
        {
            int count = pts.Count;
            for(int i=0; i<count; i++) pts[i] = new Point(pts[i].X + x, pts[i].Y + y);

        }
        public static Image Capture(this Form caller, enCaptureType captureType, params object[] options)
        {
           // if (caller != null) caller.Hide();
            //Delay(500);
            FrmCapture f = new FrmCapture();
            //Image image = f.Tag as Image;
            //if (caller != null) caller.Show();
            return f.CaptureImage(caller, captureType, options);
        }

        public static void Delay(int ms)
        {
            for (int tickCount = ms + Environment.TickCount; tickCount > Environment.TickCount; Application.DoEvents()) ;
        }

        public static bool GetRect(this IEnumerable<Point> pts, out Rectangle rc)
        {
            rc = Rectangle.Empty;
            if (pts == null || pts.Count() < 2) return false;
            Point first = pts.First();
            Point last = pts.Last();

            int x1 = first.X;
            int y1 = first.Y;
            int x2 = last.X;
            int y2 = last.Y;

            int w = x2 - x1;
            int h = y2 - y1;
            if (w == 0 || h == 0) return false;

            if (w < 0)
            {
                x1 = x2;
                w = -w;
            }

            if (h < 0)
            {
                y1 = y2;
                h = -h;
            }
            rc = new Rectangle(x1, y1, w, h);

            return true;
        }
    }
}
