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


class GaussianFilterclass
{
    //public int Width, Height;
    //public Bitmap Obj;
    //public int[,] GreyImage;
    ////Gaussian Kernel Data
    //int[,] GaussianKernel;
    //int KernelWeight;
    //int KernelSize = 5;
    //float Sigma = 1;   // for N=2 Sigma =0.85  N=5 Sigma =1, N=9 Sigma = 2    2*Sigma = (int)N/2
    //                   //Canny Edge Detection Parameters
    //float MaxHysteresisThresh, MinHysteresisThresh;
    //public float[,] DerivativeX;
    //public float[,] DerivativeY;
    //public int[,] FilteredImage;
    //public float[,] Gradient;
    //public float[,] NonMax;
    //public int[,] PostHysteresis;
    //int[,] EdgePoints;
    //public float[,] GNH;
    //public float[,] GNL;
    //public int[,] EdgeMap;
    //public int[,] VisitedMap;
    //private Bitmap image;

    //public GaussianFilterclass(Bitmap image)
    //{
    //    this.image = image;
    //}

    //public void Canny(Bitmap Input, float Th, float Tl, int GaussianMaskSize, float SigmaforGaussianKernel)
    //{

    //    // Gaussian and Canny Parameters

    //    MaxHysteresisThresh = Th;
    //    MinHysteresisThresh = Tl;
    //    KernelSize = GaussianMaskSize;
    //    Sigma = SigmaforGaussianKernel;
    //    Obj = Input;
    //    Width = Obj.Width;
    //    Height = Obj.Height;

    //    EdgeMap = new int[Width, Height];
    //    VisitedMap = new int[Width, Height];

    //    ReadImage();
    //    return;
    //}
  
   
    //public Bitmap Ob;
    //int[,] GaussianKernel;
    //int KernelWeight;
    //int KernelSize = 5;
    //float Sigma = 1;   // for N=2 Sigma =0.85  N=5 Sigma =1, N=9 Sigma = 2    2*Sigma = (int)N/2
    //                   //Canny Edge Detection Parameters
    //public static int[,] GreyImage;
    //public int[,] FilteredImage;

    public static Bitmap GfilterRead(int[,] GreyImage)
    {
        //Bitmap img;
        int[,] GaussianKernel;
        int KernelWeight;
        int KernelSize = 5;
        float Sigma = 1;   // for N=2 Sigma =0.85  N=5 Sigma =1, N=9 Sigma = 2    2*Sigma = (int)N/2
                           //Canny Edge Detection Parameters
                           //int[,] GFimg;
        int[,] FilteredImage;
        
        
        int i, j;
        int W, H;
        W = GreyImage.GetLength(0);
        H = GreyImage.GetLength(1);
        Bitmap image = new Bitmap(W, H);
        BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, W, H),
                                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        unsafe
        {
            byte* imagePointer1 = (byte*)bitmapData1.Scan0;

            for (i = 0; i < bitmapData1.Height; i++)
            {
                for (j = 0; j < bitmapData1.Width; j++)
                {
                    GreyImage[j, i] = (int)((imagePointer1[0] + imagePointer1[1] + imagePointer1[2]) / 3.0);
                    //4 bytes per pixel
                    imagePointer1 += 4;
                }//end for j
                 //4 bytes per pixel
                imagePointer1 += bitmapData1.Stride - (bitmapData1.Width * 4);
            }//end for i
        }//end unsafe
        void GenerateGaussianKernel(int N, float S, out int Weight)
        {

            float Sigma = S = 1;
            float pi;
            pi = (float)Math.PI;
            int i, j;
            int SizeofKernel = N = 5;

            float[,] Kernel = new float[N, N];
            GaussianKernel = new int[N, N];
            float[,] OP = new float[N, N];
            float D1, D2;


            D1 = 1 / (2 * pi * Sigma * Sigma);
            D2 = 2 * Sigma * Sigma;

            float min = 1000;

            for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
            {
                for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                {
                    Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = ((1 / D1) * (float)Math.Exp(-(i * i + j * j) / D2));
                    if (Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] < min)
                        min = Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];

                }
            }
            int mult = (int)(1 / min);
            int sum = 0;
            if ((min > 0) && (min < 1))
            {

                for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
                {
                    for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                    {
                        Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (float)Math.Round(Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] * mult, 0);
                        GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (int)Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                        sum = sum + GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                    }

                }

            }
            else
            {
                sum = 0;
                for (i = -SizeofKernel / 2; i <= SizeofKernel / 2; i++)
                {
                    for (j = -SizeofKernel / 2; j <= SizeofKernel / 2; j++)
                    {
                        Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (float)Math.Round(Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j], 0);
                        GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j] = (int)Kernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                        sum = sum + GaussianKernel[SizeofKernel / 2 + i, SizeofKernel / 2 + j];
                    }

                }

            }
            //Normalizing kernel Weight
            Weight = sum;

            return;
        }

        int[,] GaussianFilter(int[,] Data)
        {
            /*unsafe static Bitmap Gfilter(Bitmap GF_img)
            {
                {
                    BitmapData bitmapData = GF_img.LockBits(new Rectangle(0, 0, GF_img.Width, GF_img.Height), ImageLockMode.ReadWrite, GF_img.PixelFormat);
                    int bytesPerPixel = Bitmap.GetPixelFormatSize(GF_img.PixelFormat) / 8;
                    int heightInPixels = bitmapData.Height;
                    int widthInBytes = bitmapData.Width * bytesPerPixel;
                    byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                    for (int y = 0; y < heightInPixels; y++)
                    {
                        byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                        for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                        {
                            byte gray = (byte)(currentLine[x] * .21 + currentLine[x + 1] * .71 + currentLine[x + 2] * .071);
                            currentLine[x] = currentLine[x + 1] = currentLine[x + 2] = gray;
                            currentLine[x + 3] = 255;

                            //??? Offsetfor 24 bits 
                        }
                    }
                    GF_img.UnlockBits(bitmapData);
                }
                return GF_img;
            }
            */

            GenerateGaussianKernel(KernelSize, Sigma, out KernelWeight);
            //Bitmap GF_img 
            //BitmapData bitmapData = GF_img.LockBits(new Rectangle(0, 0, GF_img.Width, GF_img.Height), ImageLockMode.ReadWrite, GF_img.PixelFormat);
            int[,] Output = new int[3, 3];
            int i, j, k, l;
            int Limit = KernelSize / 2;

            float Sum = 0;


            Output = Data; // Removes Unwanted Data Omission due to kernel bias while convolution


            for (i = Limit; i <= ((3 - 1) - Limit); i++)
            {
                for (j = Limit; j <= ((3 - 1) - Limit); j++)
                {
                    Sum = 0;
                    for (k = -Limit; k <= Limit; k++)
                    {

                        for (l = -Limit; l <= Limit; l++)
                        {
                            Sum = Sum + ((float)Data[i + k, j + l] * GaussianKernel[Limit + k, Limit + l]);

                        }
                    }
                    Output[i, j] = (int)(Math.Round(Sum / (float)KernelWeight));
                }

            }


            return Output;
        }
        FilteredImage = GaussianFilter(GreyImage);
        
        
        image.UnlockBits(bitmapData1);
        return image;
    }
}


  
    
        //int i, j;
        //Bitmap image = new Bitmap(Obj.Width, Obj.Height);
        //BitmapData bitmapData1 = image.LockBits(new Rectangle(0, 0, Obj.Width, Obj.Height),
        //                         ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    //    unsafe
    //    {
    //        byte* imagePointer1 = (byte*)bitmapData1.Scan0;

    //        for (i = 0; i < bitmapData1.Height; i++)
    //        {
    //            for (j = 0; j < bitmapData1.Width; j++)
    //            {
    //                // write the logic implementation here
    //                imagePointer1[0] = (byte)GreyImage[j, i];
    //                imagePointer1[1] = (byte)GreyImage[j, i];
    //                imagePointer1[2] = (byte)GreyImage[j, i];
    //                imagePointer1[3] = (byte)255;
    //                //4 bytes per pixel
    //                imagePointer1 += 4;
    //            }//end for j

    //            //4 bytes per pixel
    //            imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
    //        }//end for i
    //    }//end unsafe
    //    img.UnlockBits(bitmapData1);
    //    return img;// col;
    //}      // Display Grey Image

    
    
       
        
    
    
