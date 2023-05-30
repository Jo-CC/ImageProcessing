using System;
using System.Drawing;
using System.Collections.Specialized;


    public unsafe class Processor
    {
        public Processor()
        {
        }
             
        public Histogram Create1DHistogram(Bitmap bmp, int numBinsCh1, int numBinsCh2, int numBinsCh3, int c)
        {
            Histogram hist = new Histogram(numBinsCh1 * numBinsCh2 * numBinsCh3);
            float total = 0;
            int idx = 0;

            UnsafeBitmap fastBitmap = new UnsafeBitmap(bmp);
            fastBitmap.LockBitmap();
            Point size = fastBitmap.Size;
            BGRA* pPixel;

            for (int y = 0; y < size.Y; y++)
            {
                pPixel = fastBitmap[0, y];
                for (int x = 0; x < size.X; x++)
                {
                    //get the bin index for the current pixel colour
                    idx = GetSingleBinIndex(numBinsCh1, numBinsCh2, numBinsCh3, pPixel, c);
                    hist.Data[idx] += 1;
                    total += 1;

                    //increment the pointer
                    pPixel++;
                }
            }

            fastBitmap.UnlockBitmap();

            //normalise
            if (total > 0)
                hist.Normalise(total);

            return hist;
        }

        private int GetSingleBinIndex(int binCount1, int binCount2, int binCount3, BGRA* pixel, int C)
        {
            int idx = 0;
            
            //find the index                
            int i1 = GetBinIndex(binCount1, (float)pixel->red, 255);
            int i2 = GetBinIndex(binCount2, (float)pixel->green, 255);
            int i3 = GetBinIndex(binCount3, (float)pixel->blue, 255);
        //idx = i1 + i2 * binCount1 + i3 * binCount1 * binCount2;
            if (C==1)
                { idx = i1; }
            else if (C==2)
                { idx = i2; }
            else if (C==3)
                { idx = i3; }
            else
                { idx = i1 + i2 * binCount1 + i3 * binCount1 * binCount2; }
            return idx;
        }

        private int GetBinIndex(int binCount, float colourValue, float maxValue)
        {
            int idx = (int)(colourValue * (float)binCount / maxValue);
            if (idx >= binCount)
                idx = binCount - 1;

            return idx;
        }

    }

