using kevin.AI.AgentFramework.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Interfaces.Safety
{
    /// <summary>
    /// AI输入输出安全检查服务
    /// </summary>
    public interface IAIInputOutputSafetyService
    {
        /// <summary>
        /// 用户输入检查安全 返回是否安全和安全详情
        /// </summary>
        /// <param name="text">输入文本</param>    
        /// <returns></returns>
        public Task<(bool, SafetyDto)> CheckUserInput(string text);

        /// <summary>
        /// 检查AI返回是否安全 返回是否安全和安全详情
        /// </summary>
        /// <param name="text">输出文本</param>    
        /// <returns></returns>
        public Task<(bool, SafetyDto)> CheckAIOutput(string text); 
    }
}
