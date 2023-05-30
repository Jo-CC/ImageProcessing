using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


public static class Erosion
{
    public static Bitmap imgErosion(this Bitmap image, int kernelSize)
    {
        int w = image.Width;
        int h = image.Height;

        BitmapData image_data = image.LockBits(new Rectangle(0, 0, w, h),ImageLockMode.ReadOnly,PixelFormat.Format24bppRgb);

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
                byte val = 255;
                for (int x = -kernelOffset; x <= kernelOffset; x++)
                {
                    for (int y = -kernelOffset; y <= kernelOffset; y++)
                    {
                        int kposition = position + x * 3 + y * image_data.Stride;
                        val = Math.Min(val, buffer[kposition]);
                    }
                }
                for (int c = 0; c < 3; c++)
                {
                    result[position + c] = val;
                }
            }
        }

        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(
            new Rectangle(0, 0, w, h),
            ImageLockMode.WriteOnly,
            PixelFormat.Format24bppRgb);
        Marshal.Copy(result, 0, res_data.Scan0, bytes);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}

