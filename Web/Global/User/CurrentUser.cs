using Kevin.Models.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace Web.Global.User
{ 
    public class CurrentUser : ICurrentUser
    {
        public virtual Guid UserId { get => Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.UserId).IsEmpty()?default:Guid.Parse(Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.UserId)); }

        public virtual string UserName => Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.Name);

        public virtual string TenantId => Libraries.Verify.JwtToken.GetClaims(JwtKeinClaimTypes.TenantId);
    }
}
