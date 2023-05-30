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

class km
{
    public unsafe static Bitmap KMeansss(Bitmap image, int clusters)
    {
        int w = image.Width;
        int h = image.Height;
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData image_data = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
        byte[] dstBytes = new byte[image_data.Stride * image_data.Height];
        int bytes = image_data.Stride * image_data.Height;
        byte[] buffer = new byte[bytes];
        byte* kkkPtr = (byte*)image_data.Scan0;
        Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
        image.UnlockBits(image_data);

        for (byte x = 0; x <= h; x++)

        {
            byte* idk = (byte*)(kkkPtr + (x * image_data.Stride));
            for (byte y = 0; y <= w; y++)
            {
                byte blue = (byte)idk[y];
                byte green = (byte)idk[y + 1];
                byte red = (byte)idk[y + 2];

                dstBytes[bytes] = (byte)blue;
                byte[] result = new byte[bytes];
                int[] means = new int[clusters];
                Random rnd = new Random();

                for (int i = 0; i < clusters; i++)
                {
                    int init_mean = rnd.Next(1, 255);
                    while (means.Contains((byte)init_mean))
                    {
                        init_mean = rnd.Next(1, 255);
                    }
                    means[i] = (byte)init_mean;
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
                        double norm = 999;
                        int cluster = 0;

                        for (int j = 0; j < clusters; j++)
                        {
                            double temp = Math.Abs(buffer[i] - means[j]);
                            if (norm > temp)
                            {
                                norm = temp;
                                cluster = j;
                            }
                        }
                        samples[cluster].Add(buffer[i]);

                        for (int c = 0; c < 4; c++)
                        {
                            result[i + c] = (byte)means[cluster];
                        }
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
            }

        }

        

        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

        Marshal.Copy(res_data, 0, res_data.Scan0, bytes);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}
        
    


