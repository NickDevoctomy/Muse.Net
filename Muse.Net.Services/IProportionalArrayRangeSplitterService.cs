using Muse.Net.Models;
using System;

namespace Muse.Net.Services
{
    public interface IProportionalArrayRangeSplitterService
    {
        SplitRangeResult SplitRange(
            Array sourceArray,
            float from,
            float to,
            float max);
    }
}
