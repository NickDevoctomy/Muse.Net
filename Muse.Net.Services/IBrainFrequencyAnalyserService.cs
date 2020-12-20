﻿using Muse.Net.Model;
using System.Collections.Generic;

namespace Muse.Net.Services
{
    public interface IBrainFrequencyAnalyserService
    {
        float GetAverageOverRange(
            float[] data,
            float count,
            float maxFrequencyHz,
            float fromHz,
            float toHz);

        Dictionary<FrequencyRange, float> GetFrequencyRangeAverages(
            float[] data,
            float count,
            float maxFrequencyHz);
    }
}