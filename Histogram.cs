using System;
using System.Collections.Generic;
using System.Text;

    public class Histogram
    {
        public Histogram(int binCount)
        {
            _data = new float[binCount];
        }

        public float[] Data
        {
            get { return _data; }
        }

        public void Normalise(float total)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = _data[i] / total;
            }
        }

        private float[] _data;
    }
