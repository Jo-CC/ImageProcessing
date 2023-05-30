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


public static class CV
{
    public static Bitmap ConvolutionFilter(this Bitmap srcBitmap, double[,] filterMatrix, double factor = 1, int bias = 0)
    {
        Rectangle rect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
        BitmapData srcBmpData = srcBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        byte[] srcBytes = new byte[srcBmpData.Stride * srcBmpData.Height];
        byte[] dstBytes = new byte[srcBmpData.Stride * srcBmpData.Height];

        Marshal.Copy(srcBmpData.Scan0, srcBytes, 0, srcBytes.Length);
        srcBitmap.UnlockBits(srcBmpData);

        double blue = 0.0;
        double green = 0.0;
        double red = 0.0;
        int filterWidth = filterMatrix.GetLength(1);
        int filterHeight = filterMatrix.GetLength(0);
        int kernelRadius = (filterWidth - 1) / 2;
        int calcOffset = 0;
        int byteOffset = 0;

        for (int offsetY = kernelRadius; offsetY < srcBitmap.Height - kernelRadius; offsetY++)
        {
            for (int offsetX = kernelRadius; offsetX < srcBitmap.Width - kernelRadius; offsetX++)
            {
                blue = green = red = 0;
                byteOffset = offsetY * srcBmpData.Stride + offsetX * 4;
                for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                {
                    for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                    {
                        calcOffset = byteOffset + (kx * 4) + (ky * srcBmpData.Stride);
                        blue += (double)(srcBytes[calcOffset]) * filterMatrix[ky + kernelRadius, kx + kernelRadius];
                        green += (double)(srcBytes[calcOffset + 1]) * filterMatrix[ky + kernelRadius, kx + kernelRadius];
                        red += (double)(srcBytes[calcOffset + 2]) * filterMatrix[ky + kernelRadius, kx + kernelRadius];
                    }
                }

                blue = factor * blue + bias;
                green = factor * green + bias;
                red = factor * red + bias;

                blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                dstBytes[byteOffset] = (byte)(blue);
                dstBytes[byteOffset + 1] = (byte)(green);
                dstBytes[byteOffset + 2] = (byte)(red);
                dstBytes[byteOffset + 3] = 255;
            }
        }


        Bitmap dstBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height);
        BitmapData dstBmpData = dstBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        Marshal.Copy(dstBytes, 0, dstBmpData.Scan0, dstBytes.Length);
        dstBitmap.UnlockBits(dstBmpData);
        return dstBitmap;
    }
}

