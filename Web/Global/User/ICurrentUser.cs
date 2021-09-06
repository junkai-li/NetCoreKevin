using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Global.User
{
    public interface ICurrentUser
    {
            Guid UserId { get;}

            Guid TokenId { get;}
    }
}
