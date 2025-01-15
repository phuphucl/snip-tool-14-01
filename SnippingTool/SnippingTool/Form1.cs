using PLLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnippingTool
{
    public partial class Form1 : Form
    {
        private enCaptureType _type;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            //pictureBox1.Resize += PictureBox1_Resize;
            SetCaptureTypeOption(OptRectangular, enCaptureType.Rectangular);
            SetCaptureTypeOption(OptCircular, enCaptureType.Circle);
            SetCaptureTypeOption(OptPolygon, enCaptureType.Polygon);
            SetCaptureTypeOption(OptEllipse, enCaptureType.Ellipse);
            SetCaptureTypeOption(OptFreeForm, enCaptureType.FreeForm);
            SetCaptureTypeOption(OptAllScreens, enCaptureType.AllScreens);
            SetCaptureTypeOption(OptScreen, enCaptureType.Screen);
            SetCaptureTypeOption(OptPolygonES, enCaptureType.PolyGonES);
            SetCaptureTypeOption(OptStar, enCaptureType.Star);

            OptRectangular.Checked = true;

        }

        private void SetCaptureTypeOption(RadioButton bt, enCaptureType captureType)
        {
            bt.Tag = captureType;
            bt.CheckedChanged += OptCaptureType_CheckChanged;
        }

        //private void PictureBox1_Resize(object sender, EventArgs e)
        //{
        //    panel1.AutoScroll = false;
        //    panel1.Refresh();
        //    int left = (panel1.Width - pictureBox1.Width) / 2;
        //    if (left < 0) left = 0;
        //    pictureBox1.Left = left;

        //    int top = (panel1.Height - pictureBox1.Height) / 2;
        //    if (top < 0) top = 0;
        //    pictureBox1.Top = top;
        //    panel1.AutoScroll = true;
        //    panel1.Refresh();
        //}

        private void BtnSnip_Click(object sender, EventArgs e)
        {
            int iNumberOfSides = TxtSide.Text.ToInt(3);
            Hide();
            Image image = this.Capture( _type, iNumberOfSides);
            if (image != null)
            {
                pictureBox1.Image = image;
            }
            Show();
           // pictureBox1.Image.Save("picture2.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void OptCaptureType_CheckChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button != null && button.Checked && button.Tag is enCaptureType captureType) _type = captureType;
        }
    }
}
