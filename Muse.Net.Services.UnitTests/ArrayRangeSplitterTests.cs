using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class ArrayRangeSplitterTests
    {
        [Theory]
        [InlineData(10, 1, 5, 10, 0, 4)]
        [InlineData(100, 1, 5, 10, 9, 49)]
        [InlineData(62, 1, 4, 62, 0, 3)]
        [InlineData(62, 4, 8, 62, 3, 7)]
        [InlineData(62, 8, 12, 62, 7, 11)]
        [InlineData(62, 12, 30, 62, 11, 29)]
        [InlineData(62, 30, 62, 62, 29, 61)]
        [InlineData(100, 100, 200, 200, 49, 99)]
        [InlineData(100, 0, 200, 200, 0, 99)]
        [InlineData(100, 10, 20, 100, 9, 19)]
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
