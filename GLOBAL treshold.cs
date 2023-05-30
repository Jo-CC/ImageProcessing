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

class GlobalTRES
{
    public static Bitmap GlobalThresholding(Bitmap image)
    {
        int w = image.Width;
        int h = image.Height;
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData image_data = image.LockBits(rect, ImageLockMode.ReadOnly,PixelFormat.Format32bppRgb);

        int bytes = image_data.Stride * image_data.Height;
        byte[] buffer = new byte[bytes];
        byte[] result = new byte[bytes];

        //byte* ptrFirstPixel = (byte*)image_data.Scan0;

        Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
        image.UnlockBits(image_data);

        //Getting threshold intensity value
        int[] converted = buffer.Select(x => (int)x).ToArray();//??
        int init = converted.Sum() / bytes; //??
        int delta = 1;

        while (delta > 0)
        {
            int[] histogram = new int[256];
            for (int i = 0; i < bytes; i += 4)
            {
                histogram[buffer[i]]++;
            }

            int mean1 = 0;
            int mean2 = 0;
            int sum1 = 0;
            int sum2 = 0;

            for (int i = 0; i < 255; i++)
            {
                if (i <= init)
                {
                    mean1 += histogram[i] * i;
                    sum1 += histogram[i];
                    Console.WriteLine(mean1);
                    Console.WriteLine(sum1);
                }
                else
                {
                    mean2 += histogram[i] * i;
                    sum2 += histogram[i];
                    Console.WriteLine(mean2);
                    Console.WriteLine(sum2);
                }
            }

            mean1 /= sum1;
            mean2 /= sum2;
            delta = init;
            init = (mean1 + mean2) / 2;
            delta = Math.Abs(delta - init);
        }

        //Thresholding
        for (int i = 0; i < bytes; i++)
        {
            result[i] = (byte)(buffer[i] >= init ? 255 : 0);
        }

        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(rect, ImageLockMode.WriteOnly,PixelFormat.Format32bppRgb);
        Marshal.Copy(result, 0, res_data.Scan0, bytes);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}

