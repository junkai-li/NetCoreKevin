using kevin.AI.AgentFramework.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Interfaces
{
    /// <summary>
    /// 共享信息服务
    /// </summary>
    public interface IAIShareInfoService
    {
        /// <summary>
        /// 初始化数据 用于AI前传递数据
        /// </summary>
        /// <param name="data"></param>
        public void InitData(AIShareInfoDto data);

        /// <summary>
        /// 获取数据 用于AI后传递数据
        /// </summary>
        /// <returns></returns>
        public AIShareInfoDto GetData();

    }
}
