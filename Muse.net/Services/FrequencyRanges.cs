namespace Muse.Net.Services
{
    public static class FrequencyRanges
    {
        public static FrequencyRange[] All = new FrequencyRange[]
        {
            new FrequencyRange
            {
                FrequencyRangeGroup = FrequencyRangeGroup.Delta,
                FromHz = 0.1f,
                ToHz = 3.5f
            },
            new FrequencyRange
            {
                FrequencyRangeGroup = FrequencyRangeGroup.Theta,
                FromHz = 4f,
                ToHz = 8f
            },
            new FrequencyRange
            {
                FrequencyRangeGroup = FrequencyRangeGroup.Alpha,
                FromHz = 8f,
                ToHz = 12f
            },
            new FrequencyRange
            {
                FrequencyRangeGroup = FrequencyRangeGroup.Beta,
                FromHz = 12f,
                ToHz = 30f
            },
            new FrequencyRange
            {
                FrequencyRangeGroup = FrequencyRangeGroup.Gamma,
                FromHz = 30f,
                ToHz = 62f
            },
        };
    }
}