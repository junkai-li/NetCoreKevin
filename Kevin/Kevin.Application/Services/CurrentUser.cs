using Kevin.Common.App;
using Kevin.Models.JwtBearer;
using Web.Global.User;

namespace kevin.Application
{
    public partial class CurrentUser : ICurrentUser
    {
        public IHttpContextAccessor _httpContextAccessor { get; set; }
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public virtual long UserId { get => JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor).IsEmpty() ? default : JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor).ToTryInt64(); }

        public virtual string UserName => JwtToken.GetClaims(JwtKeinClaimTypes.Name, _httpContextAccessor);

        public virtual Int32 TenantId => JwtToken.GetClaims(JwtKeinClaimTypes.TenantId, _httpContextAccessor).ToTryInt32();

        public virtual bool IsSuperAdmin => JwtToken.GetClaims(JwtKeinClaimTypes.issuperadmin, _httpContextAccessor).ToBoolean();
    }
}
