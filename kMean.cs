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


class kMeans
    {
    
    public unsafe static Bitmap KMeansClusteringSegmentation(Bitmap image, int clusters)
    {

        int w = image.Width;
        int h = image.Height;
       
        //for (i = 0; i < h; i++)
        //{
        //    for (j = 0; j < w; j++)
        //    {
        //        Color cc = image.GetPixel(i, j);
        //    }
        //}

        Rectangle rect = new Rectangle(0, 0, w, h);
        //double[] pixelArray = new double[] { ca.R, ca.G, ca.B };
        BitmapData image_data = image.LockBits(rect,ImageLockMode.ReadWrite,PixelFormat.Format32bppRgb);
        
        int bytes = image_data.Stride * image_data.Height;
        byte[] buffer = new byte[bytes];
        
        byte* kkkPtr = (byte*)image_data.Scan0;
        Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
        image.UnlockBits(image_data);

        //for (byte x = 0; x <= h; x++)

        //{
        //    byte* idk = (byte*)(kkkPtr + (x * image_data.Stride));
        //    for (byte y = 0; y <= w; y++)
        //    {
        //        byte blue =(byte)idk[y];
        //        byte green = (byte)idk[y+1];
        //        byte red = (byte)idk[y+2];
        //    }
        //}

        byte[] result = new byte[bytes];
        int[] means = new int[clusters];
        Random rnd = new Random();
        
        for (int i = 0; i < clusters; i++) // finding center
        {
            int init_mean = rnd.Next(1, 255);
            while (means.Contains((int)init_mean)) //find new center if value generated is same as previous
            {
                init_mean = rnd.Next(1, 255);
            }
            means[i] = (int)init_mean;
        }

        double error = new double();
        List<byte>[] samples = new List<byte>[clusters];

        while (true)
        {
            for (int i = 0; i < clusters; i++)
            {
                samples[i] = new List<byte>();
            }

            for (int i = 0; i < bytes; i += 4)
            {
                double caR = 999;
                int cluster = 0;

                for (int j = 0; j < clusters; j++)
                {
                    double temp = Math.Abs(buffer[i] - means[j]);
                    if (caR > temp)
                    {
                        caR = temp;
                        cluster = j;
                    }
                }
                samples[cluster].Add(buffer[i]);

               
                    result[i] = (byte)means[cluster];
                    result[i+1] = (byte)means[cluster];
                    result[i+2] = (byte)means[cluster];
                    
            }

            int[] new_means = new int[clusters];

            for (int i = 0; i < clusters; i++)
            {
                for (int j = 0; j < samples[i].Count(); j++)
                {
                    new_means[i] += samples[i][j];
                }

                new_means[i] /= (samples[i].Count() + 1);
            }

            int new_error = 0;

            for (int i = 0; i < clusters; i++)
            {
                new_error += Math.Abs(means[i] - new_means[i]);
                means[i] = new_means[i];
            }

            if (error == new_error)
            {
                break;
            }
            else
            {
                error = new_error;
            }
        }

        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(rect,ImageLockMode.WriteOnly,PixelFormat.Format32bppRgb);

        Marshal.Copy(result, 0, res_data.Scan0, result.Length);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}

