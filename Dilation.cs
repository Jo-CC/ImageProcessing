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

public static class Dilation
{
    public static Bitmap imgDilation(this Bitmap image, int kernelSize)
    {
        int w = image.Width;
        int h = image.Height;
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData image_data = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

        int bytes = image_data.Stride * image_data.Height;
        byte[] buffer = new byte[bytes];
        byte[] result = new byte[bytes];

        Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
        image.UnlockBits(image_data);
        int kernelOffset = (kernelSize - 1) / 2;

        for (int i = kernelOffset; i < w - kernelOffset; i++)
        {
            for (int j = kernelOffset; j < h - kernelOffset; j++)
            {
                int position = i * 3 + j * image_data.Stride;
                for (int k = -kernelOffset; k <= kernelOffset; k++)
                {
                    for (int l = -kernelOffset; l <= kernelOffset; l++)
                    {
                        int se_pos = position + k * 3 + l * image_data.Stride;
                        for (int c = 0; c < 3; c++)
                        {
                            result[se_pos + c] = Math.Max(result[se_pos + c], buffer[position]);
                        }
                    }
                }
            }
        }

        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(rect,ImageLockMode.WriteOnly,PixelFormat.Format24bppRgb);
        Marshal.Copy(result, 0, res_data.Scan0, bytes);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}

