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

namespace WinFormsApp1
{
    class Bpslice
    {
        //public static Bitmap SLICINGBIT(Bitmap SliceBp_img, int bis)
        //{
        //    return SliceBp_img;
        //}
        //public static Bitmap (Bitmap BPS_img, int bitplaneS)
        public static Image BPSlice(Image BPS_img, int bitplaneS)
        {
            Bitmap newSliceBp_img = new Bitmap(BPS_img);
            for (int y = 0; y < BPS_img.Height; y++)
            {
                for (int x = 0; x < BPS_img.Width; x++)
                {
                    //int bitplaneS = 1;
                    Color color = newSliceBp_img.GetPixel(x, y);
                    //int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
                    int bit = GetBit(color.R, bitplaneS);
                    Color colorafterS = Color.FromArgb(255, 255 * bit, 255 * bit);
                    newSliceBp_img.SetPixel(x, y, colorafterS);
                }
            }
            return newSliceBp_img;
        }


        public static int GetBit(byte b, int bitIndex)
        {
            return (b >> bitIndex) & 0x01;
        }


        //    //int bitplaneS = 1;
        //    Bitmap newSliceBp_img = new Bitmap(SliceBp_img.Width, SliceBp_img.Height);
        //    for (int y = 0; y<SliceBp_img.Height; y++)
        //    {
        //        for (int x = 0; x<SliceBp_img.Width; x++)
        //        {
        //            int bitplaneS = 1;
        //Color color = SliceBp_img.GetPixel(x, y);
        ////int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
        //int bit = GetBit(color.R, bitplaneS);
        //Color colorafterS = Color.FromArgb(255, 255 * bit, 255 * bit);
        //newSliceBp_img.SetPixel(x, y, colorafterS);
        //        }
    }
}
//return newSliceBp_img;
//        }

//    }
//}

