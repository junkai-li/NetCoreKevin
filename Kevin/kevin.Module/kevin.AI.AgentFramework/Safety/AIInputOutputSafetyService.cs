using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces.Safety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Safety
{
    public class AIInputOutputSafetyService : IAIInputOutputSafetyService
    {
        public Task<(bool, SafetyDto)> CheckAIOutput(string text)
        {
            return Task.FromResult((true, new SafetyDto()));
        }

        public Task<(bool, SafetyDto)> CheckUserInput(string text)
        {
            return Task.FromResult((true, new SafetyDto()));
        }
    }
}
