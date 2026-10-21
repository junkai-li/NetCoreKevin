using kevin.Domain.Share.Dtos.User;
using kevin.Permission.Interfaces;
using kevin.Permission.Permission.Action;
using Kevin.Common.App;
using Kevin.log4Net;
using Kevin.Models.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Web.Global.User;

namespace kevin.Application
{
    public partial class CurrentUser : ICurrentUser
    {
        public IHttpContextAccessor _httpContextAccessor { get; set; }

        public IServiceProvider _serviceProvider { get; set; }
        public CurrentUser(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }
        public virtual long UserId { get => JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor).IsEmpty() ? default : JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor).ToTryInt64(); }

        public virtual string UserName => JwtToken.GetClaims(JwtKeinClaimTypes.Name, _httpContextAccessor);

        public virtual Int32 TenantId => JwtToken.GetClaims(JwtKeinClaimTypes.TenantId, _httpContextAccessor).ToTryInt32();

        public virtual bool IsSuperAdmin => JwtToken.GetClaims(JwtKeinClaimTypes.issuperadmin, _httpContextAccessor).ToBoolean();

        /// <summary>
        /// 请求级数据权限缓存 Key（存放在 HttpContext.Items，随请求结束释放）
        /// </summary>
        private const string ModuleDataPermissionCacheKey = "Kevin_ModuleDataPermissionsUserIds";

        public async Task<List<long>> GetModuleDataPermissionsUserIds()
        {
            var cached = RequestCache.Get<List<long>>(_httpContextAccessor, ModuleDataPermissionCacheKey);
            if (cached != null)
            {
                return cached;
            }

            var result = await BuildModuleDataPermissionsUserIds();
            RequestCache.Set(_httpContextAccessor, ModuleDataPermissionCacheKey, result);
            return result;
        }

        /// <summary>
        /// 同步获取（供仓储构造查询表达式使用，Expression 内部无法 await）。
        /// 正常情况值已由 DataPermissionWarmupFilter 异步预热，这里只读缓存、不阻塞线程；
        /// 未预热时（未经过 MVC 管道，如直接调用 Hub/后台线程）才回退同步等待并记录告警。
        /// </summary>
        public List<long> GetModuleDataPermissionsUserIdsSync()
        {
            var cached = RequestCache.Get<List<long>>(_httpContextAccessor, ModuleDataPermissionCacheKey);
            if (cached != null)
            {
                return cached;
            }

            LogHelper<CurrentUser>.logger.Warn("数据权限未预热，回退为同步获取，请检查调用链路是否经过 MVC 管道");

            var result = BuildModuleDataPermissionsUserIds().GetAwaiter().GetResult();
            RequestCache.Set(_httpContextAccessor, ModuleDataPermissionCacheKey, result);
            return result;
        }

        /// <summary>
        /// 实际计算当前用户可查看的数据范围
        /// </summary>
        private async Task<List<long>> BuildModuleDataPermissionsUserIds()
        {
            try
            {
                //超级管理员/系统管理员拥有所有数据权限
                if (IsSuperAdmin)
                {
                    return new List<long>();
                }
                var userService = _serviceProvider.GetService<IUserService>();
                if (userService != default && _httpContextAccessor.HttpContext != default)
                {
                    var areaModule = IdentityVerification.GetAuthorizationAreaModule(_httpContextAccessor.HttpContext);
                    if (!string.IsNullOrEmpty(areaModule))
                    {
                        return await userService.GetModuleDataPermissionsUserIds(TenantId + "/" + areaModule);
                    }
                    else
                    {
                        //标识不受权限控制
                        return new List<long>();
                    }
                }
                else
                {
                    return new List<long>() { UserId };
                }
            }
            catch (Exception ex)
            {
                LogHelper<CurrentUser>.logger.Error("获取用户数据权限出错", ex);
                return new List<long>() { UserId };
            }

        }

        public virtual dtoUserInfo? UserInfo => _serviceProvider.GetService<IUserService>()?.GetUserInfo(UserId);

        public virtual List<long>? RoleIds => _serviceProvider.GetService<IKevinPermissionService>()?.GetUserRoleIds(UserId.ToString());
    }
}
