using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class ArrayRangeSplitterTests
    {
        [Theory]
        [InlineData(10, 1, 5, 10, 1, 5)]
        [InlineData(100, 1, 5, 10, 10, 50)]
        [InlineData(62, 1, 4, 62, 1, 4)]
        [InlineData(62, 4, 8, 62, 4, 8)]
        [InlineData(62, 8, 12, 62, 8, 12)]
        [InlineData(62, 12, 30, 62, 12, 30)]
        [InlineData(62, 30, 62, 62, 30, 62)]
        public void Given_When_Then(
            int sourceArraySize,
            float from,
            float to,
            float max,
            int expectedRangeFrom,
            int expectedRangeTo)
        {
            // Arrange
            var sourceArray = new int[sourceArraySize];
            var sut = new ProportionalArrayRangeSplitter();

            // Act
            var result = sut.SplitRange(
                sourceArray,
                from,
                to,
                max);

            // Assert
            Assert.Equal(expectedRangeFrom, result.From);
            Assert.Equal(expectedRangeTo, result.To);
        }
    }
}
