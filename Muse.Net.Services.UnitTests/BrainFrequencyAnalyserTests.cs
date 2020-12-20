using Muse.Net.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class BrainFrequencyAnalyserTests
    {

        [Theory]
        [InlineData(new float[] { 1f, 4f, 2f, 3f, 4f, 5f, 6f, 9f, 7f, 2f }, 100f, 20f, 50f, 3f)]
        [InlineData(new float[] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f }, 100f, 0f, 100f, 5.5f)]
        public void GivenData_AndCount_AndMaxFrequencyHz_AndFromHz_AndToHz_WhenGetAverageOverRange_ThenCorrectValueReturned(
            float[] data,
            float maxFrequencyHz,
            float fromHz,
            float toHz,
            float expectedResult)
        {
            // Arrange
            var count = data.Length;
            var sut = new BrainFrequencyAnalyserService();

            // Act
            var result = sut.GetAverageOverRange(
                data,
                count,
                maxFrequencyHz,
                fromHz,
                toHz);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(new float[] { 100, 1, 1, 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }, 62f, "Delta:1,Theta:2,Alpha:3,Beta:4,Gamma:5")]
        public void GivenData_AndMaxFrequencyHz_WhenGetFrequencyRangeAverages_ThenCorrectAveragesReturned(
            float[] data,
            float maxFrequencyHz,
            string expectedAverages)
        {
            // Arrange
            var count = data.Length;
            var expectedAveragesParsed = new List<KeyValuePair<FrequencyRangeGroup, float>>();
            var averageParts = expectedAverages.Split(',');
            foreach(var curAveragePart in averageParts)
            {
                var curAverage = curAveragePart.Split(':');
                expectedAveragesParsed.Add(new KeyValuePair<FrequencyRangeGroup, float>(
                    Enum.Parse<FrequencyRangeGroup>(curAverage[0]),
                    float.Parse(curAverage[1])));
            }
            var sut = new BrainFrequencyAnalyserService();

            // Act
            var result = sut.GetFrequencyRangeAverages(
                data,
                count,
                maxFrequencyHz);

            // Assert
            foreach(var expectedAverage in expectedAveragesParsed)
            {
                var actual = result.SingleOrDefault(x => x.Key.FrequencyRangeGroup == expectedAverage.Key);
                Assert.Equal(expectedAverage.Value, actual.Value);
            }
        }

        [Fact]
        public void GivenData_AndCountGreaterThanDataLength_WhenGetFrequencyRangeAverages_ThenNullReturned()
        {
            // Arrange
            var data = new float[] { 0, 0, 0, 0, 0 };
            var sut = new BrainFrequencyAnalyserService();

            // Act
            var result = sut.GetFrequencyRangeAverages(
                data,
                10,
                0);

            // Assert
            Assert.Null(result);
        }
    }
}
