using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Interfaces
{
    /// <summary>
    /// 用于AI工具的提供用户信息服务器接口
    /// </summary>
    public interface IAIToolUserInfoServer
    {
        public Task<string> GetUserIdAsync();
    }
}
