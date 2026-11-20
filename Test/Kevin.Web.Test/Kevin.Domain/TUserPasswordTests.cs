using kevin.Domain.Entities;
using Web.Global.Exceptions;

namespace Kevin.Unit.Test.Kevin.Domain
{
    /// <summary>
    /// 登录安全加固（R1）：密码复杂度校验单元测试。
    /// 覆盖 TUser.ChangePassword 内置默认正则「数字/大写/小写/特殊符号至少三类 + 长度>=8」。
    /// </summary>
    public class TUserPasswordTests
    {
        private const string ExpectMessage = "密码不符合复杂度要求";

        #region 成功路径：四种「至少三类」组合

        [Theory]
        [InlineData("Admin123")]        // 大写 + 小写 + 数字
        [InlineData("Admin@#!")]        // 大写 + 小写 + 特殊
        [InlineData("ADMIN123@")]       // 大写 + 数字 + 特殊
        [InlineData("admin123@")]       // 小写 + 数字 + 特殊
        public void ChangePassword_ValidCombinations_ShouldSetPasswordHash(string password)
        {
            // Arrange
            var user = new TUser();

            // Act
            user.ChangePassword(password);

            // Assert
            Assert.False(string.IsNullOrEmpty(user.PasswordHash));
            Assert.True(user.ValidatePassword(password));
        }

        [Theory]
        [InlineData("Admin123@!$%")]    // 四类全含（大写+小写+数字+特殊）
        [InlineData("abcdEFGH1234")]    // 三类：大写+小写+数字
        public void ChangePassword_AllFourCategories_ShouldPass(string password)
        {
            // Arrange
            var user = new TUser();

            // Act
            user.ChangePassword(password);

            // Assert
            Assert.True(user.ValidatePassword(password));
        }

        #endregion

        #region 失败路径：长度不足

        [Theory]
        [InlineData("Ab1")]             // 三类但长度 < 8
        [InlineData("Ab1@")]            // 四类但长度 < 8
        [InlineData("aA1!")]            // 四类但长度 4
        [InlineData("A1@#x")]           // 长度 5
        public void ChangePassword_TooShort_ShouldThrow(string password)
        {
            // Arrange
            var user = new TUser();

            // Act & Assert
            var ex = Assert.Throws<UserFriendlyException>(() => user.ChangePassword(password));
            Assert.Equal(ExpectMessage, ex.Message);
        }

        #endregion

        #region 失败路径：仅两类及以下

        [Theory]
        [InlineData("abcdefgh")]        // 仅小写
        [InlineData("ABCDEFGH")]        // 仅大写
        [InlineData("12345678")]        // 仅数字
        [InlineData("!@#$%^&*")]        // 仅特殊
        [InlineData("abcd1234")]        // 小写 + 数字（两类）
        [InlineData("ABCD1234")]        // 大写 + 数字（两类）
        [InlineData("abcd!@#$")]        // 小写 + 特殊（两类）
        [InlineData("ABCD!@#$")]        // 大写 + 特殊（两类）
        public void ChangePassword_TooFewCategories_ShouldThrow(string password)
        {
            // Arrange
            var user = new TUser();

            // Act & Assert
            var ex = Assert.Throws<UserFriendlyException>(() => user.ChangePassword(password));
            Assert.Equal(ExpectMessage, ex.Message);
        }

        #endregion

        #region 边界：空值 / 空白

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangePassword_EmptyOrWhitespace_ShouldThrow(string password)
        {
            // Arrange
            var user = new TUser();

            // Act & Assert
            var ex = Assert.Throws<UserFriendlyException>(() => user.ChangePassword(password));
            Assert.Equal(ExpectMessage, ex.Message);
        }

        [Fact]
        public void ChangePassword_Null_ShouldThrow()
        {
            // Arrange
            var user = new TUser();

            // Act & Assert
            var ex = Assert.Throws<UserFriendlyException>(() => user.ChangePassword(null!));
            Assert.Equal(ExpectMessage, ex.Message);
        }

        #endregion

        #region 自定义正则：passwordRegex 参数覆盖

        [Fact]
        public void ChangePassword_CustomRegexAllowsWeakPassword_WhenConfigured()
        {
            // Arrange：自定义宽松正则（任意非空），模拟字典配置放宽场景
            var user = new TUser();
            string weakPassword = "123";

            // Act
            user.ChangePassword(weakPassword, "^.+$");

            // Assert：自定义正则生效，弱密码被接受
            Assert.True(user.ValidatePassword(weakPassword));
        }

        [Fact]
        public void ChangePassword_CustomRegexRejectsMismatch_WhenConfigured()
        {
            // Arrange：自定义严格正则，仅允许纯数字
            var user = new TUser();

            // Act & Assert
            var ex = Assert.Throws<UserFriendlyException>(() => user.ChangePassword("abc", "^\\d+$"));
            Assert.Equal(ExpectMessage, ex.Message);
        }

        #endregion

        #region 回归：ValidatePassword 与 SHA256 一致性

        [Fact]
        public void ValidatePassword_CorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var user = new TUser();
            user.ChangePassword("Admin123");

            // Act
            var result = user.ValidatePassword("Admin123");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidatePassword_WrongPassword_ShouldReturnFalse()
        {
            // Arrange
            var user = new TUser();
            user.ChangePassword("Admin123");

            // Act
            var result = user.ValidatePassword("Admin124");

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
