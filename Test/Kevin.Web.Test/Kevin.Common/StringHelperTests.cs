using Common;

namespace Kevin.Unit.Test.Kevin.Common
{

    public class StringHelperTests
    {
        [Fact]
        public void GetOrderNo_ShouldReturnValidOrderNo()
        {
            // Arrange
            string sign = "TEST";

            // Act
            string result = StringHelper.GetOrderNo(sign);

            // Assert
            Assert.StartsWith(sign, result);
            Assert.Equal(26, result.Length);
        }

        [Fact]
        public void GetRandomString_ShouldReturnValidString()
        {
            // Arrange
            int length = 10;

            // Act
            string result = StringHelper.GetRandomString(length);

            // Assert
            Assert.Equal(length, result.Length);
            //Assert.Matches("^[a-z][a-z0-9]*$", result);
        }

        [Fact]
        public void RemovePunctuation_ShouldRemoveAllPunctuation()
        {
            // Arrange
            string text = "Hello, World!";

            // Act
            string result = StringHelper.RemovePunctuation(text);

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void IsContainsCN_ShouldDetectChineseCharacters()
        {
            // Arrange
            string chineseText = "你好";
            string englishText = "Hello";

            // Act & Assert
            Assert.True(StringHelper.IsContainsCN(chineseText));
            Assert.False(StringHelper.IsContainsCN(englishText));
        }

        [Fact]
        public void NoHtml_ShouldRemoveAllHtmlTags()
        {
            // Arrange
            string html = "<div>Test</div>";

            // Act
            string result = StringHelper.NoHtml(html);

            // Assert
            Assert.Equal("Test", result);
        }

        [Fact]
        public void NoEmoji_ShouldRemoveEmoji()
        {
            // Arrange
            string text = "Hello😊";

            // Act
            string result = StringHelper.NoEmoji(text);

            // Assert
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void SubstringText_ShouldTruncateAndAddEllipsis()
        {
            // Arrange
            string text = "This is a long text";
            int length = 10;

            // Act
            string result = StringHelper.SubstringText(text, length);

            // Assert
            Assert.Equal("This is a ...", result);
        }

        [Fact]
        public void TextStars_ShouldMaskMiddlePart()
        {
            // Arrange
            string text = "123456789";

            // Act
            string result = StringHelper.TextStars(text);

            // Assert
            Assert.Equal("123***789", result);
        }

        [Fact]
        public void UnicodeToString_ShouldConvertUnicode()
        {
            // Arrange
            string unicode = "\\u4F60\\u597D";

            // Act
            string result = StringHelper.UnicodeToString(unicode);

            // Assert
            Assert.Equal("你好", result);
        }

        [Fact]
        public void GetRandomStringZh_ShouldGenerateChineseName()
        {
            // Arrange
            int length = 3;

            // Act
            string result = StringHelper.GetRandomStringZh(length);

            // Assert
            Assert.Equal(length, result.Length);
            Assert.Matches("^[\u4e00-\u9fa5]+$", result);
        }
    }


}
