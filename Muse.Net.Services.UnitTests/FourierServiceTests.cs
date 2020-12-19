using System.Linq;
using System.Numerics;
using Xunit;

namespace Muse.Net.Services.UnitTests
{
    public class FourierServiceTests
    {
        [Fact]
        public void GivenComplexArray_WhenMagnitudes_ThenCorrectValuesReturned()
        {
            // Arrange
            var complexArray = new Complex[]
                { 
                    new Complex(23098, 0),
                    new Complex(-2148.89156465286, 2881.90965595286),
                    new Complex(3475.5, -1781.41425558459),
                    new Complex(-5345.00000000001, 2637),
                    new Complex(2045.5, 2110.50390902267),
                    new Complex(3275.89156465285, 3178.09034404714),
                    new Complex(2520, -9.32643171676004E-12),
                    new Complex(3275.89156465286, -3178.09034404714),
                    new Complex(2045.5, -2110.50390902269),
                    new Complex(-5345.00000000001, -2637.00000000001),
                    new Complex(3475.5, 1781.41425558461),
                    new Complex(-2148.89156465283, -2881.90965595287)
                };
            var expectedResult = new float[]
            {
                23098f,
                3594.87671f,
                3905.44971f,
                5960.1f,
                2939.098f,
                4564.178f,
                2520f,
                4564.178f,
                2939.098f,
                5960.1f,
                3905.44971f,
                3594.87671f,
            };
            var sut = new FourierService();

            // Act
            var results = sut.Magnitudes(complexArray);

            // Assert
            Assert.Equal(expectedResult, results);
        }

        [Fact]
        public void GivenData_WhenDFT_ThenCorrectValuesReturned()
        {
            // Arrange
            var data = new System.ReadOnlySpan<float>(new float[]
            {
                2986,
                529,
                237,
                2621,
                2048,
                2046,
                1983,
                2278,
                1129,
                1101,
                3613,
                179
            });
            var expectedResults = new Complex[]
                {
                    new Complex(20750, 0),
                    new Complex(-1792.0478600845327, 548.8244170983645),
                    new Complex(249.4999999999946, 3215.552324251618),
                    new Complex(330, 1401.9999999999977),
                    new Complex(2661.5000000000095, -4223.605894256716),
                    new Complex(4471.047860084534, -3706.8244170983617),
                    new Complex(3242, 1.7251832802508072E-12),
                    new Complex(4471.047860084529, 3706.8244170983653),
                    new Complex(2661.4999999999814, 4223.605894256691),
                    new Complex(329.99999999999187, -1402.0000000000186),
                    new Complex(249.50000000000324, -3215.5523242516265),
                    new Complex(-1792.0478600845172, -548.8244170983775)
                };
            var sut = new FourierService();

            // Act
            var results = sut.DFT(data);

            // Assert
            var expectedString = string.Join('-', expectedResults.Select(x => $"{x.Real},{x.Imaginary}").ToArray());
            var actualString = string.Join('-', results.Select(x => $"{x.Real},{x.Imaginary}").ToArray());
            Assert.Equal(
                expectedString,
                actualString);
        }
    }
}