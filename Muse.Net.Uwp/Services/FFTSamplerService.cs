using Harthoorn.MuseClient;
using System;
using System.Linq;

namespace Muse.Net.Services
{
    public class FFTSamplerService : IFFTSamplerService
    {
        private const int SAMPLESIZE = 300;

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
                out var data))
            {
                var len = data.Length;
                var d = data.Skip(len - SAMPLESIZE).Take(SAMPLESIZE).ToArray();
                samples = Fourier.DFT(d).Magnitudes();
                return true;
            }

            return false;
        }
    }
}
