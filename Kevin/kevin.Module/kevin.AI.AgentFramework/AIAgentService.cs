using HttpMataki.NET.Auto;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.AI.AgentFramework.SkillClass;
using kevin.AI.AgentFramework.Tools;
using Kevin.AI.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using OpenAI;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Reflection;
using kevin.AI.AgentFramework.Interfaces;
namespace kevin.AI.AgentFramework
{
    /// <summary>
    /// AI服务
    /// </summary>
    public class AIAgentService : IAIAgentService
    {
        public AISetting aISetting { get; set; } = new AISetting();
        private readonly ILogger<AIAgentService> Ailogger;
        public AIAgentService(ILogger<AIAgentService> _logger, IOptionsMonitor<AISetting> config)
        {
            aISetting = config.CurrentValue;
            Ailogger = _logger;
        }
        /// <summary>
        /// 智能体转换为McpServerTool
        /// </summary>
        /// <param name="aIAgent">智能体</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public McpServerTool AIAgentAsMcpServerTool(AIAgent aIAgent)
        {
            return McpServerTool.Create(aIAgent.AsAIFunction());
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public IChatClient GetChatClient(string url, string model, string keySecret)
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            openAIClientOptions.NetworkTimeout = TimeSpan.FromMinutes(10);// 设置网络超时时间为10分钟，适用于可能需要较长时间处理的请求
            var ai = new OpenAIClient(new ApiKeyCredential(model), openAIClientOptions);
            return ai.GetChatClient(keySecret).AsIChatClient();
        }
        /// <summary>
        /// 创建代理并发送消息
        /// </summary>
        /// <param name="_serviceProvider"></param>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <param name="keySecret"></param>
        /// <param name="chatClientAgentOptions"></param>
        /// <param name="isStreame"></param>
        /// <param name="streameCallback"></param>
        /// <returns></returns>
        public async Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(IServiceProvider _serviceProvider, string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions, bool isStreame = false, Action<string> streameCallback = default)
        {
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StartInterception();
            }
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri(url),
                NetworkTimeout = TimeSpan.FromMinutes(aISetting.NetworkTimeout),// 设置网络超时时间为10分钟，适用于可能需要较长时间处理的请求
                RetryPolicy = new ClientRetryPolicy(maxRetries: aISetting.MaxRetries)//重试次数和延迟
                {
                    // 可自定义延迟，默认指数退避
                }
            };
            // 当无 keySecret（本地模型无鉴权）时，尝试使用不带凭据的客户端；若构造失败则给出明确异常提示 
            var ai = new OpenAIClient(new ApiKeyCredential(string.IsNullOrWhiteSpace(keySecret) ? "local" : keySecret), openAIClientOptions);

            #region AI工具
            if (aISetting.IsAITools)
            {
                if (chatClientAgentOptions.ChatOptions != default)
                {
                    var service = _serviceProvider.GetService<IAIToolUserInfoServer>();
                    var userId = service is not null ? (await service.GetUserIdAsync()) ?? "" : "";

                    chatClientAgentOptions.ChatOptions.Tools ??= new List<AITool>();
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
                    var iKevinBasicAI = _serviceProvider.GetService<IKevinBasicAI>();
                    if (iKevinBasicAI != default)
                    {
                        tools.Add(
                           AIFunctionFactory.Create(() => iKevinBasicAI.GetNetCoreKevinInfo(userId),
                           new AIFunctionFactoryOptions { Name = "GetNetCoreKevinInfo", Description = "获取NetCoreKevin框架的介绍信息" }
                       ));
                    }
                    // 🔑 能力层：工具
                    foreach (var item in tools)
                    {
                        chatClientAgentOptions.ChatOptions.Tools.Add(item);
                    }
                }
            }
            else
            {
                if (chatClientAgentOptions.ChatOptions != default)
                {
                    chatClientAgentOptions.ChatOptions.Tools = new List<AITool>();
                }

            }
            #endregion

            #region AI技能
            if (aISetting.IsAISkills)
            {
                if (chatClientAgentOptions.AIContextProviders == default)
                {

#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                    var skill = new UnitConverterSkill();
                    var skillGetWeather = new GetWeatherSkill();
                    var skillsProvider = new AgentSkillsProviderBuilder()
                                         .UseFileScriptRunner(PySubprocessScriptRunner.StaticRunAsync)
                                        .UseFileSkill(Path.Combine(AppContext.BaseDirectory + "/Skills", "all-skills"),
                                        new AgentFileSkillsSourceOptions
                                        {
                                            AllowedScriptExtensions = [".py", ".sh", ".ps1", ".sh"],
                                            ScriptDirectories = ["scripts", "tools", "templates"],
                                        })
                                        .UseSkill(skill)
                                        .UseSkill(skillGetWeather)
                                        .UsePromptTemplate(UseSkillPromptTemplate.UseSkillPrompt)
                                        .UseOptions(options => options.DisableCaching = true)
                                        .Build();
#pragma warning restore MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。 
                    chatClientAgentOptions.AIContextProviders = [skillsProvider];
                    Console.WriteLine();
                }
            }
            else
            {
                chatClientAgentOptions.AIContextProviders = default;
            }
            #endregion 
            var aiAgent = ai.GetChatClient(model).AsIChatClient().AsAIAgent(chatClientAgentOptions);
            var reslut = new AgentResponse();
            var resultText = string.Empty;
            if (isStreame)
            {
                if (streameCallback != default)
                {
                    await foreach (var update in aiAgent.RunStreamingAsync(msg))
                    {
                        if (!string.IsNullOrEmpty(update.Text))
                        {
                            streameCallback.Invoke(update.Text);
                            resultText += update.Text;
                        }
                    }
                }
            }
            else
            {
                reslut = await aiAgent.RunAsync(msg);
                resultText = reslut.Text;
            }
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StopInterception();
            }
            return (aiAgent, resultText);
        }
    }
}
