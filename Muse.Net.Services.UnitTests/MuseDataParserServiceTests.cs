using System;
using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class MuseDataParserServiceTests
    {
        [Fact]
        public void GivenData_WhenTelemetry_ThenParsedTelemetryReturned()
        {
            // Arrange 
            var data = new byte[] { 255, 255, 255, 255, 255, 255, 0, 0, 255, 255 };
            var sut = new MuseDataParserService();

            // Act
            var result = sut.Telemetry(data);

            // Assert
            Assert.Equal(65535, result.SequenceId);
            Assert.Equal(127.99805f, result.BatteryLevel);
            Assert.Equal(144177, result.Voltage);
            Assert.Equal(65535, result.Temperature);
        }

        [Fact]
        public void GivenData_WhenGyroscope_ThenParsedTelemetryReturned()
        {
            // Arrange 
            var data = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            var sut = new MuseDataParserService();

            // Act
            var result = sut.Gyroscope(data);

            // Assert
            Assert.Equal(65535, result.SequenceId);
            Assert.Equal(3, result.Samples.Length);
            Assert.Equal("-6.10352E-05,-6.10352E-05,-6.10352E-05", $"{result.Samples[0].X},{result.Samples[0].Y},{result.Samples[0].Z}");
            Assert.Equal("-6.10352E-05,-6.10352E-05,-6.10352E-05", $"{result.Samples[1].X},{result.Samples[1].Y},{result.Samples[1].Z}");
            Assert.Equal("-6.10352E-05,-6.10352E-05,-6.10352E-05", $"{result.Samples[2].X},{result.Samples[2].Y},{result.Samples[2].Z}");
        }

        [Fact]
        public void GivenData_WhenAccelerometer_ThenParsedAccelerometerReturned()
        {
            // Arrange 
            var data = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            var sut = new MuseDataParserService();

            // Act
            var result = sut.Accelerometer(data);

            // Assert
            Assert.Equal(65535, result.SequenceId);
            Assert.Equal(3, result.Samples.Length);
            Assert.Equal("-0.0074768,-0.0074768,-0.0074768", $"{result.Samples[0].X},{result.Samples[0].Y},{result.Samples[0].Z}");
            Assert.Equal("-0.0074768,-0.0074768,-0.0074768", $"{result.Samples[1].X},{result.Samples[1].Y},{result.Samples[1].Z}");
            Assert.Equal("-0.0074768,-0.0074768,-0.0074768", $"{result.Samples[2].X},{result.Samples[2].Y},{result.Samples[2].Z}");
        }

        [Fact]
        public void GivenData_WhenEncefalogram_ThenParsedEncefalogramReturned()
        {
            // Arrange 
            var data = new byte[] { 255, 255, 255, 255, 255, 255, 0, 0, 255, 255 };
            var sut = new MuseDataParserService();

            // Act
            var result = sut.Encefalogram(data);

            // Assert
            Assert.Equal(65535, result.Index);
            Assert.True(new DateTimeOffset(DateTime.Now.Subtract(new TimeSpan(0, 0, 1))) < result.Timestamp);
            Assert.True(new DateTimeOffset(DateTime.Now) > result.Timestamp);
            Assert.Equal(10, result.Raw.Length);
            Assert.Equal(data, result.Raw);
            Assert.Equal(new Single[] { 4095, 4095, 4080, 0, 4095 }, result.Samples);
        }


        [Fact]
        public void GivenSamples_WhenEncefalogram_ThenParsedEncefalogramReturned()
        {
            // Arrange 
            var samples = new float[] { 100f };
            var sut = new MuseDataParserService();

            // Act
            sut.ScaleEeg(samples);

            // Assert
            Assert.Equal(-951.1719f, samples[0]);
        }

        [Fact]
        public void GivenSpan_WhenEncefalogram_ThenParsedEncefalogramReturned()
        {
            // Arrange 
            var span = new ReadOnlySpan<byte>(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 });
            var sut = new MuseDataParserService();

            // Act
            var result = sut.Samples(
                span,
                3,
                1);

            // Assert
            Assert.Equal(3, result.Length);
            Assert.Equal("-1,-1,-1", $"{result[0].X},{result[0].Y},{result[0].Z}");
            Assert.Equal("-1,-1,-1", $"{result[1].X},{result[1].Y},{result[1].Z}");
            Assert.Equal("-1,-1,-1", $"{result[2].X},{result[2].Y},{result[2].Z}");
        }

        [Fact]
        public void GivenSpan_WhenVector_ThenParsedVectorReturned()
        {
            // Arrange 
            var span = new ReadOnlySpan<byte>(new byte[] { 128, 255, 128, 255, 128, 255 });
            var sut = new MuseDataParserService();

            // Act
            var result = sut.Vector(span);

            // Assert
            Assert.Equal(-32513, result.X);
            Assert.Equal(-32513, result.Y);
            Assert.Equal(-32513, result.Z);
        }

        [Fact]
        public void GivenSpan_WhenUShort_ThenParsedUShortReturned()
        {
            // Arrange 
            var span = new ReadOnlySpan<byte>(new byte[] { 128, 255 });
            var sut = new MuseDataParserService();

            // Act
            var result = sut.UShort(span);

            // Assert
            Assert.Equal(33023, result);
        }

        [Fact]
        public void GivenSpan_WhenInt16_ThenParsedInt16Returned()
        {
            // Arrange 
            var span = new ReadOnlySpan<byte>(new byte[] { 128, 255 });
            var sut = new MuseDataParserService();

            // Act
            var result = sut.Int16(span);

            // Assert
            Assert.Equal(-32513, result);
        }
    }
}
