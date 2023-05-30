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



class SLIC
    {
    public static Bitmap Superpixels(Bitmap image, int K)
    {
        int w = image.Width;
        int h = image.Height;
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData image_data = image.LockBits(rect,ImageLockMode.ReadOnly,PixelFormat.Format32bppRgb);

        int bytes = image_data.Stride * image_data.Height;
        byte[] buffer = new byte[bytes];

        Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
        image.UnlockBits(image_data);

        int N = buffer.Length / 8;
        int s = (int)Math.Floor(Math.Sqrt((double)N / K));

        int[][] means = new int[K][];
        byte[] result = new byte[bytes];
        int sp = 0;

        //compute initial superpixel cluster centers
        for (int x = s / 2; x < w; x += s)
        {
            for (int y = s / 2; y < h; y += s)
            {
                int position = x * 4 + y * image_data.Stride;

                //compute lowest gradient
                double lowest_grad = 9999;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int n_pos = position + i * 4 + j * image_data.Stride;
                        double grad = 0;
                        for (int k = -1; k <= 1; k++)
                        {
                            for (int l = -1; l <= 1; l++)
                            {
                                int g_pos = n_pos + k * 4 + l * image_data.Stride;
                                //grad += buffer[g_pos] * Matrix.Laplacian();
                                grad += buffer[g_pos] * (Matrix.Sorbelx[k + 1, l + 1])+(Matrix.Sorbely[k + 1, l + 1]);
                            }
                        }
                        if (lowest_grad > grad)
                        {
                            lowest_grad = grad;
                            means[sp] = new int[] { buffer[n_pos], x + i, y + j };
                        }
                    }
                }

                for (int c = 0; c < 4; c++)
                {
                    result[means[sp][1] * 4 + means[sp][2] * image_data.Stride + c] = 255;
                }
                if (sp < K - 1)
                {
                    sp++;
                }
            }
        }

        int[] labels = new int[bytes];
        double[] distances = new double[bytes];
        for (int i = 0; i < bytes; i += 4)
        {
            labels[i] = -1;
            distances[i] = buffer.Length;
        }

        double error = new double();
        double m = 0.5;
        while (true)
        {
            int[][] new_means = new int[K][];

            //assign samples to clusters
            for (int i = 0; i < K; i++)
            {
                int m_pos = means[i][1] * 4 + means[i][2] * image_data.Stride;
                int xe = 2 * s + means[i][1];
                int xs = means[i][1] - 2 * s;
                int ye = 2 * s + means[i][2];
                int ys = means[i][2] - 2 * s;

                for (int x = (xs < 0 ? 0 : xs); x < ((xe < w) ? xe : w); x++)
                {
                    for (int y = (ys < 0 ? 0 : ys); y < (ye < h ? ye : h); y++)
                    {
                        int position = x * 4 + y * image_data.Stride;
                        double ds = Math.Sqrt(Math.Pow(x - means[i][1], 2) + Math.Pow(y - means[i][2], 2));
                        double dc = Math.Sqrt(Math.Pow(buffer[position] - means[i][0], 2)
                            + Math.Pow(buffer[position + 1] - means[i][0], 2)
                            + Math.Pow(buffer[position + 2] - means[i][0], 2));
                        double distance = dc + (m / s) * ds;
                        if (distance < distances[position])
                        {
                            distances[position] = distance;
                            labels[position] = i;
                        }
                    }
                }
            }

            //compute new means
            for (int i = 0; i < K; i++)
            {
                new_means[i] = new int[4];
                int samples = 1;
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        int position = x * 4 + y * image_data.Stride;
                        if (labels[position] == i)
                        {
                            new_means[i][0] += buffer[position];
                            new_means[i][1] += x;
                            new_means[i][2] += y;
                            samples++;
                        }
                    }
                }

                for (int j = 0; j < 4; j++)
                {
                    new_means[i][j] /= samples;
                }
            }

            //compute error
            double new_error = 0;
            for (int i = 0; i < K; i++)
            {
                new_error += (int)Math.Sqrt(Math.Pow(means[i][0] - new_means[i][0], 2)
                    + Math.Pow(means[i][1] - new_means[i][1], 2)
                    + Math.Pow(means[i][2] - new_means[i][2], 2));
                means[i] = new_means[i];
            }

            if (error < new_error)
            {
                break;
            }
            else
            {
                error = new_error;
            }
        }

        for (int i = 0; i < K; i++)
        {
            for (int j = 0; j < bytes; j += 4)
            {
                if (labels[j] == i)
                {
                    
                        result[j] = (byte)means[i][0];
                        result[j+1] = (byte)means[i][1];
                        result[j+2] = (byte)means[i][2];
                        //result[j + 3] = (byte)means[i][3];
                    byte gray = (byte)(result[j] * 0.21 + result[j + 1] * 0.71 + result[j + 2] * 0.071);
                    result[j] = result[j + 1] = result[j + 2] = gray;
                    result[j + 3] = 255;
                }
            }
        }

        Bitmap res_img = new Bitmap(w, h);
        BitmapData res_data = res_img.LockBits(rect,ImageLockMode.WriteOnly,PixelFormat.Format32bppRgb);
        Marshal.Copy(result, 0, res_data.Scan0, result.Length);
        res_img.UnlockBits(res_data);

        return res_img;
    }
}

