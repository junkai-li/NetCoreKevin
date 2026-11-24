using kevin.AI.AgentFramework.Agent.KevinChatMessageStore;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Kevin.Unit.Test.Kevin.AI.AgentFramework
{
    /// <summary>
    /// ShrinkMediaForStore 入库副本媒体收缩测试：
    /// 大 PDF 整页渲染/多图上传时 base64 累计可达十几 MB，单行 INSERT 会撑爆 MySQL max_allowed_packet（默认 4MB），
    /// 入库前必须把超限 DataContent 替换为占位文本，且不得改动当轮送模型的原始消息。
    /// </summary>
    public class KevinChatMessageStoreTests
    {
        /// <summary>1x1 红色像素的合法 PNG 字节（95 字节，远小于兜底上限）</summary>
        private static readonly byte[] SmallPngBytes = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");

        /// <summary>构造一条带超大媒体附件的 user 消息（模拟大 PDF 整页渲染结果）</summary>
        private static ChatMessage BuildMessageWithImages(int imageCount, int imageBytes)
        {
            var contents = new List<AIContent> { new TextContent("帮我总结这个文件") };
            for (var i = 0; i < imageCount; i++)
            {
                contents.Add(new DataContent(new byte[imageBytes], "image/png"));
            }
            return new ChatMessage(ChatRole.User, contents)
            {
                MessageId = "test-message-1",
                CreatedAt = DateTimeOffset.Now,
            };
        }

        [Fact]
        public void Shrink_SmallMedia_ReturnsOriginalInstance()
        {
            var message = BuildMessageWithImages(imageCount: 2, imageBytes: SmallPngBytes.Length);

            var shrunk = KevinChatMessageStore.ShrinkMediaForStore(message, 512 * 1024);

            // 无超限附件：零开销返回原消息（引用不变，序列化结果不变）
            Assert.Same(message, shrunk);
        }

        [Fact]
        public void Shrink_OversizeMedia_ReplacedWithPlaceholder_AndOriginalUnchanged()
        {
            var message = BuildMessageWithImages(imageCount: 1, imageBytes: 60_000);

            var shrunk = KevinChatMessageStore.ShrinkMediaForStore(message, 40_000);

            // 入库副本：超大附件替换为占位文本，其余内容保留
            Assert.NotSame(message, shrunk);
            Assert.Equal(2, shrunk.Contents.Count);
            Assert.IsType<TextContent>(shrunk.Contents[0]);
            var placeholder = Assert.IsType<TextContent>(shrunk.Contents[1]);
            Assert.Contains("体积过大", placeholder.Text);
            Assert.Contains("image/png", placeholder.Text);

            // 原始消息（当轮送模型用）不得被改动
            Assert.IsType<DataContent>(message.Contents[1]);
        }

        [Fact]
        public void Shrink_MultiOversizeImages_SerializedCopy_FitsPacketBudget()
        {
            // 模拟 10 张大图：原始消息序列化超过 MySQL 默认 max_allowed_packet（4MB）
            var message = BuildMessageWithImages(imageCount: 10, imageBytes: 500_000);
            var originalJson = JsonSerializer.Serialize(message);
            Assert.True(originalJson.Length > 4_000_000, "前置条件：原始序列化应超过 4MB 包限制");

            var shrunk = KevinChatMessageStore.ShrinkMediaForStore(message, 40_000, 1_500_000);
            var shrunkJson = JsonSerializer.Serialize(shrunk);

            Assert.True(shrunkJson.Length < 100_000, $"入库副本序列化长度应显著回落，实际 {shrunkJson.Length}");
            // 占位文本数量与图片数量一致，正文保留（中文会被默认序列化转义，用 ASCII 的 MIME 标记验证）
            Assert.Equal(message.Contents.Count, shrunk.Contents.Count);
            // 每个占位文本都保留 MIME 类型标记（ASCII 不受转义影响），10 张图应有 10 处
            Assert.Equal(10, shrunkJson.Split("image/png").Length - 1);
        }

        [Fact]
        public void Shrink_NonPositiveLimit_ReturnsOriginalInstance()
        {
            var message = BuildMessageWithImages(imageCount: 1, imageBytes: 60_000);

            Assert.Same(message, KevinChatMessageStore.ShrinkMediaForStore(message, 0));
            Assert.Same(message, KevinChatMessageStore.ShrinkMediaForStore(message, -1));
        }

        [Fact]
        public void Shrink_TotalBudget_ReplacesLargestFirst_SmallerMediaKept()
        {
            // 阶段一阈值 600KB（两张图均未超限），总量预算 500KB：
            // 累计 1MB + 100KB 超预算 → 按体积从大到小，1MB 图被替换为占位文本，100KB 图保留为 DataContent
            var message = new ChatMessage(ChatRole.User, new List<AIContent>
            {
                new TextContent("两张图"),
                new DataContent(new byte[1024 * 1024], "image/png"),
                new DataContent(new byte[100 * 1024], "image/png"),
            })
            { MessageId = "test-budget-largest-first" };

            var shrunk = KevinChatMessageStore.ShrinkMediaForStore(message, 600 * 1024, 500 * 1024);

            Assert.NotSame(message, shrunk);
            var placeholder = Assert.IsType<TextContent>(shrunk.Contents[1]);
            Assert.Contains("体积过大", placeholder.Text);
            Assert.Contains("image/png", placeholder.Text);
            // 体积较小的媒体保留为 DataContent，剩余媒体体积 ≤ 预算
            var kept = Assert.IsType<DataContent>(shrunk.Contents[2]);
            Assert.Equal(100 * 1024, kept.Data.Length);
            Assert.True(kept.Data.Length <= 500 * 1024, $"替换后剩余媒体体积应 ≤ 预算，实际 {kept.Data.Length}");
            // 原始消息（当轮送模型用）不得被改动
            Assert.IsType<DataContent>(message.Contents[1]);
            Assert.IsType<DataContent>(message.Contents[2]);
        }

        [Fact]
        public void Shrink_MediaWithinBudget_ReturnsOriginalInstance_ZeroOverhead()
        {
            // 累计 200KB ≤ 预算 1.5MB 且单图未超 512KB 阈值 → 未发生任何替换，零开销返回原实例
            var message = BuildMessageWithImages(imageCount: 2, imageBytes: 100 * 1024);

            var shrunk = KevinChatMessageStore.ShrinkMediaForStore(message, 512 * 1024, 1536 * 1024);

            Assert.Same(message, shrunk);
        }

        [Fact]
        public void Shrink_BudgetExhausted_AllMediaReplaced_PlaceholderCountMatchesMediaCount()
        {
            // 3 张 1MB 图，阶段一阈值 2MB（均不触发），总量预算 512KB：
            // 从大到小逐个替换直至 sum=0（预算耗尽兜底），占位文本数量 = 媒体数量，序列化体积显著回落
            var message = BuildMessageWithImages(imageCount: 3, imageBytes: 1024 * 1024);
            var originalJson = JsonSerializer.Serialize(message);
            Assert.True(originalJson.Length > 3_000_000, "前置条件：3 张 1MB 图序列化应显著超过预算");

            var shrunk = KevinChatMessageStore.ShrinkMediaForStore(message, 2 * 1024 * 1024, 512 * 1024);
            var shrunkJson = JsonSerializer.Serialize(shrunk);

            Assert.True(shrunkJson.Length < 100_000, $"入库副本序列化长度应显著回落，实际 {shrunkJson.Length}");
            Assert.Equal(message.Contents.Count, shrunk.Contents.Count);
            Assert.DoesNotContain(shrunk.Contents, c => c is DataContent);
            // 每个占位文本保留 MIME 标记：3 张图应有 3 处 image/png
            Assert.Equal(3, shrunkJson.Split("image/png").Length - 1);
        }

        [Fact]
        public void Shrink_ZeroBudget_CompatibleLegacyBehavior_NoTotalBudgetEnforcement()
        {
            // budget=0（含单参默认调用）：不做总量预算，仅单媒体阈值生效——
            // 两张 100KB 图均未超 512KB 阈值，即使累计 200KB 超过任何合理预算也原样返回（与改造前行为一致）
            var message = BuildMessageWithImages(imageCount: 2, imageBytes: 100 * 1024);

            Assert.Same(message, KevinChatMessageStore.ShrinkMediaForStore(message, 512 * 1024, 0));
            Assert.Same(message, KevinChatMessageStore.ShrinkMediaForStore(message, 512 * 1024));
        }
    }
}
