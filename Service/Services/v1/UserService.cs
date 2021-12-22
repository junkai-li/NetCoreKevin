using Service.Services.v1._;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.v1
{ 
    public class UserService : IUserService
    {
        public int GetId()
        {
            return 1;
        }
    }
}
