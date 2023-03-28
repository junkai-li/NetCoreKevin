using Kevin.Models.JwtBearer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace Web.Global.User
{
    public class CurrentUser : ICurrentUser
    {
        public IHttpContextAccessor _httpContextAccessor { get; set; }
        public CurrentUser(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor= httpContextAccessor;
        }
        public virtual Guid UserId { get => Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor).IsEmpty()?default:Guid.Parse(Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor)); }

        public virtual string UserName => Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.Name, _httpContextAccessor);

        public virtual string TenantId => Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.TenantId, _httpContextAccessor);
    }
}
