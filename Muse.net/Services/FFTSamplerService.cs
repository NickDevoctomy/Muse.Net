using Muse.Net.Client;
using System;
using System.Linq;

namespace Muse.Net.Services
{
    public class FFTSamplerService : IFFTSamplerService
    {
        private IFourierService _fourierService = new FourierService();
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
                var samplesDft = _fourierService.DFT(d);
                samples = _fourierService.Magnitudes(samplesDft);
                return true;
            }

            return false;
        }
    }
}
