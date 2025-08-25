using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Web.Test.Kevin.Common
{
    public class GuidHelperTests
    {
        [Fact]
        public void Reduce_ShouldReturn22CharString()
        {
            // Arrange
            Guid guid = Guid.NewGuid();

            // Act
            string result = GuidHelper.Reduce(guid);

            // Assert
            Assert.Equal(22, result.Length);
        }

        [Fact]
        public void Reduce_ShouldReplaceSpecialChars()
        {
            // Arrange
            Guid guid = Guid.NewGuid();

            // Act
            string result = GuidHelper.Reduce(guid);

            // Assert
            Assert.DoesNotContain("/", result);
            Assert.DoesNotContain("+", result);
        }

        [Fact]
        public void Reduction_ShouldReturnOriginalGuid_WhenGivenValidString()
        {
            // Arrange
            Guid original = Guid.NewGuid();
            string reduced = GuidHelper.Reduce(original);

            // Act
            Guid restored = GuidHelper.Reduction(reduced);

            // Assert
            Assert.Equal(original, restored);
        }

        [Fact]
        public void Reduction_ShouldReturnEmptyGuid_WhenInputIsNull()
        {
            // Arrange & Act
            Guid result = GuidHelper.Reduction(null);

            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void Reduction_ShouldReturnEmptyGuid_WhenInputIsEmpty()
        {
            // Arrange & Act
            Guid result = GuidHelper.Reduction("");

            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void Reduction_ShouldReturnEmptyGuid_WhenInputIsInvalidLength()
        {
            // Arrange & Act
            Guid result = GuidHelper.Reduction("12345");

            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void Reduction_ShouldReturnEmptyGuid_WhenInputIsInvalidFormat()
        {
            // Arrange
            string invalidBase64 = new string('*', 22);

            // Act
            Guid result = GuidHelper.Reduction(invalidBase64);

            // Assert
            Assert.Equal(Guid.Empty, result);
        }
    }
}
