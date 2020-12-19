using Muse.Net.Model;
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
            int fromIndex = (int)Math.Floor((double)fromHz / freqPerIndex) - 1;
            if (fromIndex <= 0)
            {
                fromIndex = 1;
            }
            int toIndex = (int)Math.Ceiling((double)toHz / freqPerIndex) - 1;
            int destSize = toIndex - fromIndex;
            if(destSize <= 0)
            {
                destSize = 1;
            }
            var destArray = new float[destSize];
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
            foreach(var curRange in FrequencyRanges.All)
            {
                ranges.Add(
                    curRange,
                    GetAverageOverRange(
                        data,
                        count,
                        maxFrequencyHz,
                        curRange.FromHz,
                        curRange.ToHz));
            }
            return ranges;
        }
     }
}