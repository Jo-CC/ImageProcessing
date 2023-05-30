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
using System.Numerics;
class FFTs
    {
    //public static Bitmap Complex2DToBitmap(Complex[][] comp)
    //{
    //    Bitmap output = new Bitmap(comp.GetLength(0), comp.GetLength(1));

    //    // Double the width for storing real and imaginary parts on different pixels.
    //    int[,] GreyscaleImage2DArray = new int[output.Width * 2, output.Height];

    //    // Increase the step to 2, and decrease the limit by 1.
    //    for (int i = 0; i <= output.Width - 2; i += 2)
    //    {
    //        for (int j = 0; j <= output.Height - 1; j++)
    //        {
    //            // Store RealPart and ImaginaryPart on different pixels.
    //            GreyscaleImage2DArray[i, j] = (int)comp[i, j].RealPart;
    //            GreyscaleImage2DArray[i + 1, j] = (int)comp[i, j].ImaginaryPart;
    //        }
    //    }

    //    output = ImageConverter.CanConvertTo(ToComplex, Bitmap fft);

    //    return output;
    //}



    //public static Bitmap fft(Complex[][] img)
    //{
    //    Bitmap g;
    //    g = ToComplex(img);
    //    return (Bitmap)g;

    //}

    private static void ToComplex(Complex[][] img)
    {
        throw new NotImplementedException();
    }

    public static Complex[][] ToComplex(Bitmap image)
    {
       
        int w = image.Width;
        int h = image.Height;

        BitmapData input_data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        int bytes = input_data.Stride * input_data.Height;

        byte[] buffer = new byte[bytes];
        Complex[][] result = new Complex[w][];

        Marshal.Copy(input_data.Scan0, buffer, 0, bytes);
        image.UnlockBits(input_data);

        int pixel_position;

        for (int x = 0; x < w; x++)
        {
            result[x] = new Complex[h];
            for (int y = 0; y < h; y++)
            {
                pixel_position = y * input_data.Stride + x * 4;
                result[x][y] = new Complex(buffer[pixel_position], 0);
            }
        }

        return result;
    }
    public static Complex[] Forward(Complex[] input, bool phaseShift = true)
    {
        var result = new Complex[input.Length];
        var omega = (float)(-2.0 * Math.PI / input.Length);

        if (input.Length == 1)
        {
            result[0] = input[0];

            if (Complex.IsNaN(result[0]))
            {
                return new[] { new Complex(0, 0) };
            }
            return result;
        }

        var evenInput = new Complex[input.Length / 2];
        var oddInput = new Complex[input.Length / 2];
        
        for (int i = 0; i < input.Length / 2; i++)
        {
            evenInput[i] = input[2 * i];
            oddInput[i] = input[2 * i + 1];
        }

        var even = Forward(evenInput, phaseShift);
        var odd = Forward(oddInput, phaseShift);
        
        for (int k = 0; k < input.Length / 2; k++)
        {
            int phase;

            if (phaseShift)
            {
                phase = (k - input.Length / 2);
            }
            else
            {
                phase = k;
            }
            odd[k] *= Complex.FromPolarCoordinates(1, omega * phase);
        }

        for (int k = 0; k < input.Length / 2; k++)
        {
            result[k] = even[k] + odd[k];
            result[k + input.Length / 2] = even[k] - odd[k];
        }

        return result;
    }
}

