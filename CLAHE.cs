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

using System.IO;


class Clahe
{
    public static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        else if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        else if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        else if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        else if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        else
            return Color.FromArgb(255, v, p, q);
    }

    /// <summary>
    ///限制对比度自适应直方图均衡化
    /// </summary>
    public static Bitmap CLAHE(Bitmap src, int block = 8, double contrastLimit = 40)
    {
        Bitmap srcBitmap = new Bitmap(src);
        // 图像的长和宽
        int width = src.Width;
        int height = src.Height;
        // 每个小格子的长和宽
        int blockWidth = width / block;
        int blockHeight = height / block;
        // 图像分割覆盖的长和宽
        int claheWidth = blockWidth * block;
        int claheHeight = blockHeight * block;
        // 存储各个直方图  
        int[,,] PDF = new int[block, block, 256];
        double[,,] CDF = new double[block, block, 256];
        // 每块的像素数量
        int totalPixelCount = blockWidth * blockHeight;
        for (int yBlockIndex = 0; yBlockIndex < block; yBlockIndex++)
        {
            for (int xBlockIndex = 0; xBlockIndex < block; xBlockIndex++)
            {
                int start_x = xBlockIndex * blockWidth;
                int end_x = start_x + blockWidth;
                int start_y = yBlockIndex * blockHeight;
                int end_y = start_y + blockHeight;
                //遍历小块,计算直方图
                for (int y = start_y; y < end_y; y++)
                {
                    for (int x = start_x; x < end_x; x++)
                    {
                        int grayValue = (int)(srcBitmap.GetPixel(x, y).GetBrightness() * 255);
                        PDF[xBlockIndex, yBlockIndex, grayValue]++;
                    }
                }

                /* 限制对比度 */
                // 平均分布密度
                int average = totalPixelCount / 256;
                // 限制累计分布密度斜率
                int limit = (int)(contrastLimit * average);
                // 超出部分
                int bonus = 0;
                do
                {
                    int steal = 0;
                    for (int grayValue = 0; grayValue < 256; grayValue++)
                    {
                        if (PDF[xBlockIndex, yBlockIndex, grayValue] > limit)
                        {
                            steal += PDF[xBlockIndex, yBlockIndex, grayValue] - limit;
                            PDF[xBlockIndex, yBlockIndex, grayValue] = limit;
                        }
                    }
                    bonus = steal / 256;
                    int remainder = steal % 256;
                    // 平均重分布
                    for (int grayValue = 0; grayValue < 256; grayValue++)
                    {
                        PDF[xBlockIndex, yBlockIndex, grayValue] += bonus;
                    }
                    for (int grayValue = 0; grayValue < remainder; grayValue++)
                    {
                        PDF[xBlockIndex, yBlockIndex, grayValue]++;
                    }
                } while (bonus > 1);

                //计算累积分布直方图  
                for (int grayValue = 0; grayValue < 256; grayValue++)
                {
                    if (grayValue == 0)
                        CDF[xBlockIndex, yBlockIndex, grayValue] = (double)PDF[xBlockIndex, yBlockIndex, grayValue] / totalPixelCount;
                    else
                        CDF[xBlockIndex, yBlockIndex, grayValue] = CDF[xBlockIndex, yBlockIndex, grayValue - 1] + (double)PDF[xBlockIndex, yBlockIndex, grayValue] / totalPixelCount;
                }
            }
        }
        //计算变换后的像素值  
        //根据像素点的位置，选择不同的计算方法  
        for (int y = 0; y < claheHeight; y++)
        {
            for (int x = 0; x < claheWidth; x++)
            {
                int srcGrayValue = (int)(srcBitmap.GetPixel(x, y).GetBrightness() * 255);
                double srcHue = srcBitmap.GetPixel(x, y).GetHue();
                double srcSaturation = srcBitmap.GetPixel(x, y).GetSaturation();
                /* corners */
                // top left corner
                if (x <= blockWidth / 2 && y <= blockHeight / 2)
                {
                    double brightness = CDF[0, 0, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                // top right corner
                else if (x >= claheWidth - blockWidth / 2 && y <= blockHeight / 2)
                {
                    double brightness = CDF[block - 1, 0, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                // bottom left corner
                else if (x <= blockWidth / 2 && y >= claheHeight - blockHeight / 2)
                {
                    double brightness = CDF[0, block - 1, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                // bottom right corner
                else if (x >= claheWidth - blockWidth / 2 && y >= claheHeight - blockHeight / 2)
                {
                    double brightness = CDF[block - 1, block - 1, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                /* edges */
                // left edge
                else if (x <= blockWidth / 2)
                {
                    int xBlockIndex = 0;
                    int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

                    double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
                    double p = 1 - q;

                    double brightness =
                        p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
                        q * CDF[xBlockIndex, yBlockIndex + 1, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                // right edge
                else if (x >= (claheWidth - blockWidth / 2))
                {
                    int xBlockIndex = block - 1;
                    int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

                    double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
                    double p = 1 - q;

                    double brightness =
                        p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
                        q * CDF[xBlockIndex, yBlockIndex + 1, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                // top edge
                else if (y <= blockHeight / 2)
                {
                    int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
                    int yBlockIndex = 0;

                    double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
                    double p = 1 - q;

                    double brightness =
                        p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
                        q * CDF[xBlockIndex + 1, yBlockIndex, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                // bottom edge
                else if (y >= (claheHeight - blockHeight / 2))
                {
                    int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
                    int yBlockIndex = block - 1;

                    double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
                    double p = 1 - q;

                    double brightness =
                        p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
                        q * CDF[xBlockIndex + 1, yBlockIndex, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
                /* Inner */
                else
                {
                    int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
                    int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

                    double qx = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
                    double px = 1 - qx;
                    double qy = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
                    double py = 1 - qy;
                    double brightness =
                        qx * qy * CDF[xBlockIndex + 1, yBlockIndex + 1, srcGrayValue] +
                        px * py * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
                        px * qy * CDF[xBlockIndex, yBlockIndex + 1, srcGrayValue] +
                        qx * py * CDF[xBlockIndex + 1, yBlockIndex, srcGrayValue];
                    srcBitmap.SetPixel(x, y, ColorFromHSV(srcHue, srcSaturation, brightness));
                }
            }
        }
        return srcBitmap;
    }

    /// <summary>
    ///分离通道的限制对比度自适应直方图均衡化
    /// </summary>
    public static Image SeparatedChannelCLAHE(Image src, int block = 8, double contrastLimit = 40)
    {
        Bitmap srcBitmap = new Bitmap(src);
        // 图像的长和宽
        int width = src.Width;
        int height = src.Height;
        // 每个小格子的长和宽
        int blockWidth = width / block;
        int blockHeight = height / block;
        // 图像分割覆盖的长和宽
        int claheWidth = blockWidth * block;
        int claheHeight = blockHeight * block;
        // 存储各个直方图  
        int[,,,] PDF = new int[3, block, block, 256];
        double[,,,] CDF = new double[3, block, block, 256];
        // 每块的像素数量
        int totalPixelCount = blockWidth * blockHeight;
        for (int yBlockIndex = 0; yBlockIndex < block; yBlockIndex++)
        {
            for (int xBlockIndex = 0; xBlockIndex < block; xBlockIndex++)
            {
                int start_x = xBlockIndex * blockWidth;//(0,0)
                int end_x = start_x + blockWidth;//(1,0)
                int start_y = yBlockIndex * blockHeight;//(0,0)
                int end_y = start_y + blockHeight;//(0,1)
                //遍历小块,计算直方图
                for (int y = start_y; y < end_y; y++)
                {
                    for (int x = start_x; x < end_x; x++)
                    {
                        Color color = srcBitmap.GetPixel(x, y);
                        int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
                        PDF[0, xBlockIndex, yBlockIndex, gray]++;
                        PDF[1, xBlockIndex, yBlockIndex, gray]++;
                        PDF[2, xBlockIndex, yBlockIndex, gray]++;
                        //Color c = srcBitmap.GetPixel(x, y);
                        //PDF[0, xBlockIndex, yBlockIndex, c.R]++;
                        //PDF[1, xBlockIndex, yBlockIndex, c.G]++;
                        //PDF[2, xBlockIndex, yBlockIndex, c.B]++;
                        
                        
                    }
                }

                /* 限制对比度 */
                // 平均分布密度
                double average = (double)totalPixelCount / 256;
                // 限制累计分布密度斜率
                int limit = (int)(contrastLimit * average);
                // 超出部分
                int bonus = 0;
                do
                {
                    for (int channel = 0; channel < 3; channel++)
                    {
                        int steal = 0;
                        for (int value = 0; value < 256; value++)
                        {
                            if (PDF[channel, xBlockIndex, yBlockIndex, value] > limit)
                            {
                                steal += PDF[channel, xBlockIndex, yBlockIndex, value] - limit;
                                PDF[channel, xBlockIndex, yBlockIndex, value] = limit;
                            }
                        }
                        bonus = steal / 256;
                        int remainder = steal % 256;
                        // 平均重分布
                        for (int value = 0; value < 256; value++)
                        {
                            PDF[channel, xBlockIndex, yBlockIndex, value] += bonus;
                        }
                        for (int value = 0; value < remainder; value++)
                        {
                            PDF[channel, xBlockIndex, yBlockIndex, value]++;
                        }
                    }
                } while (bonus > 1);

                //计算累积分布直方图
                for (int channel = 0; channel < 3; channel++)
                {
                    for (int value = 0; value < 256; value++)
                    {
                        if (value == 0)
                            CDF[channel, xBlockIndex, yBlockIndex, value] = (double)PDF[channel, xBlockIndex, yBlockIndex, value] / totalPixelCount;
                        else
                            CDF[channel, xBlockIndex, yBlockIndex, value] = CDF[channel, xBlockIndex, yBlockIndex, value - 1] + (double)PDF[channel, xBlockIndex, yBlockIndex, value] / totalPixelCount;
                    }
                }
            }
        }
        //计算变换后的像素值  
        //根据像素点的位置，选择不同的计算方法  
        for (int y = 0; y < claheHeight; y++)
        {
            for (int x = 0; x < claheWidth; x++)
            {
                Color c = srcBitmap.GetPixel(x, y);
                /* corners */
                // top left corner
                if (x <= blockWidth / 2 && y <= blockHeight / 2)
                {
                    int R = (int)(CDF[0, 0, 0, c.R] * 255);
                    int G = (int)(CDF[1, 0, 0, c.G] * 255);
                    int B = (int)(CDF[2, 0, 0, c.B] * 255);
                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
                // top right corner
                else if (x >= claheWidth - blockWidth / 2 && y <= blockHeight / 2)
                {
                    int R = (int)(CDF[0, block - 1, 0, c.R] * 255);
                    int G = (int)(CDF[1, block - 1, 0, c.G] * 255);
                    int B = (int)(CDF[2, block - 1, 0, c.B] * 255);
                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                    //int R = (int)(CDF[0, block - 1, 0, c.R] * 255);
                    //int G = (int)(CDF[1, block - 1, 0, c.G] * 255);
                    //int B = (int)(CDF[2, block - 1, 0, c.B] * 255);
                    //srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
                }
                // bottom left corner
                else if (x <= blockWidth / 2 && y >= claheHeight - blockHeight / 2)
                {
                    int R = (int)(CDF[0, 0, block - 1, c.R] * 255);
                    int G = (int)(CDF[1, 0, block - 1, c.G] * 255);
                    int B = (int)(CDF[2, 0, block - 1, c.B] * 255);
                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                    //int R = (int)(CDF[0, 0, block - 1, c.R] * 255);
                    //int G = (int)(CDF[1, 0, block - 1, c.G] * 255);
                    //int B = (int)(CDF[2, 0, block - 1, c.B] * 255);
                    //srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
                }
                // bottom right corner
                else if (x >= claheWidth - blockWidth / 2 && y >= claheHeight - blockHeight / 2)
                {
                    int R = (int)(CDF[0, block - 1, block - 1, c.R] * 255);
                    int G = (int)(CDF[1, block - 1, block - 1, c.G] * 255);
                    int B = (int)(CDF[2, block - 1, block - 1, c.B] * 255);
                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
                /* edges */
                // left edge
                else if (x <= blockWidth / 2)
                {
                    int xBlockIndex = 0;
                    int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

                    double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
                    double p = 1 - q;

                    int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex, yBlockIndex + 1, c.R]) * 255);
                    int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex, yBlockIndex + 1, c.G]) * 255);
                    int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex, yBlockIndex + 1, c.B]) * 255);

                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
                // right edge
                else if (x >= (claheWidth - blockWidth / 2))
                {
                    int xBlockIndex = block - 1;
                    int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

                    double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
                    double p = 1 - q;

                    int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex, yBlockIndex + 1, c.R]) * 255);
                    int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex, yBlockIndex + 1, c.G]) * 255);
                    int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex, yBlockIndex + 1, c.B]) * 255);

                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
                // top edge
                else if (y <= blockHeight / 2)
                {
                    int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
                    int yBlockIndex = 0;

                    double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
                    double p = 1 - q;

                    int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex + 1, yBlockIndex, c.R]) * 255);
                    int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex + 1, yBlockIndex, c.G]) * 255);
                    int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex + 1, yBlockIndex, c.B]) * 255);

                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
                // bottom edge
                else if (y >= (claheHeight - blockHeight / 2))
                {
                    int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
                    int yBlockIndex = block - 1;

                    double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
                    double p = 1 - q;

                    int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex + 1, yBlockIndex, c.R]) * 255);
                    int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex + 1, yBlockIndex, c.G]) * 255);
                    int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex + 1, yBlockIndex, c.B]) * 255);

                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
                /* Inner */
                else
                {
                    int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
                    int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

                    double qx = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
                    double px = 1 - qx;
                    double qy = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
                    double py = 1 - qy;

                    int R = (int)((qx * qy * CDF[0, xBlockIndex + 1, yBlockIndex + 1, c.R] +
                                   px * py * CDF[0, xBlockIndex, yBlockIndex, c.R] +
                                   px * qy * CDF[0, xBlockIndex, yBlockIndex + 1, c.R] +
                                   qx * py * CDF[0, xBlockIndex + 1, yBlockIndex, c.R]) * 255);
                    int G = (int)((qx * qy * CDF[1, xBlockIndex + 1, yBlockIndex + 1, c.G] +
                                   px * py * CDF[1, xBlockIndex, yBlockIndex, c.G] +
                                   px * qy * CDF[1, xBlockIndex, yBlockIndex + 1, c.G] +
                                   qx * py * CDF[1, xBlockIndex + 1, yBlockIndex, c.G]) * 255);
                    int B = (int)((qx * qy * CDF[2, xBlockIndex + 1, yBlockIndex + 1, c.B] +
                                   px * py * CDF[2, xBlockIndex, yBlockIndex, c.B] +
                                   px * qy * CDF[2, xBlockIndex, yBlockIndex + 1, c.B] +
                                   qx * py * CDF[2, xBlockIndex + 1, yBlockIndex, c.B]) * 255);

                    int gray = (byte)((R * 0.299) + (G * 0.114) + (B * 0.587));

                    srcBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
        }
        return srcBitmap;
    }
}

    //public static Color ColorFromHSV(double hue, double saturation, double value)
    //{
    //    int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
    //    double f = hue / 60 - Math.Floor(hue / 60);

    //    value = value * 255;
    //    int v = Convert.ToInt32(value);
    //    int p = Convert.ToInt32(value * (1 - saturation));
    //    int q = Convert.ToInt32(value * (1 - f * saturation));
    //    int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

    //    if (hi == 0)
    //        return Color.FromArgb(255, v, t, p);
    //    else if (hi == 1)
    //        return Color.FromArgb(255, q, v, p);
    //    else if (hi == 2)
    //        return Color.FromArgb(255, p, v, t);
    //    else if (hi == 3)
    //        return Color.FromArgb(255, p, q, v);
    //    else if (hi == 4)
    //        return Color.FromArgb(255, t, p, v);
    //    else
    //        return Color.FromArgb(255, v, p, q);
    //}
    //public static Bitmap CLAHE(Bitmap src, int block = 8, double contrastLimit = 40)
    //{
        
        
    //    Bitmap srcBitmap = new Bitmap(src);
    //    // 图像的长和宽
    //    int width = src.Width;
    //    int height = src.Height;
    //    // 每个小格子的长和宽
    //    int blockWidth = width / block;
    //    int blockHeight = height / block;
    //    // 图像分割覆盖的长和宽
    //    int claheWidth = blockWidth * block;
    //    int claheHeight = blockHeight * block;
    //    // 存储各个直方图  
    //    int[,,] PDF = new int[block, block, 256];
    //    double[,,] CDF = new double[block, block, 256];
    //    // 每块的像素数量
    //    int totalPixelCount = blockWidth * blockHeight;
    //    for (int yBlockIndex = 0; yBlockIndex < block; yBlockIndex++)
    //    {
    //        for (int xBlockIndex = 0; xBlockIndex < block; xBlockIndex++)
    //        {
    //            int start_x = xBlockIndex * blockWidth;
    //            int end_x = start_x + blockWidth;
    //            int start_y = yBlockIndex * blockHeight;
    //            int end_y = start_y + blockHeight;
    //            //遍历小块,计算直方图
    //            for (int y = start_y; y < end_y; y++)
    //            {
    //                for (int x = start_x; x < end_x; x++)
    //                {
    //                    int grayValue = (int)(srcBitmap.GetPixel(x, y).GetBrightness() * 255);
    //                    PDF[xBlockIndex, yBlockIndex, grayValue]++;
    //                }
    //            }

    //            /* 限制对比度 */
    //            // 平均分布密度
    //            int average = totalPixelCount / 256;
    //            // 限制累计分布密度斜率
    //            int limit = (int)(contrastLimit * average);
    //            // 超出部分
    //            int bonus = 0;
    //            do
    //            {
    //                int steal = 0;
    //                for (int grayValue = 0; grayValue < 256; grayValue++)
    //                {
    //                    if (PDF[xBlockIndex, yBlockIndex, grayValue] > limit)
    //                    {
    //                        steal += PDF[xBlockIndex, yBlockIndex, grayValue] - limit;
    //                        PDF[xBlockIndex, yBlockIndex, grayValue] = limit;
    //                    }
    //                }
    //                bonus = steal / 256;
    //                int remainder = steal % 256;
    //                // 平均重分布
    //                for (int grayValue = 0; grayValue < 256; grayValue++)
    //                {
    //                    PDF[xBlockIndex, yBlockIndex, grayValue] += bonus;
    //                }
    //                for (int grayValue = 0; grayValue < remainder; grayValue++)
    //                {
    //                    PDF[xBlockIndex, yBlockIndex, grayValue]++;
    //                }
    //            } while (bonus > 1);

    //            //计算累积分布直方图  
    //            for (int grayValue = 0; grayValue < 256; grayValue++)
    //            {
    //                if (grayValue == 0)
    //                    CDF[xBlockIndex, yBlockIndex, grayValue] = (double)PDF[xBlockIndex, yBlockIndex, grayValue] / totalPixelCount;
    //                else
    //                    CDF[xBlockIndex, yBlockIndex, grayValue] = CDF[xBlockIndex, yBlockIndex, grayValue - 1] + (double)PDF[xBlockIndex, yBlockIndex, grayValue] / totalPixelCount;
    //            }
    //        }
    //    }
    //    //计算变换后的像素值  
    //    //根据像素点的位置，选择不同的计算方法  
    //    for (int y = 0; y < claheHeight; y++)
    //    {
    //        for (int x = 0; x < claheWidth; x++)
    //        {
    //            Color color = srcBitmap.GetPixel(x, y);
    //            //int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
    //            int srcGrayValue = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
    //            //b.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
    //            //int srcGrayValue = (int)(srcBitmap.GetPixel(x, y).GetBrightness() * 255);
    //            //double srcGrayValue = srcBitmap.GetPixel(x, y).GetHue();
    //            //double srcGrayValue = srcBitmap.GetPixel(x, y).GetSaturation();
    //            /* corners */
    //            // top left corner
    //            if (x <= blockWidth / 2 && y <= blockHeight / 2)
    //            {
    //                double srcGray = CDF[0, 0, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //                //src.SetPixel(x, y, Color.FromArgb(srcHue, srcSaturation, brightness));
    //            }
    //            // top right corner
    //            else if (x >= claheWidth - blockWidth / 2 && y <= blockHeight / 2)
    //            {
    //                double srcGray = CDF[block - 1, 0, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //            }
    //            // bottom left corner
    //            else if (x <= blockWidth / 2 && y >= claheHeight - blockHeight / 2)
    //            {
    //                double srcGray = CDF[0, block - 1, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //            }
    //            // bottom right corner
    //            else if (x >= claheWidth - blockWidth / 2 && y >= claheHeight - blockHeight / 2)
    //            {
    //                double srcGray = CDF[block - 1, block - 1, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //            }
    //            /* edges */
    //            // left edge
    //            else if (x <= blockWidth / 2)
    //            {
    //                int xBlockIndex = 0;
    //                int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

    //                double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
    //                double p = 1 - q;

    //                double brightness =
    //                    p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
    //                    q * CDF[xBlockIndex, yBlockIndex + 1, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //            }
    //            // right edge
    //            else if (x >= (claheWidth - blockWidth / 2))
    //            {
    //                int xBlockIndex = block - 1;
    //                int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

    //                double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
    //                double p = 1 - q;

    //                double brightness =
    //                    p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
    //                    q * CDF[xBlockIndex, yBlockIndex + 1, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //                //srcBitmap.SetPixel(x, y, ColorFromHSV(srcGrayValue, srcGrayValue, brightness));
    //            }
    //            // top edge
    //            else if (y <= blockHeight / 2)
    //            {
    //                int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
    //                int yBlockIndex = 0;

    //                double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
    //                double p = 1 - q;

    //                double brightness =
    //                    p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
    //                    q * CDF[xBlockIndex + 1, yBlockIndex, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //                //srcBitmap.SetPixel(x, y, ColorFromHSV(srcGrayValue, srcGrayValue, brightness));
    //            }
    //            // bottom edge
    //            else if (y >= (claheHeight - blockHeight / 2))
    //            {
    //                int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
    //                int yBlockIndex = block - 1;

    //                double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
    //                double p = 1 - q;

    //                double brightness =
    //                    p * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
    //                    q * CDF[xBlockIndex + 1, yBlockIndex, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //                //srcBitmap.SetPixel(x, y, ColorFromHSV(srcGrayValue, srcGrayValue, brightness));
    //            }
    //            /* Inner */
    //            else
    //            {
    //                int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
    //                int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

    //                double qx = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
    //                double px = 1 - qx;
    //                double qy = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
    //                double py = 1 - qy;
    //                double brightness =
    //                    qx * qy * CDF[xBlockIndex + 1, yBlockIndex + 1, srcGrayValue] +
    //                    px * py * CDF[xBlockIndex, yBlockIndex, srcGrayValue] +
    //                    px * qy * CDF[xBlockIndex, yBlockIndex + 1, srcGrayValue] +
    //                    qx * py * CDF[xBlockIndex + 1, yBlockIndex, srcGrayValue];
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(srcGrayValue, srcGrayValue, srcGrayValue));
    //                //srcBitmap.SetPixel(x, y, ColorFromHSV(srcGrayValue, srcGrayValue, brightness));
    //            }
    //        }
    //    }
    //    return srcBitmap;
    //}

    ///// <summary>
    /////分离通道的限制对比度自适应直方图均衡化
    ///// </summary>
    //public static Image SeparatedChannelCLAHE(Image src, int block, double contrastLimit)
    //{
    //    Bitmap srcBitmap = new Bitmap(src);
    //    // 图像的长和宽
    //    int width = src.Width;
    //    int height = src.Height;
    //    // 每个小格子的长和宽
    //    int blockWidth = width / block;
    //    int blockHeight = height / block;
    //    // 图像分割覆盖的长和宽
    //    int claheWidth = blockWidth * block;
    //    int claheHeight = blockHeight * block;
    //    // 存储各个直方图  
    //    int[,,,] PDF = new int[3, block, block, 256];
    //    double[,,,] CDF = new double[3, block, block, 256];
    //    // 每块的像素数量
    //    int totalPixelCount = blockWidth * blockHeight;
    //    for (int yBlockIndex = 0; yBlockIndex < block; yBlockIndex++)
    //    {
    //        for (int xBlockIndex = 0; xBlockIndex < block; xBlockIndex++)
    //        {
    //            int start_x = xBlockIndex * blockWidth;
    //            int end_x = start_x + blockWidth;
    //            int start_y = yBlockIndex * blockHeight;
    //            int end_y = start_y + blockHeight;
    //            //遍历小块,计算直方图
    //            for (int y = start_y; y < end_y; y++)
    //            {
    //                for (int x = start_x; x < end_x; x++)
    //                {
    //                    Color c = srcBitmap.GetPixel(x, y);
    //                    PDF[0, xBlockIndex, yBlockIndex, c.R]++;
    //                    PDF[1, xBlockIndex, yBlockIndex, c.G]++;
    //                    PDF[2, xBlockIndex, yBlockIndex, c.B]++;
    //                }
    //            }

    //            /* 限制对比度 */
    //            // 平均分布密度
    //            double average = (double)totalPixelCount / 256;
    //            // 限制累计分布密度斜率
    //            int limit = (int)(contrastLimit * average);
    //            // 超出部分
    //            int bonus = 0;
    //            do
    //            {
    //                for (int channel = 0; channel < 3; channel++)
    //                {
    //                    int steal = 0;
    //                    for (int value = 0; value < 256; value++)
    //                    {
    //                        if (PDF[channel, xBlockIndex, yBlockIndex, value] > limit)
    //                        {
    //                            steal += PDF[channel, xBlockIndex, yBlockIndex, value] - limit;
    //                            PDF[channel, xBlockIndex, yBlockIndex, value] = limit;
    //                        }
    //                    }
    //                    bonus = steal / 256;
    //                    int remainder = steal % 256;
    //                    // 平均重分布
    //                    for (int value = 0; value < 256; value++)
    //                    {
    //                        PDF[channel, xBlockIndex, yBlockIndex, value] += bonus;
    //                    }
    //                    for (int value = 0; value < remainder; value++)
    //                    {
    //                        PDF[channel, xBlockIndex, yBlockIndex, value]++;
    //                    }
    //                }
    //            } while (bonus > 1);

    //            //计算累积分布直方图
    //            for (int channel = 0; channel < 3; channel++)
    //            {
    //                for (int value = 0; value < 256; value++)
    //                {
    //                    if (value == 0)
    //                        CDF[channel, xBlockIndex, yBlockIndex, value] = (double)PDF[channel, xBlockIndex, yBlockIndex, value] / totalPixelCount;
    //                    else
    //                        CDF[channel, xBlockIndex, yBlockIndex, value] = CDF[channel, xBlockIndex, yBlockIndex, value - 1] + (double)PDF[channel, xBlockIndex, yBlockIndex, value] / totalPixelCount;
    //                }
    //            }
    //        }
    //    }
    //    //计算变换后的像素值  
    //    //根据像素点的位置，选择不同的计算方法  
    //    for (int y = 0; y < claheHeight; y++)
    //    {
    //        for (int x = 0; x < claheWidth; x++)
    //        {
    //            Color c = srcBitmap.GetPixel(x, y);
    //            /* corners */
    //            // top left corner
    //            if (x <= blockWidth / 2 && y <= blockHeight / 2)
    //            {
    //                int R = (int)(CDF[0, 0, 0, c.R] * 255);
    //                int G = (int)(CDF[1, 0, 0, c.G] * 255);
    //                int B = (int)(CDF[2, 0, 0, c.B] * 255);
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            // top right corner
    //            else if (x >= claheWidth - blockWidth / 2 && y <= blockHeight / 2)
    //            {
    //                int R = (int)(CDF[0, block - 1, 0, c.R] * 255);
    //                int G = (int)(CDF[1, block - 1, 0, c.G] * 255);
    //                int B = (int)(CDF[2, block - 1, 0, c.B] * 255);
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            // bottom left corner
    //            else if (x <= blockWidth / 2 && y >= claheHeight - blockHeight / 2)
    //            {
    //                int R = (int)(CDF[0, 0, block - 1, c.R] * 255);
    //                int G = (int)(CDF[1, 0, block - 1, c.G] * 255);
    //                int B = (int)(CDF[2, 0, block - 1, c.B] * 255);
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            // bottom right corner
    //            else if (x >= claheWidth - blockWidth / 2 && y >= claheHeight - blockHeight / 2)
    //            {
    //                int R = (int)(CDF[0, block - 1, block - 1, c.R] * 255);
    //                int G = (int)(CDF[1, block - 1, block - 1, c.G] * 255);
    //                int B = (int)(CDF[2, block - 1, block - 1, c.B] * 255);
    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            /* edges */
    //            // left edge
    //            else if (x <= blockWidth / 2)
    //            {
    //                int xBlockIndex = 0;
    //                int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

    //                double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
    //                double p = 1 - q;

    //                int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex, yBlockIndex + 1, c.R]) * 255);
    //                int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex, yBlockIndex + 1, c.G]) * 255);
    //                int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex, yBlockIndex + 1, c.B]) * 255);

    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            // right edge
    //            else if (x >= (claheWidth - blockWidth / 2))
    //            {
    //                int xBlockIndex = block - 1;
    //                int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

    //                double q = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
    //                double p = 1 - q;

    //                int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex, yBlockIndex + 1, c.R]) * 255);
    //                int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex, yBlockIndex + 1, c.G]) * 255);
    //                int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex, yBlockIndex + 1, c.B]) * 255);

    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            // top edge
    //            else if (y <= blockHeight / 2)
    //            {
    //                int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
    //                int yBlockIndex = 0;

    //                double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
    //                double p = 1 - q;

    //                int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex + 1, yBlockIndex, c.R]) * 255);
    //                int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex + 1, yBlockIndex, c.G]) * 255);
    //                int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex + 1, yBlockIndex, c.B]) * 255);

    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            // bottom edge
    //            else if (y >= (claheHeight - blockHeight / 2))
    //            {
    //                int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
    //                int yBlockIndex = block - 1;

    //                double q = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
    //                double p = 1 - q;

    //                int R = (int)((p * CDF[0, xBlockIndex, yBlockIndex, c.R] + q * CDF[0, xBlockIndex + 1, yBlockIndex, c.R]) * 255);
    //                int G = (int)((p * CDF[1, xBlockIndex, yBlockIndex, c.G] + q * CDF[1, xBlockIndex + 1, yBlockIndex, c.G]) * 255);
    //                int B = (int)((p * CDF[2, xBlockIndex, yBlockIndex, c.B] + q * CDF[2, xBlockIndex + 1, yBlockIndex, c.B]) * 255);

    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //            /* Inner */
    //            else
    //            {
    //                int xBlockIndex = (x - blockWidth / 2 - 1) / blockWidth;
    //                int yBlockIndex = (y - blockHeight / 2 - 1) / blockHeight;

    //                double qx = (x - ((double)xBlockIndex * blockWidth + blockWidth / 2)) / blockWidth;
    //                double px = 1 - qx;
    //                double qy = (y - ((double)yBlockIndex * blockHeight + blockHeight / 2)) / blockHeight;
    //                double py = 1 - qy;

    //                int R = (int)((qx * qy * CDF[0, xBlockIndex + 1, yBlockIndex + 1, c.R] +
    //                               px * py * CDF[0, xBlockIndex, yBlockIndex, c.R] +
    //                               px * qy * CDF[0, xBlockIndex, yBlockIndex + 1, c.R] +
    //                               qx * py * CDF[0, xBlockIndex + 1, yBlockIndex, c.R]) * 255);
    //                int G = (int)((qx * qy * CDF[1, xBlockIndex + 1, yBlockIndex + 1, c.G] +
    //                               px * py * CDF[1, xBlockIndex, yBlockIndex, c.G] +
    //                               px * qy * CDF[1, xBlockIndex, yBlockIndex + 1, c.G] +
    //                               qx * py * CDF[1, xBlockIndex + 1, yBlockIndex, c.G]) * 255);
    //                int B = (int)((qx * qy * CDF[2, xBlockIndex + 1, yBlockIndex + 1, c.B] +
    //                               px * py * CDF[2, xBlockIndex, yBlockIndex, c.B] +
    //                               px * qy * CDF[2, xBlockIndex, yBlockIndex + 1, c.B] +
    //                               qx * py * CDF[2, xBlockIndex + 1, yBlockIndex, c.B]) * 255);

    //                srcBitmap.SetPixel(x, y, Color.FromArgb(R, G, B));
    //            }
    //        }
    //    }
    //    return srcBitmap;
    //}

