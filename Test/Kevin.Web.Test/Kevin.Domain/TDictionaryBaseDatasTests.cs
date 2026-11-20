using kevin.Domain.BaseDatas;

namespace Kevin.Unit.Test.Kevin.Domain
{
    /// <summary>
    /// 登录安全加固（R4）：字典种子数据单元测试。
    /// 覆盖 TDictionaryBaseDatas.Data 中 5 条 LoginSecurity 种子正确注入。
    /// </summary>
    public class TDictionaryBaseDatasTests
    {
        private const string TypeName = "LoginSecurity";

        private static List<kevin.Domain.Entities.TDictionary> GetLoginSecuritySeeds()
        {
            return TDictionaryBaseDatas.Data.Where(t => t.Type == TypeName).ToList();
        }

        [Fact]
        public void Data_ShouldContainExactlyFiveLoginSecuritySeeds()
        {
            // Act
            var seeds = GetLoginSecuritySeeds();

            // Assert
            Assert.Equal(5, seeds.Count);
        }

        [Theory]
        [InlineData("PasswordRegex", "^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$|^(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$")]
        [InlineData("AccountFailedCount", "5")]
        [InlineData("AccountLockMinutes", "15")]
        [InlineData("IpFailedCount", "50")]
        [InlineData("IpLockMinutes", "30")]
        public void Data_SeedKeyValue_ShouldMatchExpected(string key, string expectedValue)
        {
            // Act
            var seed = TDictionaryBaseDatas.Data.FirstOrDefault(t => t.Type == TypeName && t.Key == key);

            // Assert
            Assert.NotNull(seed);
            Assert.Equal(expectedValue, seed.Value);
        }

        [Fact]
        public void Data_AllLoginSecuritySeeds_ShouldBeSystemAndHaveSortOrder()
        {
            // Act
            var seeds = GetLoginSecuritySeeds();

            // Assert
            Assert.All(seeds, seed =>
            {
                Assert.True(seed.IsSystem);
                Assert.True(seed.Sort > 0);
                Assert.True(seed.CreateUserId > 0);
                Assert.True(seed.TenantId != 0);
            });
        }

        [Fact]
        public void Data_LoginSecuritySeeds_ShouldHaveDistinctSort()
        {
            // Act
            var seeds = GetLoginSecuritySeeds();

            // Assert
            Assert.Equal(seeds.Count, seeds.Select(s => s.Sort).Distinct().Count());
        }
    }
}
