using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.Interfaces.Tools;
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

        private readonly ICommonToolsService _iCommonTools;

        private readonly IPythonToolsService _iPythonTools;

        private readonly IShellToolsService _iShellTools;
        public AIAgentToolSkillService(IKevinAITaskService kevinAITaskService, IAISkillToolBindIdService iAISkillToolBindIdService, IAISkillToolManagementService iAISkillToolManagementService,
                ICommonToolsService commonTools, IPythonToolsService pythonTools, IShellToolsService shellTools
            )
        {
            _kevinAITaskService = kevinAITaskService;
            _iAISkillToolBindIdService = iAISkillToolBindIdService;
            _iAISkillToolManagementService = iAISkillToolManagementService;
            _iCommonTools = commonTools;
            _iPythonTools = pythonTools;
            _iShellTools = shellTools;
        }
        private async Task<List<AITool>> GetAITools(object data, List<string> toolNames)
        {
            var aiTools = new List<AITool>();
            _kevinAITaskService.InitData(data);
            _iCommonTools.InitData(data);
            _iPythonTools.InitData(data);
            _iShellTools.InitData(data);

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
                            case "ShellTools.RunShell":
                                aiTools.Add(
                                    AIFunctionFactory.Create(_iShellTools.RunShell,
                                    new AIFunctionFactoryOptions
                                    {
                                        Name = "RunShell",
                                        Description = "执行 Shell 命令。通过操作系统原生 Shell 执行命令(Windows 用 cmd也可以执行bash相关命令，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。"
                                    }
                                ));
                                break;
                            case "PythonTools.RunPythonPy":
                                aiTools.Add(
                                   AIFunctionFactory.Create(_iPythonTools.RunPythonPy,
                                   new AIFunctionFactoryOptions
                                   {
                                       Name = "RunPythonPy",
                                       Description = "执行Python脚本。 可以帮助Skills工具执行scripts中含有后缀.py脚本的能力 通过PythonNet库来调用Python.py的脚本,并返回执行脚本结果 如果执行返回结果为空或者报错 可以使用RunShell来提取脚本代码然后自行调整定义main函数使用RunPythonCode来执行"
                                   }
                               ));
                                break;
                            case "PythonTools.RunPythonCode":
                                aiTools.Add(
                                   AIFunctionFactory.Create(_iPythonTools.RunPythonCode,
                                   new AIFunctionFactoryOptions
                                   {
                                       Name = "RunPythonCode",
                                       Description = "执行Python代码。"
                                   }
                               ));
                                break;
                            case "CommonTools.GetRuntimePlatform":
                                aiTools.Add(
                                  AIFunctionFactory.Create(_iCommonTools.GetRuntimePlatform,
                                  new AIFunctionFactoryOptions
                                  {
                                      Name = "GetRuntimePlatform",
                                      Description = "获取系统。用于获取当前运行在什么系统平台上"
                                  }
                              ));
                                break;
                            case "CommonTools.GetDesktopPath":
                                aiTools.Add(
                                  AIFunctionFactory.Create(_iCommonTools.GetDesktopPath,
                                  new AIFunctionFactoryOptions
                                  {
                                      Name = "GetDesktopPath",
                                      Description = "获取当前系统桌面路径。 用于获取当前用户的桌面路径"
                                  }
                              ));
                                break;
                            case "CommonTools.WriteTextToDesktop":
                                aiTools.Add(
                                  AIFunctionFactory.Create(_iCommonTools.WriteTextToDesktop,
                                  new AIFunctionFactoryOptions
                                  {
                                      Name = "WriteTextToDesktop",
                                      Description = "输出文件到系统桌面。 用于把各种文件输出到桌面"
                                  }
                              ));
                                break;
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
