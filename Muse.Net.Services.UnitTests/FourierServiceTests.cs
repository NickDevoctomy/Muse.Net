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
    }
}