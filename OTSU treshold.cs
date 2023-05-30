using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;


class OTSUtres
{
    public static Bitmap OtsuThresholding(Bitmap image)
    {
        int w = image.Width;
        int h = image.Height;
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData image_data = image.LockBits(rect,ImageLockMode.ReadOnly,PixelFormat.Format32bppRgb);

        int bytes = image_data.Stride * image_data.Height;
        byte[] buffer = new byte[bytes];
        byte[] result = new byte[bytes];

        Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
        image.UnlockBits(image_data);

        //Get histogram values
        double[] histogram = new double[256];
        for (int i = 0; i < bytes; i += 4)
        {
            histogram[buffer[i]]++;
        }

        //Normalize histogram
        histogram = histogram.Select(x => (x / (w * h))).ToArray();//???

        //Global mean
        double mg = 0;
        for (int i = 0; i < 255; i++)
        {
            mg += i * histogram[i];
        }

        //Get max between-class variance
        double bcv = 0;
        int threshold = 0;
        for (int i = 0; i < 256; i++)
        {
            double cs = 0;
            double m = 0;
            for (int j = 0; j < i; j++)
            {
                cs += histogram[j];
                m += j * histogram[j];
            }

            if (cs == 0)
            {
                continue;
            }

            double old_bcv = bcv;
            bcv = Math.Max(bcv, Math.Pow(mg * cs - m, 2) / (cs * (1 - cs)));

            if (bcv > old_bcv)
            {
                threshold = i;
            }
        }

        for (int i = 0; i < bytes; i++)
        {
            result[i] = (byte)((buffer[i] > threshold) ? 255 : 0);
        }

        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(rect,ImageLockMode.WriteOnly,PixelFormat.Format32bppRgb);
        Marshal.Copy(result, 0, res_data.Scan0, bytes);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}

