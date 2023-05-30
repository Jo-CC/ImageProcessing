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


class FastBOXBlur
{
//    public static Bitmap fastBoxBlur(Bitmap img, int kradius)
//    {
//        if (kradius % 2 == 0)
//            kradius++;
//        Bitmap boxblur = (Bitmap)img.Clone();
//        float Avg = (float)1 / kradius;

//        for (int j = 0; j <= img.Height; j++)
//        {

//            float[] hSum = new float[] { 0, 0, 0, 0 };
//            float[] iAvg = new float[] { 0, 0, 0, 0 };
//            for (int x = 0; x <= kradius; x++)
//            {
//                Color imgcolor = img.GetPixel(x, j);
//                hSum[0] += imgcolor.A;
//                hSum[1] += imgcolor.R;
//                hSum[2] += imgcolor.G;
//                hSum[3] += imgcolor.B;

//                iAvg[0] = hSum[0] * Avg;
//                iAvg[1] = hSum[1] * Avg;
//                iAvg[2] = hSum[2] * Avg;
//                iAvg[3] = hSum[3] * Avg;

//                for (int i = 0; i < img.Width; i++)
//                {
//                    if (i - kradius / 2 >= 0 && i + 1 + kradius / 2 < img.Width)
//                    {
//                        Color tmp_pColor = img.GetPixel(i - kradius / 2, j);
//                        hSum[0] -= tmp_pColor.A;
//                        hSum[1] -= tmp_pColor.R;
//                        hSum[2] -= tmp_pColor.G;
//                        hSum[3] -= tmp_pColor.B;
//                        Color tmp_nColor = img.GetPixel(i + 1 + kradius / 2, j);
//                        hSum[0] += tmp_nColor.A;
//                        hSum[1] += tmp_nColor.R;
//                        hSum[2] += tmp_nColor.G;
//                        hSum[3] += tmp_nColor.B;
//                        //
//                        iAvg[0] = hSum[0] * Avg;
//                        iAvg[1] = hSum[1] * Avg;
//                        iAvg[2] = hSum[2] * Avg;
//                        iAvg[3] = hSum[3] * Avg;
//                    }
//                    boxblur.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
//                }
//            }
   
//}
//        Bitmap total = (Bitmap)boxblur.Clone();
//        for (int i = 0; i < boxblur.Width; i++)
//        {
//            float[] tSum = new float[] { 0f, 0f, 0f, 0f };
//            float[] imAvg = new float[] { 0f, 0f, 0f, 0f };
//            for (int y = 0; y < kradius; y++)
//            {
//                Color tmpColor = boxblur.GetPixel(i, y);
//                tSum[0] += tmpColor.A;
//                tSum[1] += tmpColor.R;
//                tSum[2] += tmpColor.G;
//                tSum[3] += tmpColor.B;
//            }
//            imAvg[0] = tSum[0] * Avg;
//            imAvg[1] = tSum[1] * Avg;
//            imAvg[2] = tSum[2] * Avg;
//            imAvg[3] = tSum[3] * Avg;

//            for (int y = 0; y < boxblur.Height; y++)

//                if (y - kradius / 2 >= 0 && y + 1 + kradius / 2 < boxblur.Height)

//                {
//                    Color tmp_pColor = boxblur.GetPixel(i, y - kradius / 2);
//                    tSum[0] -= tmp_pColor.A;
//                    tSum[1] -= tmp_pColor.R;
//                    tSum[2] -= tmp_pColor.G;
//                    tSum[3] -= tmp_pColor.B;
//                    Color tmp_nColor = boxblur.GetPixel(i, y + 1 + kradius / 2);
//                    tSum[0] += tmp_nColor.A;
//                    tSum[1] += tmp_nColor.R;
//                    tSum[2] += tmp_nColor.G;
//                    tSum[3] += tmp_nColor.B;
//                    //
//                    iAvg[0] = tSum[0] * Avg;
//                    iAvg[1] = tSum[1] * Avg;
//                    iAvg[2] = tSum[2] * Avg;
//                    iAvg[3] = tSum[3] * Avg;
//                }
//            total.SetPixel(i, y, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));


//        }




//    }
//        return total;
    




//    for (int x = 0; x < img.Width; x++)
//    { 
//        if (x – kradius / 2 >= 0 && x + 1 + kradius / 2 < img.Width)
//            {

//            boxblur.SetPixel(y, x, Color.FromArgb((int)imageAvg[0], (int)imageAvg[1], (int)imageAvg[2], (int)imageAvg[3]));

//        }
//        return boxblur;
//    }
//}


public static Bitmap FastBoxBlur(Bitmap img, int radius)
{
    int kSize = radius;
    if (kSize % 2 == 0) kSize++;
    Bitmap Hblur = (Bitmap)img.Clone();
    float Avg = (float)1 / kSize;

    for (int j = 0; j < img.Height; j++)
    {

        float[] hSum = new float[] { 0f, 0f, 0f, 0f };
        float[] iAvg = new float[] { 0f, 0f, 0f, 0f };

        for (int x = 0; x < kSize; x++)
        {
            Color tmpColor = img.GetPixel(x, j);
            hSum[0] += tmpColor.A;
            hSum[1] += tmpColor.R;
            hSum[2] += tmpColor.G;
            hSum[3] += tmpColor.B;
        }
        iAvg[0] = hSum[0] * Avg;
        iAvg[1] = hSum[1] * Avg;
        iAvg[2] = hSum[2] * Avg;
        iAvg[3] = hSum[3] * Avg;
        for (int i = 0; i < img.Width; i++) { if (i - kSize / 2 >= 0 && i + 1 + kSize / 2 < img.Width)
{
    Color tmp_pColor = img.GetPixel(i - kSize / 2, j);
    hSum[0] -= tmp_pColor.A;
    hSum[1] -= tmp_pColor.R;
    hSum[2] -= tmp_pColor.G;
    hSum[3] -= tmp_pColor.B;
    Color tmp_nColor = img.GetPixel(i + 1 + kSize / 2, j);
    hSum[0] += tmp_nColor.A;
    hSum[1] += tmp_nColor.R;
    hSum[2] += tmp_nColor.G;
    hSum[3] += tmp_nColor.B;
    //
    iAvg[0] = hSum[0] * Avg;
    iAvg[1] = hSum[1] * Avg;
    iAvg[2] = hSum[2] * Avg;
    iAvg[3] = hSum[3] * Avg;
}
Hblur.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
}
}
Bitmap total = (Bitmap)Hblur.Clone();
for (int i = 0; i < Hblur.Width; i++)
{
    float[] tSum = new float[] { 0f, 0f, 0f, 0f };
    float[] iAvg = new float[] { 0f, 0f, 0f, 0f };
    for (int y = 0; y < kSize; y++)
    {
        Color tmpColor = Hblur.GetPixel(i, y);
        tSum[0] += tmpColor.A;
        tSum[1] += tmpColor.R;
        tSum[2] += tmpColor.G;
        tSum[3] += tmpColor.B;
    }
    iAvg[0] = tSum[0] * Avg;
    iAvg[1] = tSum[1] * Avg;
    iAvg[2] = tSum[2] * Avg;
    iAvg[3] = tSum[3] * Avg;

    for (int j = 0; j < Hblur.Height; j++) { if (j - kSize / 2 >= 0 && j + 1 + kSize / 2 < Hblur.Height)
{
    Color tmp_pColor = Hblur.GetPixel(i, j - kSize / 2);
    tSum[0] -= tmp_pColor.A;
    tSum[1] -= tmp_pColor.R;
    tSum[2] -= tmp_pColor.G;
    tSum[3] -= tmp_pColor.B;
    Color tmp_nColor = Hblur.GetPixel(i, j + 1 + kSize / 2);
    tSum[0] += tmp_nColor.A;
    tSum[1] += tmp_nColor.R;
    tSum[2] += tmp_nColor.G;
    tSum[3] += tmp_nColor.B;
    //
    iAvg[0] = tSum[0] * Avg;
    iAvg[1] = tSum[1] * Avg;
    iAvg[2] = tSum[2] * Avg;
    iAvg[3] = tSum[3] * Avg;
}

total.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
}
}
return total;
}
    public static Bitmap Gaussain(Bitmap src, int Radius)
    {
        return FastGaussianBlur(src, Radius);

        Bitmap FastGaussianBlur(Bitmap src, int Raduis)
        {
            var bxs = boxesForGaussian(Raduis, 3);
            Bitmap img = FastBoxBlur(src, bxs[0]);
            Bitmap img_2 = FastBoxBlur(img, bxs[1]);
            Bitmap img_3 = FastBoxBlur(img_2, bxs[2]);
            return img_3;
            int[] boxesForGaussian(double sigma, int n)
            {
                double wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);

                double wl = Math.Floor(wIdeal); if (wl % 2 == 0) wl--;
                double wu = wl + 2;

                double mIdeal = (12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
                double m = Math.Round(mIdeal);

                int[] sizes = new int[n];
                for (int i = 0; i < n; i++)
                {
                    if (i < m) { sizes[i] = (int)wl; } else { sizes[i] = (int)wu; }
                }
                return sizes;
            }
        }
    }
}



