using FakeItEasy;
using kevin.Application;
using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Interfaces.IServices.Organizational;
using kevin.Permission.Interfaces;
using Kevin.Authentication.Jwt.IService;
using Kevin.Common.Helper;
using Microsoft.AspNetCore.Http;
using Web.Global.Exceptions;
using Web.Global.User;

namespace Kevin.Unit.Kevin.Services
{
    /// <summary>
    /// 登录链路回归测试（TASK-04）：
    /// 覆盖 UserService.LoginUser 成功分支写入 RecentLoginTime/UpdateTime、失败分支保留 FailedCount、
    /// 超阈值写入 LockoutEnd、锁定拦截与解锁后重登等行为。
    /// </summary>
    public class UserServiceLoginUserTests
    {
        /// <summary>测试用明文密码</summary>
        private const string Password = "Admin123";

        /// <summary>字典 LoginSecurity.AccountFailedCount 的测试值</summary>
        private const int FailedThreshold = 5;

        /// <summary>字典 LoginSecurity.AccountLockMinutes 的测试值</summary>
        private const int LockMinutes = 15;

        #region 成功分支：写入最近登录时间 + 计数清零

        [Fact]
        public void LoginUser_WithCorrectPassword_ShouldWriteRecentLoginTimeAndUpdateTime()
        {
            // Arrange：账号此前有 3 次失败记录
            var user = CreateUser(failedCount: 3);
            var ctx = CreateContext(user);
            var before = DateTime.Now;

            // Act
            var result = ctx.Service.LoginUser(user.Name!, Password, user.TenantId, "");

            // Assert
            var after = DateTime.Now;
            Assert.NotNull(result);
            Assert.Equal(0, user.FailedCount);
            Assert.Null(user.LockoutEnd);
            Assert.NotNull(user.RecentLoginTime);
            Assert.InRange(user.RecentLoginTime!.Value, before, after);
            Assert.Equal(user.RecentLoginTime, user.UpdateTime);
            Assert.Equal(1, ctx.SaveChangesCount);
        }

        [Fact]
        public void LoginUser_WithCorrectPassword_ShouldCommitAllFourFieldsInSingleSaveChanges()
        {
            // Arrange：失败计数已归零前的状态
            var user = CreateUser(failedCount: 4, lockoutEnd: DateTime.Now.AddMinutes(-1));
            var ctx = CreateContext(user);

            // Act
            ctx.Service.LoginUser(user.Name!, Password, user.TenantId, "");

            // Assert：仅提交一次，且提交瞬间四项变更均已生效（无"计数已清但时间未写"的中间态）
            Assert.Equal(1, ctx.SaveChangesCount);
            Assert.Equal(0, ctx.FailedCountAtSave);
            Assert.Null(ctx.LockoutEndAtSave);
            Assert.NotNull(ctx.RecentLoginTimeAtSave);
            Assert.NotNull(ctx.UpdateTimeAtSave);
            Assert.Equal(ctx.RecentLoginTimeAtSave, ctx.UpdateTimeAtSave);
        }

        [Fact]
        public void LoginUser_WithPasswordHash_ShouldAlsoWriteRecentLoginTime()
        {
            // Arrange：小程序 GetTokenBySms 走 passwordHash 直连校验分支
            var user = CreateUser();
            var ctx = CreateContext(user);
            var before = DateTime.Now;

            // Act
            ctx.Service.LoginUser(user.Name!, "", user.TenantId, user.PasswordHash!);

            // Assert
            Assert.NotNull(user.RecentLoginTime);
            Assert.InRange(user.RecentLoginTime!.Value, before, DateTime.Now);
        }

        #endregion

        #region 失败分支：累计计数 + 超阈值锁定（保留 FailedCount）

        [Theory]
        [InlineData(0, false)] // 0 -> 1
        [InlineData(1, false)] // 1 -> 2
        [InlineData(4, false)] // 4 -> 5，等于阈值不锁定
        [InlineData(5, true)]  // 5 -> 6，超过阈值锁定
        [InlineData(9, true)]  // 9 -> 10，持续锁定
        public void LoginUser_WithWrongPassword_ShouldIncrementFailedCountAndLockOnlyOverThreshold(
            int initialFailedCount, bool expectLocked)
        {
            // Arrange
            var user = CreateUser(failedCount: initialFailedCount);
            var ctx = CreateContext(user);
            var before = DateTime.Now;

            // Act
            var ex = Assert.Throws<UserFriendlyException>(
                () => ctx.Service.LoginUser(user.Name!, "WrongPwd123", user.TenantId, ""));

            // Assert
            Assert.Equal("账户或密码错误", ex.Message);
            Assert.Equal(1, ctx.SaveChangesCount);
            // 关键回归点：失败计数为累计值，锁定时不清零
            Assert.Equal(initialFailedCount + 1, user.FailedCount);
            if (expectLocked)
            {
                Assert.NotNull(user.LockoutEnd);
                Assert.InRange(
                    user.LockoutEnd!.Value,
                    before.AddMinutes(LockMinutes),
                    DateTime.Now.AddMinutes(LockMinutes));
            }
            else
            {
                Assert.Null(user.LockoutEnd);
            }
        }

        [Fact]
        public void LoginUser_WhenAccountLocked_ShouldThrowLockedMessageWithoutChangingCounter()
        {
            // Arrange：锁定截止时间在未来
            var user = CreateUser(failedCount: 6, lockoutEnd: DateTime.Now.AddMinutes(10));
            var ctx = CreateContext(user);

            // Act
            var ex = Assert.Throws<UserFriendlyException>(
                () => ctx.Service.LoginUser(user.Name!, Password, user.TenantId, ""));

            // Assert：锁定拦截发生在计数逻辑之前，不落库、不改计数
            Assert.Equal("账号已被锁定，请稍后再试", ex.Message);
            Assert.Equal(6, user.FailedCount);
            Assert.Equal(0, ctx.SaveChangesCount);
        }

        [Fact]
        public void LoginUser_WhenLockoutExpired_ShouldAllowLoginAndResetCounters()
        {
            // Arrange：锁定已过期，累计失败次数仍为 6
            var user = CreateUser(failedCount: 6, lockoutEnd: DateTime.Now.AddMinutes(-1));
            var ctx = CreateContext(user);

            // Act
            var result = ctx.Service.LoginUser(user.Name!, Password, user.TenantId, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, user.FailedCount);
            Assert.Null(user.LockoutEnd);
            Assert.NotNull(user.RecentLoginTime);
        }

        [Fact]
        public void LoginUser_WhenAccountNotExist_ShouldThrowCredentialError()
        {
            // Arrange：库中无匹配账号
            var ctx = CreateContext();

            // Act
            var ex = Assert.Throws<UserFriendlyException>(
                () => ctx.Service.LoginUser("not-exist", Password, 1, ""));

            // Assert
            Assert.Equal("账户或密码错误", ex.Message);
            Assert.Equal(0, ctx.SaveChangesCount);
        }

        #endregion

        #region R-01 修复回归：禁用账号 + 正确密码

        /// <summary>
        /// R-01 修复回归：Status=false 且密码正确时必须登录失败。
        /// 修复内容：LoginUser 构造 dtoUser 时映射 Status，并将 LoginUserCheck() 前移到 SaveChanges 之前，
        /// 禁用账号不抛"用户已失效"以外的任何成功路径：不刷新 RecentLoginTime、不清失败计数、不触发 SaveChanges。
        /// </summary>
        [Fact]
        public void LoginUser_DisabledAccountWithCorrectPassword_ShouldThrowAndNotPersist()
        {
            // Arrange
            var user = CreateUser(failedCount: 2);
            user.Status = false;
            var ctx = CreateContext(user);

            // Act
            Exception? exception = null;
            try
            {
                ctx.Service.LoginUser(user.Name!, Password, user.TenantId, "");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert：抛"用户已失效"，且不落库（不刷新登录时间、不清计数、不清锁定）
            Assert.NotNull(exception);
            Assert.Equal("用户已失效", exception!.Message);
            Assert.Equal(0, ctx.SaveChangesCount);
            Assert.Null(user.RecentLoginTime);
            Assert.Equal(2, user.FailedCount);
        }

        #endregion

        #region 测试辅助

        private static TUser CreateUser(int failedCount = 0, DateTime? lockoutEnd = null)
        {
            return new TUser
            {
                Id = 10001,
                Name = "tester",
                NickName = "测试账号",
                Phone = "13800000000",
                Email = "tester@example.com",
                TenantId = 1,
                IsDelete = false,
                IsSystem = true,
                Status = true,
                PasswordHash = new HashHelper().SHA256Hash(Password),
                FailedCount = failedCount,
                LockoutEnd = lockoutEnd,
                CreateTime = DateTime.Now.AddDays(-30),
            };
        }

        /// <summary>
        /// 构造被测 UserService，除用户仓储外的依赖全部用 FakeItEasy 打桩；
        /// 记录 SaveChanges 调用次数与提交瞬间的实体快照，用于验证"同一事务提交"。
        /// </summary>
        private static LoginContext CreateContext(params TUser[] users)
        {
            var userList = users.ToList();
            var context = new LoginContext();

            var userRp = A.Fake<IUserRp>();
            A.CallTo(() => userRp.Query(A<bool>._, A<bool>._)).Returns(userList.AsQueryable());
            A.CallTo(() => userRp.SaveChanges()).Invokes(() =>
            {
                context.SaveChangesCount++;
                var entity = userList.FirstOrDefault();
                if (entity != null)
                {
                    context.FailedCountAtSave = entity.FailedCount;
                    context.LockoutEndAtSave = entity.LockoutEnd;
                    context.RecentLoginTimeAtSave = entity.RecentLoginTime;
                    context.UpdateTimeAtSave = entity.UpdateTime;
                }
            }).Returns(1);

            var dictionaryService = A.Fake<IDictionaryService>();
            A.CallTo(() => dictionaryService.GetValueByKey("LoginSecurity", "AccountFailedCount"))
                .Returns(FailedThreshold.ToString());
            A.CallTo(() => dictionaryService.GetValueByKey("LoginSecurity", "AccountLockMinutes"))
                .Returns(LockMinutes.ToString());

            var userBindRoleRp = A.Fake<IUserBindRoleRp>();
            A.CallTo(() => userBindRoleRp.Query(A<bool>._, A<bool>._))
                .Returns(new List<TUserBindRole>().AsQueryable());

            var httpContextAccessor = A.Fake<IHttpContextAccessor>();
            // 明确返回 null，使 BaseService 走"非 Http 请求上下文"分支，避免解析 RequestServices
            A.CallTo(() => httpContextAccessor.HttpContext).Returns((HttpContext?)null);
            context.Service = new UserService(
                httpContextAccessor,
                userRp,
                A.Fake<IWeiXinKeyRp>(),
                A.Fake<IFileRp>(),
                A.Fake<IRoleRp>(),
                userBindRoleRp,
                A.Fake<IPositionService>(),
                A.Fake<IUserBindPositionRp>(),
                A.Fake<IDepartmentService>(),
                A.Fake<IUserInfoRp>(),
                A.Fake<IKevinPermissionService>(),
                A.Fake<ITokenService>(),
                dictionaryService)
            {
                CurrentUser = A.Fake<ICurrentUser>(),
                HttpContextAccessor = httpContextAccessor,
                SnowflakeIdService = new global::Kevin.SnowflakeId.Service.SnowflakeIdService(),
            };
            return context;
        }

        private sealed class LoginContext
        {
            public UserService Service { get; set; } = null!;
            public int SaveChangesCount { get; set; }
            public int FailedCountAtSave { get; set; }
            public DateTime? LockoutEndAtSave { get; set; }
            public DateTime? RecentLoginTimeAtSave { get; set; }
            public DateTime? UpdateTimeAtSave { get; set; }
        }

        #endregion
    }
}
