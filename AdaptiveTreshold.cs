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

class AdaptiveTreshold
{
    public unsafe static Bitmap VariableThresholdingLocalProperties(Bitmap image, double a, double b)
    {
            int w = image.Width;
            int h = image.Height;
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData image_data = image.LockBits(rect,ImageLockMode.ReadOnly,PixelFormat.Format24bppRgb);

            int bytes = image_data.Stride * image_data.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];

            Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            image.UnlockBits(image_data);

            
            double mg = 0;
            for (int i = 0; i < bytes; i += 3) //????
            {
                mg += buffer[i];
            }
            mg /= (w * h);

            for (int x = 1; x < w - 1; x++)
            {
                for (int y = 1; y < h - 1; y++)
                {
                    int position = x * 3 + y * image_data.Stride;
                    double[] histogram = new double[256];

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int nposition = position + i * 3 + j * image_data.Stride;
                            histogram[buffer[nposition]]++;
                        }
                    }

                    histogram = histogram.Select(l => l / (w * h)).ToArray();

                    double mean = 0;
                    for (int i = 0; i < 256; i++)
                    {
                        mean += i * histogram[i];
                    }

                    double std = 0;
                    for (int i = 0; i < 256; i++)
                    {
                        std += Math.Pow(i - mean, 2) * histogram[i];
                    }
                    std = Math.Sqrt(std);

                    double threshold = a * std + b * mg;
                    for (int c = 0; c < 3; c++)
                    {
                        result[position + c] = (byte)((buffer[position] > threshold) ? 255 : 0);
                    }
                }
            }

            Bitmap res_img = new Bitmap(w, h);
            BitmapData res_data = res_img.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(result, 0, res_data.Scan0, bytes);
            res_img.UnlockBits(res_data);

            return res_img;
        }
    }

