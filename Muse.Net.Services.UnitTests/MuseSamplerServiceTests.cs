using Muse.Net.Models.Enums;
using System.Threading.Tasks;
using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class MuseSamplerServiceTests
    {
        [Fact]
        public void GivenSample10Values_WhenGetSampleCountForLastSecond_Then10Returned()
        {
            // Arrange
            var config = new MuseSamplerServiceConfiguration
            {
                SamplePeriod = new System.TimeSpan(0, 0, 1)
            };
            var sut = new MuseSamplerService(config);
            var channel = Channel.EEG_AF7;
            var values = new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            sut.Sample(
                channel,
                values);

            // Act
            var result = sut.GetSampleCount(
                    channel,
                    new System.TimeSpan(0, 0, 1));

            // Assert
            Assert.Equal(10, result);
        }

        [Fact]
        public async Task GivenSample10Values_AndWait2Seconds_WhenGetSampleCountForLastSecond_Then0Returned()
        {
            // Arrange
            var config = new MuseSamplerServiceConfiguration
            {
                SamplePeriod = new System.TimeSpan(0, 0, 1)
            };
            var sut = new MuseSamplerService(config);
            var channel = Channel.EEG_AF7;
            var values = new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            sut.Sample(
                channel,
                values);
            await Task.Delay(2000);

            // Act
            var result = sut.GetSampleCount(
                    channel,
                    new System.TimeSpan(0, 0, 1));

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GivenSample10Values_WheTryGetSamplesForLastSecond_ThenAllSamplesReturned()
        {
            // Arrange
            var config = new MuseSamplerServiceConfiguration
            {
                SamplePeriod = new System.TimeSpan(0, 0, 1)
            };
            var sut = new MuseSamplerService(config);
            var channel = Channel.EEG_AF7;
            var values = new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            sut.Sample(
                channel,
                values);

            // Act
            var gotSamples = sut.TryGetSamples(
                    channel,
                    new System.TimeSpan(0, 0, 1),
                    out var samples);

            // Assert
            Assert.True(gotSamples);
            Assert.Equal(10, samples.Length);
            Assert.Equal(values, samples);
        }

        [Fact]
        public async Task GivenSample10Values_AndWait2Seconds_WheTryGetSamplesForLastSecond_ThenNoSamplesReturned()
        {
            // Arrange
            var config = new MuseSamplerServiceConfiguration
            {
                SamplePeriod = new System.TimeSpan(0, 0, 1)
            };
            var sut = new MuseSamplerService(config);
            var channel = Channel.EEG_AF7;
            var values = new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            sut.Sample(
                channel,
                values);
            await Task.Delay(2000);

            // Act
            var gotSamples = sut.TryGetSamples(
                    channel,
                    new System.TimeSpan(0, 0, 1),
                    out var samples);

            // Assert
            Assert.True(gotSamples);
            Assert.Empty(samples);
        }
    }
}
