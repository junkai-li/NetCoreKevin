using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Global.User
{
    public interface ICurrentUser
    {
        Guid UserId { get; }

        string UserName { get; }
        Int32 TenantId { get;   }

    }
}
