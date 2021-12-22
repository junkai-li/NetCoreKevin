using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace Web.Global.User
{ 
    public class CurrentUser : ICurrentUser
    {
        public virtual Guid UserId { get => Libraries.Verify.JwtToken.GetClaims("userId").IsEmpty()?default:Guid.Parse(Libraries.Verify.JwtToken.GetClaims("userId")); }
        public  virtual Guid TokenId { get => Libraries.Verify.JwtToken.GetClaims("TokenId").IsEmpty() ? default : Guid.Parse(Libraries.Verify.JwtToken.GetClaims("TokenId")); }
    }
}
