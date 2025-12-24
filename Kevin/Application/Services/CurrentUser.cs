using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Dtos.User;
using kevin.Permission.Permission.Action;
using Kevin.Common.App;
using Kevin.log4Net;
using Kevin.Models.JwtBearer;
using Kevin.SnowflakeId.Service;
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

        public async Task<List<long>> GetModuleDataPermissionsUserIds()
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
    }
}
