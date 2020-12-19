using System;
using System.Numerics;

namespace Muse.Net.Services
{
    public interface IFourierService
    {
        float[] Magnitudes(Complex[] array);
        Complex[] DFT(ReadOnlySpan<float> data);
    }
}