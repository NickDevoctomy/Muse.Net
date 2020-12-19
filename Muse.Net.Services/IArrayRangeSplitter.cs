using System;

namespace Muse.Net.Services
{
    public interface IArrayRangeSplitter
    {
        SplitRangeResult SplitRange(
            Array sourceArray,
            float from,
            float to,
            float max);
    }
}
