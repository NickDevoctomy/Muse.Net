using Muse.Net.Models.Enums;
using System.Threading.Tasks;
using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class MuseSamplerServiceTests
    {
        [Fact]
        public void GivenChannel_And10Values_WhenSample_ThenSamplesStored_AndGetSampleCountForPreviousSecondReturns10()
        {
            // Arrange
            var config = new MuseSamplerServiceConfiguration
            {
                SamplePeriod = new System.TimeSpan(0, 0, 1)
            };
            var sut = new MuseSamplerService(config);
            var channel = Channel.EEG_AF7;
            var values = new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Act
            sut.Sample(
                channel,
                values);

            // Assert
            Assert.Equal(10,
                sut.GetSampleCount(
                    channel,
                    new System.TimeSpan(0, 0, 1)));
        }

        [Fact]
        public async Task GivenChannel_And10Values_WhenSample_ThenSamplesStored_AndWait2Seconds_GetSampleCountForPreviousSecondReturns0()
        {
            // Arrange
            var config = new MuseSamplerServiceConfiguration
            {
                SamplePeriod = new System.TimeSpan(0, 0, 1)
            };
            var sut = new MuseSamplerService(config);
            var channel = Channel.EEG_AF7;
            var values = new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Act
            sut.Sample(
                channel,
                values);
            await Task.Delay(2000);

            // Assert
            Assert.Equal(0,
                sut.GetSampleCount(
                    channel,
                    new System.TimeSpan(0, 0, 1)));
        }
    }
}
