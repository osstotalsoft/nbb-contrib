using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NBB.Contrib.Sample.Tests
{
    public class SampleTests
    {

        public SampleTests()
        {
        }

        [Fact]
        public void Sample_test()
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
