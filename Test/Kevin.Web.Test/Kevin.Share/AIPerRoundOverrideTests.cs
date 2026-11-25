using kevin.Domain.Share.Dtos.AI;

namespace Kevin.Unit.Test.Kevin.Share
{
    /// <summary>
    /// AIPerRoundOverride.From 单元测试（TASK-04）：
    /// 覆盖 / 弹层动态选择知识库与 Skill 的入参解析链路 —— 字符串 Id 解析容错（非法/非正数忽略）、
    /// 去重保序、忽略默认开关透传，以及旧客户端不传新字段时等价于空 override 的兼容性承诺。
    /// </summary>
    public class AIPerRoundOverrideTests
    {
        #region 兼容性：旧请求不传新字段 → 空 override（行为与现状一致）

        [Fact]
        public void From_WithNullDto_ShouldReturnEmptyOverride()
        {
            // Act
            var result = AIPerRoundOverride.From(null!);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.SelectedKmsIds);
            Assert.Empty(result.SelectedSkillIds);
            Assert.False(result.ExcludeDefaultKmss);
            Assert.False(result.ExcludeDefaultSkills);
        }

        [Fact]
        public void From_WithDefaultDto_ShouldReturnEmptyIdsAndFalseFlags()
        {
            // Arrange：旧客户端 / 聊天室 / OpenAI 兼容入口的请求体（不携带 4 个新字段）
            var dto = new AIChatHistorysDto();

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert：等价于空 override，下游保持原行为
            Assert.Empty(result.SelectedKmsIds);
            Assert.Empty(result.SelectedSkillIds);
            Assert.False(result.ExcludeDefaultKmss);
            Assert.False(result.ExcludeDefaultSkills);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new int[0])]
        public void From_WithNullOrEmptySelectedKmsIds_ShouldReturnEmptyList(int[]? _)
        {
            // Arrange
            var dto = new AIChatHistorysDto { SelectedKmsIds = null };

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert
            Assert.Empty(result.SelectedKmsIds);
        }

        #endregion

        #region 正常解析：字符串 Id → long

        [Fact]
        public void From_WithValidIds_ShouldParseToStringLongPairs()
        {
            // Arrange
            var dto = new AIChatHistorysDto
            {
                SelectedKmsIds = new List<string> { "11", "22" },
                SelectedSkillIds = new List<string> { "33", "44" }
            };

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert
            Assert.Equal(new List<long> { 11, 22 }, result.SelectedKmsIds);
            Assert.Equal(new List<long> { 33, 44 }, result.SelectedSkillIds);
        }

        #endregion

        #region 容错：非法 / 非正数 Id 忽略

        [Theory]
        [InlineData("abc")]   // 非数字
        [InlineData("0")]     // 非正数
        [InlineData("-5")]    // 负数
        [InlineData("")]      // 空串
        [InlineData("  ")]    // 空白
        [InlineData("1.5")]   // 小数
        public void From_WithInvalidKmsId_ShouldIgnoreIt(string invalid)
        {
            // Arrange
            var dto = new AIChatHistorysDto { SelectedKmsIds = new List<string> { invalid, "7" } };

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert：非法 Id 被忽略，合法 Id 保留
            Assert.Equal(new List<long> { 7 }, result.SelectedKmsIds);
        }

        [Fact]
        public void From_WithOnlyInvalidSkillIds_ShouldReturnEmptyList()
        {
            // Arrange
            var dto = new AIChatHistorysDto { SelectedSkillIds = new List<string> { "x", "0", "-1" } };

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert
            Assert.Empty(result.SelectedSkillIds);
        }

        #endregion

        #region 去重：重复 Id 只保留一个且保序

        [Fact]
        public void From_WithDuplicateIds_ShouldDeduplicatePreservingOrder()
        {
            // Arrange
            var dto = new AIChatHistorysDto
            {
                SelectedKmsIds = new List<string> { "3", "1", "3", "2", "1" },
                SelectedSkillIds = new List<string> { "9", "9", "8" }
            };

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert
            Assert.Equal(new List<long> { 3, 1, 2 }, result.SelectedKmsIds);
            Assert.Equal(new List<long> { 9, 8 }, result.SelectedSkillIds);
        }

        #endregion

        #region 开关透传：忽略默认知识库 / 技能

        [Fact]
        public void From_ShouldCopyExcludeFlags()
        {
            // Arrange
            var dto = new AIChatHistorysDto
            {
                ExcludeDefaultKmss = true,
                ExcludeDefaultSkills = true,
                SelectedKmsIds = new List<string> { "5" },
                SelectedSkillIds = new List<string> { "6" }
            };

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert
            Assert.True(result.ExcludeDefaultKmss);
            Assert.True(result.ExcludeDefaultSkills);
            Assert.Equal(new List<long> { 5 }, result.SelectedKmsIds);
            Assert.Equal(new List<long> { 6 }, result.SelectedSkillIds);
        }

        [Fact]
        public void From_WithExcludeFlagsTrueButNoIds_ShouldKeepFlagsAndEmptyIds()
        {
            // Arrange：忽略默认 + 无追加 → 下游合并集合为空，本轮回退纯对话
            var dto = new AIChatHistorysDto
            {
                ExcludeDefaultKmss = true,
                ExcludeDefaultSkills = true
            };

            // Act
            var result = AIPerRoundOverride.From(dto);

            // Assert
            Assert.True(result.ExcludeDefaultKmss);
            Assert.True(result.ExcludeDefaultSkills);
            Assert.Empty(result.SelectedKmsIds);
            Assert.Empty(result.SelectedSkillIds);
        }

        #endregion
    }
}
