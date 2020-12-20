﻿using Muse.Net.Models;
using System;

namespace Muse.Net.Services
{
    public class ProportionalArrayRangeSplitter : IProportionalArrayRangeSplitterService
    {
        public SplitRangeResult SplitRange(
            Array sourceArray,
            float from,
            float to,
            float max)
        {
            double freqPerIndex = (double)max / (double)sourceArray.Length;
            int fromIndex = (int)Math.Floor((double)from / freqPerIndex);
            int toIndex = (int)Math.Ceiling((double)to / freqPerIndex);
            return new SplitRangeResult
            {
                From = fromIndex > 0 ? fromIndex - 1 : 0,
                To = toIndex - 1
            };
        }
    }
}