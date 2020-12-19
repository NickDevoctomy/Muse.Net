using Moq;
using Muse.Net.Models.Enums;
using System;
using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class FFTSamplerServiceTests
    {
        [Fact]
        public void GivenSamplerService_AndChannel_AndTimeSpan_WhenTryGetFFTSample_ThenGetDft_AndGetMagnitudes_AndSamplesAndTrueReturned()
        {
            // Arrange
            var mockFourierService = new Mock<IFourierService>();
            var mockMuseSamplerService = new Mock<IMuseSamplerService>();
            var channel = Channel.EEG_AF7;
            var timeSpan = new TimeSpan(0, 0, 1);
            var expectedSamples = new float[] { 0 };
            var magnitudes = new float[] { 1, 2, 3 };

            var sut = new FFTSamplerService(mockFourierService.Object);

            mockMuseSamplerService.Setup(x => x.TryGetSamples(
                It.IsAny<Channel>(),
                It.IsAny<TimeSpan>(),
                out expectedSamples))
                .Returns(true);

            mockFourierService.Setup(x => x.DFT(
                It.IsAny<float[]>()))
                .Returns(new System.Numerics.Complex[] { });

            mockFourierService.Setup(x => x.Magnitudes(
                It.IsAny<System.Numerics.Complex[]>()))
                .Returns(magnitudes);

            // Act
            var result = sut.TryGetFFTSample(
                mockMuseSamplerService.Object,
                channel,
                timeSpan,
                out var actualSamples);

            // Assert
            Assert.True(result);
            mockFourierService.Verify(x => x.DFT(
                 It.IsAny<float[]>()), Times.Once);
            mockFourierService.Verify(x => x.Magnitudes(
                It.IsAny<System.Numerics.Complex[]>()), Times.Once);
            Assert.Equal(string.Join(',', magnitudes), string.Join(',', actualSamples));
        }

        [Fact]
        public void GivenSamplerService_AndChannel_AndTimeSpan_WhenTryGetFFTSample_ThenFalseReturned()
        {
            // Arrange
            var mockFourierService = new Mock<IFourierService>();
            var mockMuseSamplerService = new Mock<IMuseSamplerService>();
            var channel = Channel.EEG_AF7;
            var timeSpan = new TimeSpan(0, 0, 1);
            var expectedSamples = new float[] { 0 };
            var magnitudes = new float[] { 1, 2, 3 };

            var sut = new FFTSamplerService(mockFourierService.Object);

            mockMuseSamplerService.Setup(x => x.TryGetSamples(
                It.IsAny<Channel>(),
                It.IsAny<TimeSpan>(),
                out expectedSamples))
                .Returns(false);

            // Act
            var result = sut.TryGetFFTSample(
                mockMuseSamplerService.Object,
                channel,
                timeSpan,
                out var actualSamples);

            // Assert
            Assert.False(result);
        }
    }
}
