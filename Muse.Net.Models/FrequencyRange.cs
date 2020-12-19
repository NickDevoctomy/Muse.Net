using Muse.Net.Models.Enums;

namespace Muse.Net.Model
{
    public class FrequencyRange
    {
        public FrequencyRangeGroup FrequencyRangeGroup { get; set; }
        public float FromHz { get; set; }
        public float ToHz { get; set; }
    }
}