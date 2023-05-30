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

    class GraySlice
    {
        public unsafe static Bitmap GSlice(Bitmap graySlice_img)
        {
        BitmapData bitmapData = graySlice_img.LockBits(new Rectangle(0, 0, graySlice_img.Width, graySlice_img.Height), ImageLockMode.ReadWrite, graySlice_img.PixelFormat);
        int bytesPerPixel = Bitmap.GetPixelFormatSize(graySlice_img.PixelFormat) / 8;
        int heightInPixels = bitmapData.Height;
        int widthInBytes = bitmapData.Width * bytesPerPixel;
        byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

        for (int y = 0; y < heightInPixels; y++)
            {
            byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
            for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                byte gray = (byte)(currentLine[x] * .587 + currentLine[x + 1] * .114 + currentLine[x + 2] * .299);
                //currentLine[x] = currentLine[x + 1] = currentLine[x + 2] = gray;
                currentLine[x + 3] = 255;

                if (gray >= 128)
                {
                    currentLine[x] = currentLine[x + 1] = currentLine[x + 2] = 255;
                    

                }
                else
                {

                    currentLine[x] = currentLine[x + 1] = currentLine[x + 2] = 0;
                    
                }
                //graySlice_img.SetPixel(y, x, Color.FromArgb((int)gray, (int)gray, (int)gray, (int)gray));
            }
            }
        graySlice_img.UnlockBits(bitmapData);
        return graySlice_img;
        }
    }

