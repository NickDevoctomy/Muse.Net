using System;
using System.Collections.Generic;
using System.Linq;

namespace Muse.Net.Services
{
    public class BrainFrequencyAnalyser : IBrainFrequencyAnalyser
    {
        public float GetAverageOverRange(
            float[] data,
            float count,
            float maxFrequencyHz,
            float fromHz,
            float toHz)
        {
            double freqPerIndex = (double)maxFrequencyHz / (double)count;
            int fromIndex = (int)Math.Floor((double)fromHz / freqPerIndex);
            if (fromIndex == 0)
            {
                fromIndex = 1;
            }
            int toIndex = (int)Math.Ceiling((double)toHz / freqPerIndex);
            var destArray = new float[toIndex - fromIndex];
            Array.Copy(data, fromIndex, destArray, 0, destArray.Length);
            return destArray.Average();
        }

        public Dictionary<FrequencyRange, float> GetFrequencyRangeAverages(
            float[] data,
            float count,
            float maxFrequencyHz)
        {
            if(data.Length < count)
            {
                return null;
            }

            var ranges = new Dictionary<FrequencyRange, float>();
            ranges.Add(
                FrequencyRange.Delta,
                GetAverageOverRange(
                    data,
                    count,
                    maxFrequencyHz,
                    0.1f,
                    3.5f));
            ranges.Add(
                FrequencyRange.Theta,
                GetAverageOverRange(
                    data,
                    count,
                    maxFrequencyHz,
                    4,
                    8));
            ranges.Add(
                FrequencyRange.Alpha,
                GetAverageOverRange(
                    data,
                    count,
                    maxFrequencyHz,
                    8,
                    12));
            ranges.Add(
                FrequencyRange.Beta,
                GetAverageOverRange(
                    data,
                    count,
                    maxFrequencyHz,
                    12,
                    30));
            ranges.Add(
                FrequencyRange.Gamma,
                GetAverageOverRange(
                    data,
                    count,
                    maxFrequencyHz,
                    30,
                    maxFrequencyHz));
            return ranges;
        }
     }
}
