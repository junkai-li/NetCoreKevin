using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.Domain.Interfaces.IServices.AI;
using Microsoft.Extensions.AI;

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

        private readonly IAgentHttpClientToolsService _agentHttpClientToolsService;

        private readonly IUserService _userService;

        private readonly IAIFileToolService _iAIFileToolService;

        private readonly IAIMsgService _IAIMsgService;
        public AIAgentToolSkillService(IKevinAITaskService kevinAITaskService, IAISkillToolBindIdService iAISkillToolBindIdService,
            IAISkillToolManagementService iAISkillToolManagementService, ICommonToolsService commonTools, IPythonToolsService pythonTools,
            IShellToolsService shellTools, IAgentHttpClientToolsService agentHttpClientToolsService, IUserService userService, IAIFileToolService iAIFileToolService,
            IAIMsgService iAIMsgService)
        {
            _kevinAITaskService = kevinAITaskService;
            _iAISkillToolBindIdService = iAISkillToolBindIdService;
            _iAISkillToolManagementService = iAISkillToolManagementService;
            _iCommonTools = commonTools;
            _iPythonTools = pythonTools;
            _iShellTools = shellTools;
            _agentHttpClientToolsService = agentHttpClientToolsService;
            _userService = userService;
            _iAIFileToolService = iAIFileToolService;
            _IAIMsgService = iAIMsgService;
        }
        private async Task<List<AITool>> GetAITools(object data, List<string> toolNames)
        {
            var aiTools = new List<AITool>();
            _kevinAITaskService.InitData(data);
            _iCommonTools.InitData(data);
            _iPythonTools.InitData(data);
            _iShellTools.InitData(data);
            _agentHttpClientToolsService.InitData(data);
            aiTools.Add(
                    AIFunctionFactory.Create(_iCommonTools.GetCurrentTime,
                    new AIFunctionFactoryOptions
                    {
                        Name = "GetCurrentTime",
                        Description = "获取当前时间信息，当用户询问当前时间、日期、星期，或需要基于当下时刻进行计算与判断时调用"
                    }
             ));
            aiTools.Add(
                   AIFunctionFactory.Create(_userService.GetCurrentUserInfo,
                   new AIFunctionFactoryOptions
                   {
                       Name = "GetCurrentUserInfo",
                       Description = "获取当前登录用户信息，当用户询问或者其他技能需要当前登录用户信息时调用"
                   }
             ));

            aiTools.Add(
                  AIFunctionFactory.Create(_iAIFileToolService.SaveFileContent,
                  new AIFunctionFactoryOptions
                  {
                      Name = "SaveFileContent",
                      Description = "保存文件内容并返回访问url，当用户需要将内容保存为文件时调用。"
                  }
            ));
            aiTools.Add(
                AIFunctionFactory.Create(_IAIMsgService.SendDDToMyMsg,
                new AIFunctionFactoryOptions
                {
                    Name = "SendDDToMyMsg",
                    Description = "发送消息到（当前用户/我/自己）钉钉，当用户需要发送钉钉消息到（当前用户/我/自己）时调用。"
                }
          ));
            foreach (var item in toolNames)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    switch (item)
                    {
                        case "AgentHttpClientTools.GetAsync":
                            aiTools.Add(
                                AIFunctionFactory.Create(_agentHttpClientToolsService.GetAsync,
                                new AIFunctionFactoryOptions
                                {
                                    Name = "GetAsync",
                                    Description = "通用 HTTP 工具 发送 GET 请求"
                                }
                            ));
                            break;
                        case "AgentHttpClientTools.PostAsync":
                            aiTools.Add(
                                AIFunctionFactory.Create(_agentHttpClientToolsService.PostAsync,
                                new AIFunctionFactoryOptions
                                {
                                    Name = "PostAsync",
                                    Description = "通用 HTTP 工具 发送 POST 请求"
                                }
                            ));
                            break;
                        case "AgentHttpClientTools.PutAsync":
                            aiTools.Add(
                                AIFunctionFactory.Create(_agentHttpClientToolsService.PutAsync,
                                new AIFunctionFactoryOptions
                                {
                                    Name = "PutAsync",
                                    Description = "通用 HTTP 工具 发送 PUT 请求"
                                }
                            ));
                            break;
                        case "AgentHttpClientTools.DeleteAsync":
                            aiTools.Add(
                                AIFunctionFactory.Create(_agentHttpClientToolsService.DeleteAsync,
                                new AIFunctionFactoryOptions
                                {
                                    Name = "DeleteAsync",
                                    Description = "通用 HTTP 工具 发送 DELETE 请求"
                                }
                            ));
                            break;
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

                        case "AIMsgService.SendDDToUserMsg":
                            aiTools.Add(
                            AIFunctionFactory.Create(_IAIMsgService.SendDDToUserMsg,
                            new AIFunctionFactoryOptions { Name = "SendDDToUserMsg", Description = "发送钉钉消息给其他用户， 用于把消息发送给指定用户的钉钉账户。以 ❌ 开头的错误信息。" }
                        ));
                            break;
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
