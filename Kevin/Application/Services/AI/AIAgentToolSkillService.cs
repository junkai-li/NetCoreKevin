using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.Tools;
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
        public AIAgentToolSkillService(IKevinAITaskService kevinAITaskService)
        {
            _kevinAITaskService = kevinAITaskService;
        }

        public async Task<List<string>> GetAIAgentSkillsAsync(object data, string agentId)
        {
            return new List<string>() { "/Skills/all-skills" };
        }

        public async Task<List<AITool>> GetAIAgentToolsAsync(object data, string agentId)
        {
            var tools = new List<AITool>() {
                    AIFunctionFactory.Create(AgentHttpClientTools.GetAsync,new AIFunctionFactoryOptions{ Name = "GetAsync",Description = "通用 HTTP 工具 发送 GET 请求" }),
                    AIFunctionFactory.Create(AgentHttpClientTools.PostAsync,new AIFunctionFactoryOptions{ Name = "PostAsync",Description = "通用 HTTP 工具 发送 POST 请求" }),
                    AIFunctionFactory.Create(AgentHttpClientTools.PutAsync,new AIFunctionFactoryOptions{ Name = "PutAsync",Description = "通用 HTTP 工具 发送 PUT 请求" }),
                    AIFunctionFactory.Create(AgentHttpClientTools.DeleteAsync,new AIFunctionFactoryOptions{ Name = "DeleteAsync",Description = "通用 HTTP 工具 发送 DELETE 请求" }),
                   AIFunctionFactory.Create(ShellTools.RunShell,new AIFunctionFactoryOptions{ Name = "RunShell",Description = "执行 Shell 命令。通过操作系统原生 Shell 执行命令(Windows 用 cmd也可以执行bash相关命令，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。" }),
                    AIFunctionFactory.Create(PythonTools.RunPythonPy,new AIFunctionFactoryOptions{ Name = "RunPythonPy",Description = "执行Python脚本。 可以帮助Skills工具执行scripts中含有后缀.py脚本的能力 通过PythonNet库来调用Python.py的脚本,并返回执行脚本结果 如果执行返回结果为空或者报错 可以使用RunShell来提取脚本代码然后自行调整定义main函数使用RunPythonCode来执行" }),
                    AIFunctionFactory.Create(PythonTools.RunPythonCode,new AIFunctionFactoryOptions{ Name = "RunPythonCode",Description = "执行Python代码。" }),
                    AIFunctionFactory.Create(CommonTools.GetRuntimePlatform,new AIFunctionFactoryOptions{ Name = "GetRuntimePlatform",Description = "获取系统。用于获取当前运行在什么系统平台上" }),
                    AIFunctionFactory.Create(CommonTools.GetDesktopPath,new AIFunctionFactoryOptions{ Name = "GetDesktopPath",Description = "获取当前系统桌面路径。 用于获取当前用户的桌面路径" }),
                    AIFunctionFactory.Create(CommonTools.WriteTextToDesktop,new AIFunctionFactoryOptions{ Name = "WriteTextToDesktop",Description = "输出文件到系统桌面。 用于把各种文件输出到桌面" }),
                    };
            _kevinAITaskService.InitData(data);
            tools.Add(
               AIFunctionFactory.Create(_kevinAITaskService.AddOrUpdateCronTask,
               new AIFunctionFactoryOptions { Name = "AddOrUpdateCronTask", Description = "创建或更新一个周期性自动任务" }
           ));
            tools.Add(
             AIFunctionFactory.Create(_kevinAITaskService.RemoveCronTask,
             new AIFunctionFactoryOptions { Name = "RemoveCronTask", Description = "移除周期性任务" }
         ));
            tools.Add(
             AIFunctionFactory.Create(_kevinAITaskService.TriggerCronTask,
             new AIFunctionFactoryOptions { Name = "TriggerCronTask", Description = "立即触发某个周期性任务一次" }
         ));
            tools.Add(
             AIFunctionFactory.Create(_kevinAITaskService.GetTaskList,
             new AIFunctionFactoryOptions { Name = "GetTaskList", Description = "获取我的所有周期性任务列表" }
         ));
            return tools;
        }

        public async Task<List<string>> GetAllAIAgentSkillsAsync(object data)
        {
            return new List<string>() { "/Skills/all-skills" };
        }

        public async Task<List<AITool>> GetAllAIAgentToolsAsync(object data)
        {
            return await GetAIAgentToolsAsync(data, "");
        }

        public async Task<List<string>> GetUserAIAgentSkillsAsync(object data, string agentId, string userId)
        {
            return new List<string>() { "/Skills/all-skills" };
        }

        public async Task<List<AITool>> GetUserAIAgentToolsAsync(object data, string agentId, string userId)
        {
            return await GetAIAgentToolsAsync(data, "");
        }
    }
}
