using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.v1
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        public int GetId();
    }
    public class UserService : IUserService
    {
        public int GetId()
        {
            return 1;
        }
    }
}
