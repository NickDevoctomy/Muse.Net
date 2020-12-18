﻿using Harthoorn.MuseClient;
using System;

namespace Muse.Net.Services
{
    public interface IFFTSamplerService
    {
        bool TryGetFFTSample(
            IMuseSamplerService museSamplerService,
            Channel channel,
            TimeSpan timeSpan,
            out float[] samples);
    }
}