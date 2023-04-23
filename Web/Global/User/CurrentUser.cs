using Kevin.Common.Kevin;
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
        public virtual Guid UserId { get => JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor).IsEmpty()?default:Guid.Parse(JwtToken.GetClaims(JwtKeinClaimTypes.UserId, _httpContextAccessor)); }

        public virtual string UserName => JwtToken.GetClaims(JwtKeinClaimTypes.Name, _httpContextAccessor);

        public virtual string TenantId => JwtToken.GetClaims(JwtKeinClaimTypes.TenantId, _httpContextAccessor);
    }
}
