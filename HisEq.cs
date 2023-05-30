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

class HisEq
{
        public static Bitmap HisEqual(Bitmap img)
        {
            if (img == null) return null;
            BitmapData sd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            int bytes = sd.Stride * sd.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(sd.Scan0, buffer, 0, bytes);
            img.UnlockBits(sd);
            int current = 0;
            double[] pdf = new double[256];
            double[] cdf = new double[256];
            for (int p = 0; p < bytes; p += 4)
            {
                pdf[buffer[p]]++;
            }

            for (int prob = 0; prob < pdf.Length; prob++)
            {
                pdf[prob] /= (img.Width * img.Height);
            }


            cdf[0] = pdf[0];

            for (int i = 1; i < 255; i++)
            {
                cdf[i] = cdf[i - 1] + pdf[i];

            }
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    current = y * sd.Stride + x * 4;
                    double sum = cdf[buffer[current]];

                    for (int c = 0; c < 3; c++)
                    {
                        result[current + c] = (byte)Math.Floor(255 * sum);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap res = new Bitmap(img.Width, img.Height);
            BitmapData rd = res.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, rd.Scan0, bytes);
            res.UnlockBits(rd);
            return res;

        }
    public static Bitmap colorHisEqual(Bitmap img)
    {
        if (img == null) return null;
        BitmapData sd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
        int bytes = sd.Stride * sd.Height;
        byte[] buffer = new byte[bytes];
        byte[] result = new byte[bytes];
        Marshal.Copy(sd.Scan0, buffer, 0, bytes);
        img.UnlockBits(sd);
        int current = 0;
        double[] pdf = new double[256];
        double[] cdf = new double[256];
        for (int p = 0; p < bytes; p += 4)
        {
            pdf[buffer[p]]++;
            //pdf[buffer[p+1]]++;
            //pdf[buffer[p+2]]++;
        }

        for (int prob = 0; prob < pdf.Length; prob++)
        {
            pdf[prob] /= (img.Width * img.Height);
        }


        cdf[0] = pdf[0];

        for (int i = 1; i < 255; i++)
        {
            cdf[i] = cdf[i - 1] + pdf[i];

        }
        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                current = y * sd.Stride + x * 4;
                double sum = cdf[buffer[current]];
                double sum1 = cdf[buffer[current + 1]];
                double sum2 = cdf[buffer[current + 2]];

                //for (int c = 0; c < 3; c++)
                //{
                result[current] = (byte)Math.Floor(255*sum);
                    result[current + 1] = (byte)Math.Floor(255*sum1);
                    result[current+2] = (byte)Math.Floor(255*sum2);
                //}
                result[current + 3] = 255;
            }
        }
        Bitmap res = new Bitmap(img.Width, img.Height);
        BitmapData rd = res.LockBits(new Rectangle(0, 0, img.Width, img.Height),ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        Marshal.Copy(result, 0, rd.Scan0, result.Length);
        res.UnlockBits(rd);
        return res;

    }
}
