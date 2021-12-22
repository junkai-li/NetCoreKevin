using Service.Services.v1._;
using Web.Base;

namespace Service.Services.v1
{
    public class UserService : BaseService,IUserService
    {
        public int GetId()
        {
            return 1;
        }
    }
}
