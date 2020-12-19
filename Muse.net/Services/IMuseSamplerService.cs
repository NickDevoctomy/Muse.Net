using Muse.Net.Client;
using System;

namespace Muse.Net.Services
{
    public interface IMuseSamplerService
    {
        void Sample(
            Channel channel,
            float[] values);

        bool TryGetSamples(
            Channel channel,
            TimeSpan timeSpan,
            out float[] samples);
    }
}
