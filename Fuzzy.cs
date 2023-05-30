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


class Class3
{
    public static Bitmap Fuzzy(Bitmap originalImage)
    {
        Rectangle rect = new Rectangle(0, 0, w, h);
        BitmapData image_data = originalImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

        List<ClusterPoint> points = new List<ClusterPoint>(); //??
        for (int row = 0; row < originalImage.Width; ++row)
        {
            for (int col = 0; col < originalImage.Height; ++col)
            {
                Color c2 = originalImage.GetPixel(row, col);
                points.Add(new ClusterPoint(row, col, c2));
            }
        }
        List<ClusterCentroid> centroids = new List<ClusterCentroid>();

        //Create random points to use a the cluster centroids
        Random random = new Random();
        for (int i = 0; i < 5 /*numClusters*/; i++)
        {
            int randomNumber1 = random.Next(originalImage.Width);
            int randomNumber2 = random.Next(originalImage.Height);
            centroids.Add(new ClusterCentroid(randomNumber1, randomNumber2,
                          filteredImage.GetPixel(randomNumber1, randomNumber2)));
        }
        FCM alg = new FCM(points, centroids, 2, filteredImage, (int)numericUpDown2.Value);
        k++;
        alg.J = alg.CalculateObjectiveFunction();
        alg.CalculateClusterCentroids();
        alg.Step();
        double Jnew = alg.CalculateObjectiveFunction();
        Console.WriteLine("Run method i={0} accuracy = {1} delta={2}",
        k, alg.J, Math.Abs(alg.J - Jnew));
        backgroundWorker.ReportProgress((100 * k) / maxIterations, "Iteration " + k);
        if (Math.Abs(alg.J - Jnew) < accuracy) break;
    }
}

