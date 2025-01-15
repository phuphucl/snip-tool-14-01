namespace SnippingTool
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnSnip = new System.Windows.Forms.Button();
            this.Grbox1 = new System.Windows.Forms.GroupBox();
            this.TxtSide = new System.Windows.Forms.TextBox();
            this.OptPolygonES = new System.Windows.Forms.RadioButton();
            this.OptAllScreens = new System.Windows.Forms.RadioButton();
            this.OptScreen = new System.Windows.Forms.RadioButton();
            this.OptPolygon = new System.Windows.Forms.RadioButton();
            this.OptFreeForm = new System.Windows.Forms.RadioButton();
            this.OptCircular = new System.Windows.Forms.RadioButton();
            this.OptEllipse = new System.Windows.Forms.RadioButton();
            this.OptRectangular = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.OptStar = new System.Windows.Forms.RadioButton();
            this.Grbox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnSnip
            // 
            this.BtnSnip.Location = new System.Drawing.Point(600, 19);
            this.BtnSnip.Name = "BtnSnip";
            this.BtnSnip.Size = new System.Drawing.Size(75, 23);
            this.BtnSnip.TabIndex = 1;
            this.BtnSnip.Text = "Snip";
            this.BtnSnip.UseVisualStyleBackColor = true;
            this.BtnSnip.Click += new System.EventHandler(this.BtnSnip_Click);
            // 
            // Grbox1
            // 
            this.Grbox1.Controls.Add(this.OptStar);
            this.Grbox1.Controls.Add(this.TxtSide);
            this.Grbox1.Controls.Add(this.OptPolygonES);
            this.Grbox1.Controls.Add(this.OptAllScreens);
            this.Grbox1.Controls.Add(this.OptScreen);
            this.Grbox1.Controls.Add(this.OptPolygon);
            this.Grbox1.Controls.Add(this.OptFreeForm);
            this.Grbox1.Controls.Add(this.OptCircular);
            this.Grbox1.Controls.Add(this.OptEllipse);
            this.Grbox1.Controls.Add(this.OptRectangular);
            this.Grbox1.Controls.Add(this.BtnSnip);
            this.Grbox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Grbox1.Location = new System.Drawing.Point(0, 358);
            this.Grbox1.Name = "Grbox1";
            this.Grbox1.Size = new System.Drawing.Size(681, 92);
            this.Grbox1.TabIndex = 3;
            this.Grbox1.TabStop = false;
            // 
            // TxtSide
            // 
            this.TxtSide.Location = new System.Drawing.Point(600, 46);
            this.TxtSide.Name = "TxtSide";
            this.TxtSide.Size = new System.Drawing.Size(75, 20);
            this.TxtSide.TabIndex = 10;
            this.TxtSide.Text = "3";
            // 
            // OptPolygonES
            // 
            this.OptPolygonES.AutoSize = true;
            this.OptPolygonES.Location = new System.Drawing.Point(13, 47);
            this.OptPolygonES.Name = "OptPolygonES";
            this.OptPolygonES.Size = new System.Drawing.Size(136, 17);
            this.OptPolygonES.TabIndex = 9;
            this.OptPolygonES.Text = "Polygon w/ Same sides";
            this.OptPolygonES.UseVisualStyleBackColor = true;
            // 
            // OptAllScreens
            // 
            this.OptAllScreens.AutoSize = true;
            this.OptAllScreens.Location = new System.Drawing.Point(423, 22);
            this.OptAllScreens.Name = "OptAllScreens";
            this.OptAllScreens.Size = new System.Drawing.Size(78, 17);
            this.OptAllScreens.TabIndex = 8;
            this.OptAllScreens.Text = "All Screens";
            this.OptAllScreens.UseVisualStyleBackColor = true;
            // 
            // OptScreen
            // 
            this.OptScreen.AutoSize = true;
            this.OptScreen.Location = new System.Drawing.Point(358, 22);
            this.OptScreen.Name = "OptScreen";
            this.OptScreen.Size = new System.Drawing.Size(59, 17);
            this.OptScreen.TabIndex = 7;
            this.OptScreen.Text = "Screen";
            this.OptScreen.UseVisualStyleBackColor = true;
            // 
            // OptPolygon
            // 
            this.OptPolygon.AutoSize = true;
            this.OptPolygon.Location = new System.Drawing.Point(211, 22);
            this.OptPolygon.Name = "OptPolygon";
            this.OptPolygon.Size = new System.Drawing.Size(63, 17);
            this.OptPolygon.TabIndex = 6;
            this.OptPolygon.Text = "Polygon";
            this.OptPolygon.UseVisualStyleBackColor = true;
            // 
            // OptFreeForm
            // 
            this.OptFreeForm.AutoSize = true;
            this.OptFreeForm.Location = new System.Drawing.Point(280, 22);
            this.OptFreeForm.Name = "OptFreeForm";
            this.OptFreeForm.Size = new System.Drawing.Size(72, 17);
            this.OptFreeForm.TabIndex = 5;
            this.OptFreeForm.Text = "Free Form";
            this.OptFreeForm.UseVisualStyleBackColor = true;
            // 
            // OptCircular
            // 
            this.OptCircular.AutoSize = true;
            this.OptCircular.Location = new System.Drawing.Point(154, 22);
            this.OptCircular.Name = "OptCircular";
            this.OptCircular.Size = new System.Drawing.Size(51, 17);
            this.OptCircular.TabIndex = 4;
            this.OptCircular.Text = "Circle";
            this.OptCircular.UseVisualStyleBackColor = true;
            // 
            // OptEllipse
            // 
            this.OptEllipse.AutoSize = true;
            this.OptEllipse.Location = new System.Drawing.Point(93, 22);
            this.OptEllipse.Name = "OptEllipse";
            this.OptEllipse.Size = new System.Drawing.Size(55, 17);
            this.OptEllipse.TabIndex = 3;
            this.OptEllipse.Text = "Ellipse";
            this.OptEllipse.UseVisualStyleBackColor = true;
            // 
            // OptRectangular
            // 
            this.OptRectangular.AutoSize = true;
            this.OptRectangular.Location = new System.Drawing.Point(13, 22);
            this.OptRectangular.Name = "OptRectangular";
            this.OptRectangular.Size = new System.Drawing.Size(74, 17);
            this.OptRectangular.TabIndex = 2;
            this.OptRectangular.Text = "Rectangle";
            this.OptRectangular.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(681, 358);
            this.panel1.TabIndex = 5;
            // 
            // OptStar
            // 
            this.OptStar.AutoSize = true;
            this.OptStar.Location = new System.Drawing.Point(154, 47);
            this.OptStar.Name = "OptStar";
            this.OptStar.Size = new System.Drawing.Size(44, 17);
            this.OptStar.TabIndex = 11;
            this.OptStar.Text = "Star";
            this.OptStar.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Grbox1);
            this.Name = "Form1";
            this.Text = "Snipping Tool";
            this.Grbox1.ResumeLayout(false);
            this.Grbox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnSnip;
        private System.Windows.Forms.GroupBox Grbox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton OptEllipse;
        private System.Windows.Forms.RadioButton OptRectangular;
        private System.Windows.Forms.RadioButton OptScreen;
        private System.Windows.Forms.RadioButton OptPolygon;
        private System.Windows.Forms.RadioButton OptFreeForm;
        private System.Windows.Forms.RadioButton OptCircular;
        private System.Windows.Forms.RadioButton OptAllScreens;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.RadioButton OptPolygonES;
        private System.Windows.Forms.TextBox TxtSide;
        private System.Windows.Forms.RadioButton OptStar;
    }
}

