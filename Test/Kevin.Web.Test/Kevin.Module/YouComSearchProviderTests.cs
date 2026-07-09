using kevin.AI.AgentFramework.Tools;

namespace Kevin.Unit.Tests.Kevin.Module
{
    /// <summary>
    /// YouComSearchProvider 单元测试。
    /// 只覆盖纯函数FormatResults（JSON->格式化文本），不发起真实网络请求，
    /// 因此无需mock即可离线运行。SearchAsync的联网调用见下方的live-gated测试。
    /// </summary>
    public class YouComSearchProviderTests
    {
        [Fact]
        public void FormatResults_WithWebResults_ShouldReturnNumberedText()
        {
            // Arrange
            var json = @"{
                ""results"": {
                    ""web"": [
                        {
                            ""title"": ""标题一"",
                            ""url"": ""https://example.com/1"",
                            ""description"": ""描述一"",
                            ""snippets"": [""摘要片段A"", ""摘要片段B""]
                        },
                        {
                            ""title"": ""标题二"",
                            ""url"": ""https://example.com/2"",
                            ""description"": ""描述二"",
                            ""snippets"": [""摘要片段C""]
                        }
                    ]
                }
            }";

            // Act
            var result = YouComSearchProvider.FormatResults(json, 10);

            // Assert
            Assert.Contains("1. 标题一", result);
            Assert.Contains("链接: https://example.com/1", result);
            Assert.Contains("描述: 描述一", result);
            Assert.Contains("- 摘要片段A", result);
            Assert.Contains("- 摘要片段B", result);
            Assert.Contains("2. 标题二", result);
            Assert.Contains("信息来源: You.com", result);
        }

        [Fact]
        public void FormatResults_WithMissingOptionalFields_ShouldNotThrowAndSkipMissingParts()
        {
            // Arrange: url/description/snippets都缺失，只有title
            var json = @"{
                ""results"": {
                    ""web"": [
                        { ""title"": ""仅有标题"" }
                    ]
                }
            }";

            // Act
            var result = YouComSearchProvider.FormatResults(json, 10);

            // Assert
            Assert.Contains("1. 仅有标题", result);
            Assert.DoesNotContain("链接:", result);
            Assert.DoesNotContain("描述:", result);
            Assert.DoesNotContain("摘要:", result);
            Assert.Contains("信息来源: You.com", result);
        }

        [Fact]
        public void FormatResults_WithCompletelyMissingTitle_ShouldUsePlaceholder()
        {
            // Arrange
            var json = @"{ ""results"": { ""web"": [ { ""url"": ""https://example.com"" } ] } }";

            // Act
            var result = YouComSearchProvider.FormatResults(json, 10);

            // Assert
            Assert.Contains("1. 无标题", result);
        }

        [Fact]
        public void FormatResults_WithEmptyWebArray_ShouldReturnEmptyString()
        {
            // Arrange
            var json = @"{ ""results"": { ""web"": [] } }";

            // Act
            var result = YouComSearchProvider.FormatResults(json, 10);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatResults_WithNoWebKey_ShouldReturnEmptyString()
        {
            // Arrange: news-only结果，没有web字段
            var json = @"{ ""results"": { ""news"": [] } }";

            // Act
            var result = YouComSearchProvider.FormatResults(json, 10);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatResults_WithMalformedJson_ShouldReturnEmptyStringWithoutThrow()
        {
            // Arrange
            var malformedJson = "{ this is not valid json ";

            // Act
            var result = YouComSearchProvider.FormatResults(malformedJson, 10);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FormatResults_WithNullOrEmptyJson_ShouldReturnEmptyString()
        {
            Assert.Equal(string.Empty, YouComSearchProvider.FormatResults("", 10));
            Assert.Equal(string.Empty, YouComSearchProvider.FormatResults(null!, 10));
        }

        [Fact]
        public void FormatResults_ShouldCapResultCountAtRequestedCount()
        {
            // Arrange: 5条结果，但请求只展示2条
            var items = string.Join(",", Enumerable.Range(1, 5).Select(i =>
                $@"{{ ""title"": ""标题{i}"", ""url"": ""https://example.com/{i}"" }}"));
            var json = $@"{{ ""results"": {{ ""web"": [{items}] }} }}";

            // Act
            var result = YouComSearchProvider.FormatResults(json, 2);

            // Assert
            Assert.Contains("1. 标题1", result);
            Assert.Contains("2. 标题2", result);
            Assert.DoesNotContain("3. 标题3", result);
        }

        [Fact]
        public void FormatResults_ShouldNeverExceedHardCapOfTwenty()
        {
            // Arrange: 25条结果，请求count=25，应被限制在20条以内
            var items = string.Join(",", Enumerable.Range(1, 25).Select(i =>
                $@"{{ ""title"": ""标题{i}"", ""url"": ""https://example.com/{i}"" }}"));
            var json = $@"{{ ""results"": {{ ""web"": [{items}] }} }}";

            // Act
            var result = YouComSearchProvider.FormatResults(json, 25);

            // Assert
            Assert.Contains("20. 标题20", result);
            Assert.DoesNotContain("21. 标题21", result);
        }

        [Fact]
        public void IsEnabled_WithoutInitializedConfiguration_ShouldBeFalse()
        {
            // Arrange & Act: 单元测试进程中未调用ConfigHelper.Initialize，
            // Configuration应为空，IsEnabled必须安全地返回false而不是抛异常。
            var isEnabled = YouComSearchProvider.IsEnabled;

            // Assert
            Assert.False(isEnabled);
        }

        [Fact]
        public async Task SearchAsync_LiveCall_ShouldReturnStructuredResultsWhenKeyConfigured()
        {
            // 联网门控测试：仅当环境变量YDC_API_KEY存在时才真正发起一次真实调用。
            // 未设置时直接跳过（早返回），不消耗任何API调用额度。
            var apiKey = Environment.GetEnvironmentVariable("YDC_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return;
            }

            // 直接调用You.com接口验证真实响应能被正确格式化（不依赖ConfigHelper/appsettings）。
            var handler = new HttpClientHandler();
            using var http = new HttpClient(handler);
            http.Timeout = TimeSpan.FromSeconds(15);
            http.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            var response = await http.GetAsync("https://ydc-index.io/v1/search?query=You.com&count=3");

            Assert.True(response.IsSuccessStatusCode, $"You.com接口调用失败: {(int)response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            var formatted = YouComSearchProvider.FormatResults(json, 3);

            Assert.False(string.IsNullOrEmpty(formatted));
            Assert.Contains("信息来源: You.com", formatted);
        }
    }
}
