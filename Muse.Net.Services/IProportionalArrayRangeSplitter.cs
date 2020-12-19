using Muse.Net.Models;
using System;

namespace Muse.Net.Services
{
    public interface IProportionalArrayRangeSplitter
    {
        SplitRangeResult SplitRange(
            Array sourceArray,
            float from,
            float to,
            float max);
    }
}
