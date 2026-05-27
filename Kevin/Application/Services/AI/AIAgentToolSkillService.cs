using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.Tools;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Application.Services.AI
{
    public class AIAgentToolSkillService : BaseService, IAIAgentToolSkillService
    {
        private readonly IKevinAITaskService _kevinAITaskService;

        private readonly IAISkillToolBindIdService _iAISkillToolBindIdService;

        private readonly IAISkillToolManagementService _iAISkillToolManagementService;
        public AIAgentToolSkillService(IKevinAITaskService kevinAITaskService, IAISkillToolBindIdService iAISkillToolBindIdService, IAISkillToolManagementService iAISkillToolManagementService)
        {
            _kevinAITaskService = kevinAITaskService;
            _iAISkillToolBindIdService = iAISkillToolBindIdService;
            _iAISkillToolManagementService = iAISkillToolManagementService;
        }
        private async Task<List<AITool>> GetAITools(object data, List<string> toolNames)
        {
            var aiTools = new List<AITool>();
            _kevinAITaskService.InitData(data);
            foreach (var item in toolNames)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (SysTools.Tools.ContainsKey(item))
                    {
                        aiTools.Add(SysTools.Tools[item]);//静态工具
                    }
                    else
                    {
                        switch (item)
                        {
                            case "iKevinAITasksService.AddOrUpdateCronTask":
                                aiTools.Add(
                                    AIFunctionFactory.Create(_kevinAITaskService.AddOrUpdateCronTask,
                                    new AIFunctionFactoryOptions { Name = "AddOrUpdateCronTask", Description = "创建或更新一个周期性自动任务" }
                                ));
                                break;
                            case "iKevinAITasksService.RemoveCronTask":
                                aiTools.Add(
                                    AIFunctionFactory.Create(_kevinAITaskService.RemoveCronTask,
                                    new AIFunctionFactoryOptions { Name = "RemoveCronTask", Description = "移除周期性任务" }
                                ));
                                break;
                            case "iKevinAITasksService.TriggerCronTask":
                                aiTools.Add(
                                    AIFunctionFactory.Create(_kevinAITaskService.TriggerCronTask,
                                    new AIFunctionFactoryOptions { Name = "TriggerCronTask", Description = "立即触发某个周期性任务一次" }
                                ));
                                break;
                            case "iKevinAITasksService.GetTaskList":
                                aiTools.Add(
                                AIFunctionFactory.Create(_kevinAITaskService.GetTaskList,
                                new AIFunctionFactoryOptions { Name = "GetTaskList", Description = "获取我的所有周期性任务列表" }
                            ));
                                break;
                        }
                    }
                }
            }
            return aiTools;
        }

        public async Task<List<string>> GetAIAgentSkillsAsync(object data, string agentId)
        {
            var agentBindIds = (await _iAISkillToolBindIdService.GetListById(agentId)).Select(t => t.AISkillToolManagementId).ToList();
            var skills = (await _iAISkillToolManagementService.GetAllSkills()).Where(t => agentBindIds.Contains(t.Id)).ToList();
            return skills.Where(t => agentBindIds.Contains(t.Id)).Select(t => t.Name).ToList();
        }

        public async Task<List<AITool>> GetAIAgentToolsAsync(object data, string agentId)
        {
            var aiTools = new List<AITool>();
            var agentBindIds = (await _iAISkillToolBindIdService.GetListById(agentId)).Select(t => t.AISkillToolManagementId).ToList();
            var tools = (await _iAISkillToolManagementService.GetAllTools()).Where(t => agentBindIds.Contains(t.Id)).ToList();
            return await GetAITools(data, tools.Select(t => t.ClassMethod ?? "").ToList());
        }

        public async Task<List<string>> GetAllAIAgentSkillsAsync(object data)
        {
            return (await _iAISkillToolManagementService.GetAllSkills()).Select(t => t.Name).ToList();
        }

        public async Task<List<AITool>> GetAllAIAgentToolsAsync(object data)
        {
            var tools = (await _iAISkillToolManagementService.GetAllTools());
            return await GetAITools(data, tools.Select(t => t.ClassMethod ?? "").ToList());
        }

        public async Task<List<string>> GetUserAIAgentSkillsAsync(object data, string agentId, string userId)
        {
            var agentBindIds = (await _iAISkillToolBindIdService.GetListById(agentId)).Select(t => t.AISkillToolManagementId).ToList();
            var skills = (await _iAISkillToolManagementService.GetAllSkills()).Where(t => agentBindIds.Contains(t.Id)).ToList();
            return skills.Where(t => agentBindIds.Contains(t.Id)).Select(t => t.Name).ToList();
        }

        public async Task<List<AITool>> GetUserAIAgentToolsAsync(object data, string agentId, string userId)
        {
            var aiTools = new List<AITool>();
            var agentBindIds = (await _iAISkillToolBindIdService.GetListById(agentId)).Select(t => t.AISkillToolManagementId).ToList();
            var tools = (await _iAISkillToolManagementService.GetAllTools()).Where(t => agentBindIds.Contains(t.Id)).ToList();
            return await GetAITools(data, tools.Select(t => t.ClassMethod ?? "").ToList());
        }
    }
}
