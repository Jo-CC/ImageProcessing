using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        int bitSlice = 1;
        int avalue = 1;
        double bvalue = 1;
        Bitmap bmp;
        public Point current;
        Color mlinecolor;
        FFT ImgFFT;
        public int rec_width, rec_height;
        public int scale = 25; // Scaling percentage
        public int WindowSize = 256;
        Bitmap InputImage;
        Bitmap SelectedImage;
        Processor img_proc = new Processor();
        Histogram histo1 = null;
        Histogram histo2 = null;
        public Form1()
        {
            InitializeComponent();
        }
        Canny CannyData;

        private void insertImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog oripic = new OpenFileDialog();
            oripic.Filter = "Image file | *.jpg;*.png;*.bmp;.jfif";
            if (DialogResult.OK == oripic.ShowDialog())
            {
                this.ori_pic.Image = new Bitmap(oripic.FileName);
            }
        }

        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap neg_img = new Bitmap((Bitmap)this.ori_pic.Image);
            neg_img = NegativeImg.Ori2Neg(neg_img);
            processed_img.Image = neg_img;
        }

        private void graySliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap gs_img = new Bitmap((Bitmap)this.ori_pic.Image);
            gs_img = GraySlice.GSlice(gs_img);
            processed_img.Image = gs_img;
        }

        private void level1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
            bps_img = BPSlicing.BPSlice(bps_img, bitSlice);
            processed_img.Image = bps_img;
            
        }
        //private void level2ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    bps_img = BPSlicing.BPSlice(bps_img, 2);
        //    processed_img.Image = bps_img;
        //}

        //private void level3ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    bps_img = BPSlicing.BPSlice(bps_img, 3);
        //    processed_img.Image = bps_img;
        //}


        //private void level4ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    bps_img = BPSlicing.BPSlice(bps_img, 4);
        //    processed_img.Image = bps_img;
        //}

        //private void level5ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    bps_img = BPSlicing.BPSlice(bps_img, 5);
        //    processed_img.Image = bps_img;
        //}
        //private void level6ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    bps_img = BPSlicing.BPSlice(bps_img, 6);
        //    processed_img.Image = bps_img;
        //}

        //private void level7ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    bps_img = BPSlicing.BPSlice(bps_img, 7);
        //    processed_img.Image = bps_img;
        //}

        //private void level8ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    bps_img = BPSlicing.BPSlice(bps_img, 8);
        //    processed_img.Image = bps_img;
        //}

        //private void hEToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Bitmap he_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    he_img = HisEq.HisEqual(he_img);
        //    processed_img.Image = he_img;
        //}

        //private void cLAHEToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Image clahe_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    int split = 8;
        //    double limit = 40;
        //    clahe_img = Clahe.SeparatedChannelCLAHE(clahe_img, split, limit);
        //    processed_img.Image = clahe_img;
        //}

        //private void cOLORCLAHEToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Image colorclahe_img = new Bitmap((Bitmap)this.ori_pic.Image);
        //    int split = 8;
        //    double limit = 40;
        //    colorclahe_img = ColorClahe.SeparatedChannelCLAHE(colorclahe_img, split, limit);
        //    processed_img.Image = colorclahe_img;
            
        //}

        private void monochromeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap rgb2Gray_img = new Bitmap((Bitmap)this.ori_pic.Image);
            rgb2Gray_img = Fastcolor2mono.Grayscale_Unsafe(rgb2Gray_img);
            processed_img.Image = rgb2Gray_img;
        }

        private void MedianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap mf_img = new Bitmap((Bitmap)this.ori_pic.Image);
            mf_img = MFilter.MedianFilter(mf_img, 3*3, 0, false);
            processed_img.Image = mf_img;
            sw.Stop();
            string algorithmMethod = "Median Filter";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void lapacianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap lpc_img = new Bitmap((Bitmap)this.ori_pic.Image);
            lpc_img = lpc_img.ConvolutionFilter(Matrix.Laplacian, 1, 0);
            processed_img.Image = lpc_img;
            sw.Stop();
            string algorithmMethod = "Laplacian's Filter";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void perwittsFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap PW_img = new Bitmap((Bitmap)this.ori_pic.Image);
            PW_img = CV.ConvolutionFilter(PW_img, Matrix.Perwittsx, 1, 0);
            PW_img = CV.ConvolutionFilter(PW_img, Matrix.Perwittsy, 1, 0);
            processed_img.Image = PW_img;
            sw.Stop();
            string algorithmMethod = "Prewitt's Filter";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void sorbelsFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap sb_img = new Bitmap((Bitmap)this.ori_pic.Image);
            sb_img = CV.ConvolutionFilter(sb_img, Matrix.Sorbelx, 1, 0);
            sb_img = CV.ConvolutionFilter(sb_img, Matrix.Sorbely, 1, 0);
            processed_img.Image = sb_img;
            sw.Stop();
            string algorithmMethod = "Sobel's Filter";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void lowPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Bitmap lp_img = new Bitmap((Bitmap)this.ori_pic.Image);
            //lp_img = CV.ConvolutionFilter(lp_img, Matrix.Mean5x5, 1.0/25.0, 0);
            //processed_img.Image = lp_img;
            Bitmap bb_img = new Bitmap((Bitmap)this.ori_pic.Image);
            bb_img = CV.ConvolutionFilter(bb_img, Matrix.Mean3x3, 1.0/9, 0);
            processed_img.Image = bb_img;
            sw.Stop();
            string algorithmMethod = "Fast Box Filter";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void negativeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Bitmap lpc_img = new Bitmap((Bitmap)this.ori_pic.Image);
            lpc_img = CV.ConvolutionFilter(lpc_img, Matrix.Laplacian, 1, 0);
            lpc_img = NegativeImg.Ori2Neg(lpc_img);
            processed_img.Image = lpc_img;
        }

        private void hEToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            Bitmap lpc_img = new Bitmap((Bitmap)this.ori_pic.Image);
            lpc_img = CV.ConvolutionFilter(lpc_img, Matrix.Laplacian, 1, 0);
            lpc_img = HisEq.HisEqual(lpc_img);
            processed_img.Image = lpc_img;
        }
        private int Bin1
        {
            get { return Convert.ToInt32(textBox1.Text); }
        }
        private int Bin2
        { 
            get { return Convert.ToInt32(textBox2.Text); } 
        }
        private int Bin3
        {
            get { return Convert.ToInt32(textBox3.Text); }
        }
        private Histogram CreateHistogram(Bitmap img)
        {
            
            return img_proc.Create1DHistogram(img, Bin1, Bin2 , Bin3, Convert.ToInt32(scalepercentage.Text));
        }
        private void DisplayHistogram(ref Histogram hist, PictureBox picBox, PictureBox histBox)
        {
            Image bmp = picBox.Image;
            if (bmp != null)
            {
                hist = CreateHistogram((Bitmap)bmp);
                histBox.Refresh(); //force paint event
            }
        }
        private void hEToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap he_img = new Bitmap((Bitmap)this.ori_pic.Image);
            he_img = HisEq.HisEqual(he_img);
            processed_img.Image = he_img;
            sw.Stop();
            string algorithmMethod = "HE";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void cLAHEToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Image clahe_img = new Bitmap((Bitmap)this.ori_pic.Image);
            int split = avalue;
            double limit = 40;
            clahe_img = Clahe.SeparatedChannelCLAHE(clahe_img, split, limit);
            processed_img.Image = clahe_img;
            sw.Stop();
            string algorithmMethod = "CLAHE";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void cOLORCLAHEToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Image colorclahe_img = new Bitmap((Bitmap)this.ori_pic.Image);
            int split = 8;
            double limit = 40;
            colorclahe_img = ColorClahe.SeparatedChannelCLAHE(colorclahe_img, split, limit);
            processed_img.Image = colorclahe_img;
        }

        private void bit_lvlslice_Scroll(object sender, EventArgs e)
        {
            bitSlice = int.Parse(bit_lvlslice.Value.ToString());
            Bitmap bps_img = new Bitmap((Bitmap)this.ori_pic.Image);
            bps_img = BPSlicing.BPSlice(bps_img, bitSlice);
            processed_img.Image = bps_img;
        }

        private void globalTresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap gtres_img = new Bitmap((Bitmap)this.ori_pic.Image);
            gtres_img = GlobalTRES.GlobalThresholding(gtres_img);
            processed_img.Image = gtres_img;
            
        }

        private void otsuTresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap otsu_img = new Bitmap((Bitmap)this.ori_pic.Image);
            otsu_img = OTSUtres.OtsuThresholding(otsu_img);
            processed_img.Image = otsu_img;

        }

        private void adaptiveTresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap adapt_img = new Bitmap((Bitmap)this.ori_pic.Image);
            adapt_img = AdaptiveTreshold.VariableThresholdingLocalProperties(adapt_img, 138, 0.03);
            processed_img.Image = adapt_img;
        }

       

        private void kMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap kms_img = new Bitmap((Bitmap)this.ori_pic.Image);
            kms_img = kMeans.KMeansClusteringSegmentation(kms_img, avalue);
            processed_img.Image = kms_img;
            sw.Stop();
            string algorithmMethod = "K-Mean";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }
        /// <param name="imgPhoto"></param>
        /// <param name="Percent"></param>
        public static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(imgPhoto,
            new Rectangle(destX, destY, destWidth, destHeight),
            new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
            GraphicsUnit.Pixel);
            grPhoto.Dispose();
            return bmPhoto;
        }
        private void ori_pic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            toolTip1.SetToolTip(ori_pic, e.X.ToString() + ", " + e.Y.ToString());
            Pen ppen = new Pen(mlinecolor, 1);
            Graphics g;
            ori_pic.Refresh();
            try
            {
                
                g = this.ori_pic.CreateGraphics();
                Rectangle rec = new Rectangle(e.X, e.Y, (int)(WindowSize * Convert.ToInt32(scalepercentage.Text) / 100), (int)(WindowSize * Convert.ToInt32(scalepercentage.Text) / 100));
                g.DrawRectangle(ppen, rec);
                current.X = e.X;
                current.Y = e.Y;
                ppen.Color = Color.Red;
                g.DrawLine(ppen, ori_pic.Width / 2, ori_pic.Top, ori_pic.Width / 2, ori_pic.Height);
                g.DrawLine(ppen, 0, ori_pic.Height / 2, ori_pic.Width, ori_pic.Height / 2);
                ppen.Color = Color.LightBlue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void fFTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int x, y, width, height;
            //Code for Preview
            //Application.DoEvents();
            ori_pic.Image = InputImage;
            //try
            //{
            //    Bitmap temp = (Bitmap)InputImage;
            //    //width = height = (int)(WindowSize * Convert.ToInt32(scalepercentage.Text) / 100);
            //    //bmp = new Bitmap(width, height, InputImage.PixelFormat);

            //    //x = (int)((float)current.X * (100 / Convert.ToDouble(scalepercentage.Text)));
            //    //y = (int)((float)current.Y * (100 / Convert.ToDouble(scalepercentage.Text)));
            //    //width = height = (int)(rec_width * (100 / (float)scale));
            //    //if (width > WindowSize)
            //    //{
            //    //    width = height = WindowSize;
            //    //}

            //    //Rectangle area = new Rectangle(x, y, width, height);
               
            //    //SelectedImage = bmp;
            //}
            //catch (System.OutOfMemoryException ex)
            //{
            //    MessageBox.Show("Select Area Inside Image only : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            ImgFFT = new FFT(bmp);

            ImgFFT.ForwardFFT();// Finding 2D FFT of Image
            ImgFFT.FFTShift();
            ImgFFT.FFTPlot(ImgFFT.FFTShifted);
            processed_img.Image = (Image)ImgFFT.FourierPlot;
            processed_img.Image = (Image)ImgFFT.PhasePlot;
            
            ImgFFT.InverseFFT();
            processed_img.Image = (Image)ImgFFT.Obj;

        }

        private void sLICToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap slic_img = new Bitmap((Bitmap)this.ori_pic.Image);
            slic_img = SLIC.Superpixels(slic_img,avalue);
            processed_img.Image = slic_img;
            sw.Stop();

            string algorithmMethod = "SLIC";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap GF_img = new Bitmap((Bitmap)this.ori_pic.Image);
            GF_img = CV.ConvolutionFilter(GF_img, Matrix.Gaussian3x3, 1.0 / 16, 0);
            processed_img.Image = GF_img;
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap GF5_img = new Bitmap((Bitmap)this.ori_pic.Image);
            GF5_img = CV.ConvolutionFilter(GF5_img, Matrix.Gaussian5x5, 1.0 / 273, 0);
            processed_img.Image = GF5_img;
            sw.Stop();

            string algorithmMethod = "Fast Gaussian Filter";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

       

        private void bval_ValueChanged(object sender, EventArgs e)
        {
            bvalue = double.Parse(bval.Value.ToString());
            
        }

        private void aval_ValueChanged(object sender, EventArgs e)
        {
            avalue = int.Parse(aval.Value.ToString());
            Bitmap adapt_img = new Bitmap((Bitmap)this.ori_pic.Image);
            adapt_img = AdaptiveTreshold.VariableThresholdingLocalProperties(adapt_img, avalue, bvalue);
            processed_img.Image = adapt_img;
        }

        private void globalThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap clahe_img = new Bitmap((Bitmap)this.ori_pic.Image);
            int split = 8;
            double limit = 40;
            clahe_img = Clahe.CLAHE(clahe_img,split,limit);
            clahe_img = GlobalTRES.GlobalThresholding(clahe_img);
            processed_img.Image = clahe_img;
        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap dilation_img = new Bitmap((Bitmap)this.ori_pic.Image);
            dilation_img = Dilation.imgDilation(dilation_img, 3);
            processed_img.Image = dilation_img;
            sw.Stop();

            string algorithmMethod = "Dilation";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap erosion_img = new Bitmap((Bitmap)this.ori_pic.Image);
            erosion_img = Erosion.imgErosion(erosion_img, 3);
            processed_img.Image = erosion_img;
            sw.Stop();

            string algorithmMethod = "Erosion";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void openingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap opening_img = new Bitmap((Bitmap)this.ori_pic.Image);
            opening_img = Erosion.imgErosion(opening_img, 3);
            opening_img = Dilation.imgDilation(opening_img, 3);
            processed_img.Image = opening_img;
            sw.Stop();

            string algorithmMethod = "Opening";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void closingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap closing_img = new Bitmap((Bitmap)this.ori_pic.Image);
            closing_img = Dilation.imgDilation(closing_img, 3);
            closing_img = Erosion.imgErosion(closing_img, 3);
            processed_img.Image = closing_img;
            sw.Stop();

            string algorithmMethod = "Closing";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void stage1GaussianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CannyData = new Canny((Bitmap)ori_pic.Image);
            processed_img.Image = CannyData.DisplayImage(CannyData.FilteredImage);
            sw.Stop();

            string algorithmMethod = "Stage 1";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void stage2NonMaxSuppresionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            CannyData = new Canny((Bitmap)ori_pic.Image);
            processed_img.Image = CannyData.DisplayImage(CannyData.NonMax);
            sw.Stop();

            string algorithmMethod = "Stage 2";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void highToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CannyData = new Canny((Bitmap)ori_pic.Image);
            processed_img.Image = CannyData.DisplayImage(CannyData.GNH);
            sw.Stop();

            string algorithmMethod = "High Intensity Edge";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void lowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CannyData = new Canny((Bitmap)ori_pic.Image);
            processed_img.Image = CannyData.DisplayImage(CannyData.GNL);
            sw.Stop();

            string algorithmMethod = "Low Intensity Edge";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void stage4FinalResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CannyData = new Canny((Bitmap)ori_pic.Image);
            processed_img.Image = CannyData.DisplayImage(CannyData.EdgeMap);
            sw.Stop();

            string algorithmMethod = "Canny Edge Detection";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

       

        private void selectImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x, y, width, height;
            //Code for Preview
            //Application.DoEvents();
            try
            {
                Bitmap temp = (Bitmap)InputImage.Clone();
                width = height = (int)(WindowSize * Convert.ToInt32(scalepercentage.Text) / 100);
                bmp = new Bitmap(width, height, InputImage.PixelFormat);

                x = (int)((float)current.X * (100 / Convert.ToDouble(scalepercentage.Text)));
                y = (int)((float)current.Y * (100 / Convert.ToDouble(scalepercentage.Text)));
                width = height = (int)(rec_width * (100 / (float)scale));
                if (width > WindowSize)
                {
                    width = height = WindowSize;
                }

                Rectangle area = new Rectangle(x, y, width, height);
                bmp = (Bitmap)InputImage.Clone(area, InputImage.PixelFormat);
                SelectedImage = bmp;
            }
            catch (System.OutOfMemoryException ex)
            {
                MessageBox.Show("Select Area Inside Image only : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DisplayModel(Histogram model, int height, int width, PaintEventArgs e, Color color, int binRange)
        {
            if (model != null)
            {
                Pen p = new Pen(color, 1);
                SolidBrush b = new SolidBrush(color);
                float[] copy = new float[model.Data.Length];
                Array.Copy(model.Data, copy, model.Data.Length);
                Array.Sort(copy);
                float max = copy[copy.Length - 1];
                float scale = height / max;
                if (float.IsNaN(scale))
                    scale = 1;
                //Approximation: divide by total bins and remove 4 from width to account for picbox border
                float w = (float)(width - 4) / (float)(model.Data.Length);
                for (int count = 0; count < model.Data.Length; count++)
                {
                    if (model.Data[count] > 0)
                    {
                        e.Graphics.DrawRectangle(p, (float)count * w, (float)height - (model.Data[count] * scale), w, model.Data[count] * scale);
                        e.Graphics.FillRectangle(b, (float)count * w, (float)height - (model.Data[count] * scale), w, model.Data[count] * scale);

                    }
                }
            }
        }
        private void segmentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //private void pictureBox1_Paint(object sender, PaintEventArgs e)
        //{
        //    DisplayModel(histo, pictureBox1.Height, 512, e, Color.Red, Bin1 * Bin2 * Bin3);
        //}
        //private void pictureBox2_Paint(object sender, PaintEventArgs e)
        //{
        //    DisplayModel(histo, pictureBox2.Height, 512, e, Color.Green, Bin1 * Bin2 * Bin3);
        //}
        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            DisplayModel(histo1, pictureBox3.Height, 512, e, Color.Blue, Bin1 * Bin2 * Bin3);
        }
        private void pictureBox4_Paint(object sender, PaintEventArgs e)
        {
            DisplayModel(histo2, pictureBox4.Height, 512, e, Color.Black, Bin1 * Bin2 * Bin3);
        }
        private float Bhattacharyya(Histogram hist1, Histogram hist2)
        {
            double coeff = 0;
            float t = 0;
            for (int i = 0; i < hist1.Data.Length; i++)
            {
                coeff += Math.Sqrt((double)hist1.Data[i] * (double)hist2.Data[i]);
                t += hist1.Data[i];
            }
            return (float)coeff;
        }
       
        private int c
        {
            get { return Convert.ToInt32(scalepercentage.Text); }
        }
        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            { DisplayHistogram(ref histo1, pictureBox1, pictureBox2); }
          
            { DisplayHistogram(ref histo2, pictureBox3, pictureBox4); }


                
           
            
        }

        private void bhaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((histo1 != null) && (histo2 != null))
            {
                MessageBox.Show("Bhattacharyya Coefficient: " + Bhattacharyya(histo1, histo2).ToString());
            }
            else
            {
                MessageBox.Show("First select two images");
            }
        }

        private void srcImg1_Click(object sender, EventArgs e)
        {
            OpenFileDialog oripic = new OpenFileDialog();
            oripic.Filter = "Image file | *.jpg;*.png;*.bmp;.jfif";
            if (DialogResult.OK == oripic.ShowDialog())
            {
                this.pictureBox1.Image = new Bitmap(oripic.FileName);
            }
        }

        private void srcImg2_Click(object sender, EventArgs e)
        {
            OpenFileDialog oripic = new OpenFileDialog();
            oripic.Filter = "Image file | *.jpg;*.png;*.bmp;.jfif";
            if (DialogResult.OK == oripic.ShowDialog())
            {
                this.pictureBox3.Image = new Bitmap(oripic.FileName);
            }
        }

        private void fastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap bb_img = new Bitmap((Bitmap)this.ori_pic.Image);
          
            bb_img = FastBOXBlur.Gaussain(bb_img, 1);
            //bb_img = FastBOXBlur.FastBoxBlur(bb_img, 3);
            //bb_img = FastBOXBlur.FastBoxBlur(bb_img, 3);
            processed_img.Image = bb_img;

            sw.Stop();

            string algorithmMethod = "Normal Gaussian";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void fastToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap bb_img = new Bitmap((Bitmap)this.ori_pic.Image);

            bb_img = FastBOXBlur.FastBoxBlur(bb_img, 3);
            //bb_img = FastBOXBlur.FastBoxBlur(bb_img, 3);
            //bb_img = FastBOXBlur.FastBoxBlur(bb_img, 3);
            processed_img.Image = bb_img;

            sw.Stop();

            string algorithmMethod = "Box Filter";
            //medianFilter.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void unsafeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap rgb2Gray_img = new Bitmap((Bitmap)this.ori_pic.Image);
            rgb2Gray_img = Fastcolor2mono.Grayscale_Unsafe(rgb2Gray_img);
            processed_img.Image = rgb2Gray_img;
            sw.Stop();
            string algorithmMethod = "Unsafe";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void marshallCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap mc_img = new Bitmap((Bitmap)this.ori_pic.Image);
            mc_img = imageAqui.marshallCopy(mc_img);
            processed_img.Image = mc_img;
            sw.Stop();
            string algorithmMethod = "Marshall Copy";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void getPixelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap gpixel = new Bitmap((Bitmap)this.ori_pic.Image);
            gpixel = imageAqui.getPixel(gpixel);
            processed_img.Image = gpixel;
            sw.Stop();
            string algorithmMethod = "Get Pixel";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Image file | *.jpg;*.png;*.bmp;*.jfif";
            if (DialogResult.OK == sfd.ShowDialog())
            {
                this.processed_img.Image.Save(sfd.FileName, ImageFormat.Jpeg);
            }
        }

        private void cOLORHEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Bitmap he_img = new Bitmap((Bitmap)this.ori_pic.Image);
            he_img = HisEq.colorHisEqual(he_img);
            processed_img.Image = he_img;
            sw.Stop();
            string algorithmMethod = "COLOR HE";
            label5.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }

        private void stage2SobelsFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CannyData = new Canny((Bitmap)ori_pic.Image);
            processed_img.Image = CannyData.DisplayImage(CannyData.FilteredImage);
            processed_img.Image = CV.ConvolutionFilter((Bitmap)processed_img.Image, Matrix.Sorbelx, 1, 0);
            processed_img.Image = CV.ConvolutionFilter((Bitmap)processed_img.Image, Matrix.Sorbely, 1, 0);
            sw.Stop();
            string algorithmMethod = "Sobel's";
            sobelFilter.Text = string.Format("{0} Time Used = {1} ms", algorithmMethod, sw.Elapsed.TotalMilliseconds);
        }







        //private void Save_btn_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog
        //}
    }
}
     

