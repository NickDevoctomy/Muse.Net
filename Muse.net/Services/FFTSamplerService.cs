using Harthoorn.MuseClient;
using System;
using System.Linq;

namespace Muse.Net.Services
{
    public class FFTSamplerService : IFFTSamplerService
    {
        public bool TryGetFFTSample(
            IMuseSamplerService museSamplerService,
            Channel channel,
            TimeSpan timeSpan,
            out float[] samples)
        {
            samples = null;
            if (museSamplerService.TryGetSamples(
                channel,
                timeSpan,
                out var dataTP9))
            {
                var len = dataTP9.Length;
                const int SIZE = 300;
                var d = dataTP9.Skip(len - SIZE).Take(SIZE).ToArray();
                samples = Fourier.DFT(d).Magnitudes();
                return true;
            }

            return false;
        }
    }
}
