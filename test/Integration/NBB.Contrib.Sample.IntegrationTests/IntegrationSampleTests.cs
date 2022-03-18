using FluentAssertions;
using Xunit;

namespace NBB.Contrib.Sample.IntegrationTests
{
    public class IntegrationSampleTests
    {
        public IntegrationSampleTests()
        {
        }

        [Fact]
        public void Integration_sample_test()
        {
            //Arrange
            var x = 1;

            //Act
            x++;
            //Assert
            x.Should().Be(2);
        }
    }
}
