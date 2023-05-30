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

class NegativeImg
{
    public static Bitmap Ori2Neg(Bitmap negatve_img)
    {
        for (int y = 0; y < negatve_img.Height; y++)
        {
            for (int x = 0; x < negatve_img.Width; x++)
            {
                Color color = negatve_img.GetPixel(x, y);
                int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
                negatve_img.SetPixel(x, y, Color.FromArgb(255 - gray, 255 - gray, 255 - gray));
            }
        }
        return negatve_img;
    }
}
