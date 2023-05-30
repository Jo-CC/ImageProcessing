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

class Fastcolor2mono
{
    public Fastcolor2mono()
    {
    }

    public float FloatClip(float value, float min = 0, float max = 100)
    {
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        else
        {
            return value;
        }
    }
    public static byte ByteClip(float value, byte min = 0, byte max = 255)
    {
        if (value < min)
        {
            return (byte)min;
        }
        else if (value > max)
        {
            return (byte)max;
        }
        else
        {
            return (byte)value;
        }
    }


    public static unsafe Bitmap Grayscale_Unsafe (Bitmap processedBitmap)
    {
        
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

            for (int y = 0; y < heightInPixels; y++)
            {
                byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    byte gray = (byte)(currentLine[x] * .21 + currentLine[x + 1] * .71 + currentLine[x + 2] * .071);
                    currentLine[x] = currentLine[x + 1] = currentLine[x + 2] = gray;
                    currentLine[x + 3] = 255;

                    //??? Offsetfor 24 bits 
                }
            }
            processedBitmap.UnlockBits(bitmapData);
        }
        return processedBitmap;
    }
}




//public static Bitmap fastcolour(Bitmap srcImage)
//{
//    //for (int y = 0; y < b.Height; y++)
//    //{
//    //	for (int x = 0; x < b.Width; x++)
//    //	{
//    //Color color = b.GetPixel(x, y);
//    //int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
//    //b.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
//    Rectangle rect = new Rectangle(0, 0, srcImage.Width, srcImage.Height);

//    BitmapData srcBmpData = srcImage.LockBits(new Rectangle(0, 0, srcImage.Width, srcImage.Height),
//    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);  
//    IntPtr srcPtr = srcBmpData.Scan0;


//    //Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
//    //b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
//    //BitmapData bmpData = b.LockBits(rect, ImageLockMode.ReadWrite, b.PixelFormat);
//    //ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
//    //IntPtr ptr = bmpData.Scan0;

//    int srcBytes = Math.Abs(srcBmpData.Stride) * srcImage.Height;
//    byte[] srcRGBValues = new byte[srcBytes];

//    Marshal.Copy(srcPtr, srcRGBValues, 0, srcBytes);

//    for (int i = 0; i < srcRGBValues.Length; i += 4)
//    {
//        //byte gray = 0;
//        byte gray = (byte)(srcRGBValues[i] * .21 + srcRGBValues[i + 1] * .71 + srcRGBValues[i + 2] * .071);
//        //gray = fGray < 0 ? (byte)0 : fGray > 255 ? (byte)255 : (byte) fGray;
//        //gray = ByteClip(fGray);
//        srcRGBValues[i] = srcRGBValues[i + 1] = srcRGBValues[i + 2] = gray;
//        srcRGBValues[i + 3] = 255;
//    }


//    Marshal.Copy(srcRGBValues, 0, srcPtr, srcBytes);


//    srcImage.UnlockBits(srcBmpData);


//    return srcImage;
//}
class imageAqui
{
    public static Bitmap getPixel(Bitmap srcImage)
    {
        for (int y = 0; y < srcImage.Height; y++)
        {
            for (int x = 0; x < srcImage.Width; x++)
            {
                Color color = srcImage.GetPixel(x, y);
                int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
                srcImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));

            }
        }
        return srcImage;
    }
    public static Bitmap marshallCopy(Bitmap b)
    {
        int w = b.Width;
        int h = b.Height;
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData srcBmpData = b.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);



        int srcBytes = Math.Abs(srcBmpData.Stride) * srcBmpData.Height;
        byte[] srcRGBValues = new byte[srcBytes];

        Marshal.Copy(srcBmpData.Scan0, srcRGBValues, 0, srcBytes);

        for (int i = 0; i < srcRGBValues.Length; i += 4)
        {
            byte gray = 0;
            gray = (byte)(srcRGBValues[i] * .21 + srcRGBValues[i + 1] * .71 + srcRGBValues[i + 2] * .071);
            srcRGBValues[i] = srcRGBValues[i + 1] = srcRGBValues[i + 2] = gray;
            srcRGBValues[i + 3] = 255;
        }


        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
        Marshal.Copy(srcRGBValues, 0, res_data.Scan0, srcBytes);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}
