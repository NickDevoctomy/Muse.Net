using System;
using System.Numerics;

namespace Muse.Net.Services
{
    public class FourierService : IFourierService
    {
        public Complex[] DFT(float[] data)
        {
            double pi2oN = -2.0 * Math.PI / data.Length;
            Complex[] frequencies = new Complex[data.Length];

            for (int k = 0; k < data.Length; k++)
            {
                (double re, double im) = (0, 0);

                for (int n = 0; n < data.Length; n++)
                {
                    re += data[n] * Math.Cos(pi2oN * k * n);
                    im += data[n] * Math.Sin(pi2oN * k * n);
                }

                var c = new Complex(re, im);
                frequencies[k] = c;
            }

            return frequencies;
        }

        public float[] Magnitudes(Complex[] array)
        {
            float[] result = new float[array.Length];
            for (int i = 0; i < array.Length; i++) result[i] = (float)array[i].Magnitude;
            return result;
        }
    }
}