using Microsoft.Agents.AI;
using System.Text.Json;

namespace kevin.AI.AgentFramework.Interfaces
{
    public interface IPySubprocessScriptRunner
    {
        Task<object?> StaticRunAsync(
        AgentFileSkill skill,
        AgentFileSkillScript script,
        JsonElement? arguments,
        IServiceProvider? serviceProvider,
        CancellationToken cancellationToken);
    }
}
