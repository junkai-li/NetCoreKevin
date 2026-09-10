using kevin.AI.AgentFramework.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Interfaces.Safety
{
    /// <summary>
    /// 技能安全检查服务
    /// </summary>
    public interface ISkillSafetyService
    {
        /// <summary>
        /// 检查安全 返回是否安全和安全详情
        /// </summary>
        /// <param name="skillUrl">skill地址 包含 SKILL.md 和相关文件的 ZIP</param>    
        /// <returns></returns>
        public Task<(bool, SafetyDto)> CheckSafety(string skillUrl);
    }
}
