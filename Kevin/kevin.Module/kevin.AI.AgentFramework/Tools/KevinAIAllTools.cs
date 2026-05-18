using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Tasks;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Tools
{
    public class KevinAIAllTools
    {
        /// <summary>
        /// 获取kevin框架AI框架内置的所有工具列表，包含工具名称、描述、输入参数等信息，供智能体调用使用
        /// </summary>
        /// <returns></returns>
        public async Task<List<AITool>> GetKevinAIAllTools(IServiceProvider serviceProvider, object data,bool isOpenTaskTool = true)
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
            var iKevinBasicAI = serviceProvider.GetService<IKevinBasicAI>();
            if (iKevinBasicAI != default)
            {
                tools.Add(
                   AIFunctionFactory.Create(() => iKevinBasicAI.GetNetCoreKevinInfo(),
                   new AIFunctionFactoryOptions { Name = "GetNetCoreKevinInfo", Description = "获取NetCoreKevin框架的介绍信息" }
               ));
            }
            var aiToolUserInfoServer = serviceProvider.GetService<IAIToolUserInfoServer>();
            if (aiToolUserInfoServer != default)
            {
                tools.Add(
                   AIFunctionFactory.Create(() => aiToolUserInfoServer.GetUserInfo(),
                   new AIFunctionFactoryOptions { Name = "GetUserInfo", Description = "获取当前登录用户信息，获取当前用户信息" }
               ));
                tools.Add(
             AIFunctionFactory.Create(() => aiToolUserInfoServer.GetUserIdAsync(),
             new AIFunctionFactoryOptions { Name = "GetUserIdAsync", Description = "获取当前登录用户Id" }
         ));
            }
            if (isOpenTaskTool)
            {
                var iKevinAITasksService = serviceProvider.GetService<IKevinAITaskService>();
                if (iKevinAITasksService != default)
                {
                    iKevinAITasksService.InitData(data);
                    tools.Add(
                       AIFunctionFactory.Create(iKevinAITasksService.AddOrUpdateCronTask,
                       new AIFunctionFactoryOptions { Name = "AddOrUpdateCronTask", Description = "创建或更新一个周期性自动任务" }
                   ));
                    tools.Add(
                     AIFunctionFactory.Create(iKevinAITasksService.RemoveCronTask,
                     new AIFunctionFactoryOptions { Name = "RemoveCronTask", Description = "移除周期性任务" }
                 ));
                    tools.Add(
                     AIFunctionFactory.Create(iKevinAITasksService.TriggerCronTask,
                     new AIFunctionFactoryOptions { Name = "TriggerCronTask", Description = "立即触发某个周期性任务一次" }
                 ));
                    tools.Add(
                     AIFunctionFactory.Create(iKevinAITasksService.GetTaskList,
                     new AIFunctionFactoryOptions { Name = "GetTaskList", Description = "获取我的所有周期性任务列表" }
                 ));
                }
            } 
            return tools;
        }
    }
}
