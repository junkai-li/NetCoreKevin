using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces.Safety;

namespace kevin.AI.AgentFramework.Safety
{
    public class SkillSafetyService : ISkillSafetyService
    {
        public Task<(bool, SafetyDto)> CheckSafety(string skillUrl)
        {
            return Task.FromResult((true, new SafetyDto()));
        }
    }
}
