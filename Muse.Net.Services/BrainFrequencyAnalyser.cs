using Muse.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Muse.Net.Services
{
    public class BrainFrequencyAnalyser : IBrainFrequencyAnalyser
    {
        private IProportionalArrayRangeSplitter _arrayRangeSplitter = new ProportionalArrayRangeSplitter();

        public float GetAverageOverRange(
            float[] data,
            float count,
            float maxFrequencyHz,
            float fromHz,
            float toHz)
        {
            var range = _arrayRangeSplitter.SplitRange(
                data,
                fromHz,
                toHz,
                maxFrequencyHz);
            if(range.From == 0)
            {
                range.From = 1;
            }

            int destSize = range.To - range.From;
            var destArray = new float[destSize];
            Array.Copy(
                data,
                range.From,
                destArray,
                0,
                destArray.Length);
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