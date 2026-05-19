using HttpMataki.NET.Auto;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.AI.AgentFramework.SkillClass;
using kevin.AI.AgentFramework.Tools;
using Kevin.AI.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using OpenAI;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
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
        /// <param name="data">传递自定义数据</param>
        /// <param name="isStreame"></param>
        /// <param name="streameCallback"></param>
        /// <returns></returns>
        public async Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(IServiceProvider _serviceProvider, string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions, object data = default, bool isOpenTaskTool = true, bool isStreame = false, Action<string> streameCallback = default)
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
                    chatClientAgentOptions.ChatOptions.Tools ??= new List<AITool>();
                    var tools = await new KevinAIAllTools().GetKevinAIAllTools(_serviceProvider, data, isOpenTaskTool);
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
                                        //.UsePromptTemplate(UseSkillPromptTemplate.UseSkillPrompt)
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
