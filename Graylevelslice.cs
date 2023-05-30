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

class BPSlicing
{
    public static int GetBit(byte b, int bitIndex)
    {
        return (b >> bitIndex) & 0x01;
    }
    public static Bitmap BPSlice(Bitmap graySlice_img, int bitplaneS)
    {
        Bitmap newSliceBp_img = new Bitmap(graySlice_img);
        for (int y = 0; y < graySlice_img.Height; y++)
        {
            for (int x = 0; x < graySlice_img.Width; x++)
            {
                //int bitplaneS = 1;
                Color color = newSliceBp_img.GetPixel(x, y);
                //int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
                int bit = GetBit(color.B, bitplaneS);
                Color colorafterS = Color.FromArgb(255 * bit, 255 * bit, 255);
                newSliceBp_img.SetPixel(x, y, colorafterS);
            }
        }
        return newSliceBp_img;
        

        //int SlicingLevel = 10;
        //switch(SlicingLevel)
        //{
        //    default: graySlice_img.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
        //        break;

        //    case 1:
        //        if (gray > 128)
        //        {
        //            gray = 255;
        //        }
        //        else 
        //        {
        //            gray = 0;
        //        }
        //        graySlice_img.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
        //        break;











    }
        //return graySlice_img;

    
   

}

