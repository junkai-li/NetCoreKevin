using FakeItEasy;
using kevin.Application;
using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Interfaces.IServices.Organizational;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.User;
using kevin.Permission.Interfaces;
using kevin.Share.Dtos;
using Kevin.Authentication.Jwt.IService;
using Microsoft.AspNetCore.Http;
using Web.Global.User;

namespace Kevin.Unit.Kevin.Services
{
    /// <summary>
    /// 用户列表查询投影回归测试（TASK-04）：
    /// 覆盖 GetUserList / GetSysUserList / GetSysUserWhereId 是否真实带出 FailedCount 与 LockoutEnd 字段。
    /// </summary>
    public class UserServiceUserListProjectionTests
    {
        [Fact]
        public void GetUserList_ShouldProjectFailedCountAndLockoutEnd()
        {
            // Arrange：锁定用户 + 正常用户各一个
            var lockedUser = CreateUser(10001, "locked", isSystem: false, failedCount: 6, lockoutEnd: DateTime.Now.AddMinutes(10));
            var normalUser = CreateUser(10002, "normal", isSystem: false, failedCount: 0, lockoutEnd: null);
            var service = CreateService(lockedUser, normalUser);

            // Act
            var result = service.GetUserList(new dtoPageData<dtoUser> { pageNum = 1, pageSize = 10 });

            // Assert
            Assert.Equal(2, result.total);
            Assert.Equal(2, result.data.Count);
            var locked = result.data.Single(t => t.Id == lockedUser.Id);
            Assert.Equal(6, locked.FailedCount);
            Assert.NotNull(locked.LockoutEnd);
            var normal = result.data.Single(t => t.Id == normalUser.Id);
            Assert.Equal(0, normal.FailedCount);
            Assert.Null(normal.LockoutEnd);
        }

        [Fact]
        public async Task GetSysUserList_ShouldProjectFailedCountAndLockoutEnd()
        {
            // Arrange
            var lockedUser = CreateUser(20001, "sysLocked", isSystem: true, failedCount: 9, lockoutEnd: DateTime.Now.AddMinutes(3));
            var normalUser = CreateUser(20002, "sysNormal", isSystem: true, failedCount: 2, lockoutEnd: null);
            var service = CreateService(lockedUser, normalUser);

            // Act
            var result = await service.GetSysUserList(new dtoPagePar<dtoUserPar> { pageNum = 1, pageSize = 10 });

            // Assert
            Assert.Equal(2, result.total);
            Assert.Equal(2, result.data.Count);
            var locked = result.data.Single(t => t.Id == lockedUser.Id);
            Assert.Equal(9, locked.FailedCount);
            Assert.NotNull(locked.LockoutEnd);
            var normal = result.data.Single(t => t.Id == normalUser.Id);
            Assert.Equal(2, normal.FailedCount);
            Assert.Null(normal.LockoutEnd);
            // 既有字段未被破坏
            Assert.Equal("sysNormal", normal.Name);
            Assert.True(normal.Status);
        }

        [Fact]
        public async Task GetSysUserWhereId_ShouldMapFailedCountAndLockoutEnd()
        {
            // Arrange：详情走 MapTo<dtoUser>，新字段应按同名自动映射
            var lockoutEnd = DateTime.Now.AddMinutes(8);
            var user = CreateUser(30001, "detail", isSystem: true, failedCount: 4, lockoutEnd: lockoutEnd);
            var service = CreateService(user);

            // Act
            var result = await service.GetSysUserWhereId(user.Id);

            // Assert
            Assert.Equal(4, result.FailedCount);
            Assert.NotNull(result.LockoutEnd);
            Assert.Equal(lockoutEnd.ToString("yyyyMMddHHmmss"), result.LockoutEnd!.Value.ToString("yyyyMMddHHmmss"));
        }

        #region 测试辅助

        private static TUser CreateUser(
            long id, string name, bool isSystem, int failedCount, DateTime? lockoutEnd)
        {
            return new TUser
            {
                Id = id,
                Name = name,
                NickName = name,
                Phone = "1380000" + (id % 10000).ToString("0000"),
                Email = name + "@example.com",
                TenantId = 1,
                IsDelete = false,
                IsSystem = isSystem,
                Status = true,
                FailedCount = failedCount,
                LockoutEnd = lockoutEnd,
                CreateTime = DateTime.Now.AddDays(-1),
            };
        }

        private static UserService CreateService(params TUser[] users)
        {
            var userList = users.ToList();

            var userRp = A.Fake<IUserRp>();
            A.CallTo(() => userRp.Query(A<bool>._, A<bool>._)).Returns(userList.AsQueryable());

            var fileRp = A.Fake<IFileRp>();
            A.CallTo(() => fileRp.Query(A<bool>._, A<bool>._)).Returns(new List<TFile>().AsQueryable());

            var userBindRoleRp = A.Fake<IUserBindRoleRp>();
            A.CallTo(() => userBindRoleRp.Query(A<bool>._, A<bool>._)).Returns(new List<TUserBindRole>().AsQueryable());

            var userBindPositionRp = A.Fake<IUserBindPositionRp>();
            A.CallTo(() => userBindPositionRp.Query(A<bool>._, A<bool>._)).Returns(new List<TUserBindPosition>().AsQueryable());

            var userInfoRp = A.Fake<IUserInfoRp>();
            A.CallTo(() => userInfoRp.Query(A<bool>._, A<bool>._)).Returns(new List<TUserInfo>().AsQueryable());

            var httpContextAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => httpContextAccessor.HttpContext).Returns((HttpContext?)null);

            return new UserService(
                httpContextAccessor,
                userRp,
                A.Fake<IWeiXinKeyRp>(),
                fileRp,
                A.Fake<IRoleRp>(),
                userBindRoleRp,
                A.Fake<IPositionService>(),
                userBindPositionRp,
                A.Fake<IDepartmentService>(),
                userInfoRp,
                A.Fake<IKevinPermissionService>(),
                A.Fake<ITokenService>(),
                A.Fake<IDictionaryService>())
            {
                CurrentUser = A.Fake<ICurrentUser>(),
                HttpContextAccessor = httpContextAccessor,
                SnowflakeIdService = new global::Kevin.SnowflakeId.Service.SnowflakeIdService(),
            };
        }

        #endregion
    }
}
