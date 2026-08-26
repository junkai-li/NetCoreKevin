using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Msg;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.Tools;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.RepositorieRps.Repositories.AI;
using Kevin.Common.App;
using Kevin.Common.Extension;
using Kevin.log4Net;
using Microsoft.Extensions.AI;
using Microsoft.IdentityModel.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System;
using System.Text.Json;

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

        private readonly IAuthorizedToolsService _authorizedToolsService;

        private readonly IAIJsonLogService _aIJsonLogService;

        private readonly IWebSearchEngine _webSearchEngine;

        private readonly IAIAgentMemoryService _aiAgentMemoryService;

        private readonly IAuthorizeService _authorizeService;
        public AIAgentToolSkillService(IKevinAITaskService kevinAITaskService, IAISkillToolBindIdService iAISkillToolBindIdService,
            IAISkillToolManagementService iAISkillToolManagementService, ICommonToolsService commonTools, IPythonToolsService pythonTools,
            IShellToolsService shellTools, IAgentHttpClientToolsService agentHttpClientToolsService, IUserService userService, IAIJsonLogService aIJsonLogService,
            IAIFileToolService iAIFileToolService, IAIMsgService iAIMsgService, IAuthorizedToolsService authorizedToolsService, IWebSearchEngine webSearchEngine, IAIAgentMemoryService aiAgentMemoryService, IAuthorizeService authorizeService, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
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
            _authorizedToolsService = authorizedToolsService;
            _aIJsonLogService = aIJsonLogService;
            _webSearchEngine = webSearchEngine;
            _aiAgentMemoryService = aiAgentMemoryService;
            _authorizeService = authorizeService;
        }
        private async Task<List<AITool>> GetAITools(object data, List<string> toolNames)
        {
            var aiTools = new List<AITool>();
            _kevinAITaskService.InitData(data);
            _iCommonTools.InitData(data);
            _iPythonTools.InitData(data);
            _iShellTools.InitData(data);
            _agentHttpClientToolsService.InitData(data);
            _authorizedToolsService.InitData(data);
            _IAIMsgService.InitData(data);
            _aIJsonLogService.InitData(data);
            _aiAgentMemoryService.InitData(data);
            aiTools.Add(
                 AIFunctionFactory.Create(_aIJsonLogService.Add,
                 new AIFunctionFactoryOptions
                 {
                     Name = "AddJson",
                     Description = "专门用于保存 Json 数据。"
                 }
          ));
            aiTools.Add(
                AIFunctionFactory.Create(_authorizedToolsService.GetUrlAuthorizedCodeAsync,
                new AIFunctionFactoryOptions
                {
                    Name = "GetUrlAuthorizedCodeAsync",
                    Description = "获取授权登录代码：当使用Python，Shell工具，http工具发起Http请求时，需要先获取401授权代码， 返回授权码：输出JSON明确指示Token值和放置位置（URL参数或Headers） 失败异常返回以 ❌ 开头的错误信息"
                }
         ));
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
                   Description = "保存文件(本地文件地址和文件内容二选一（必须有一个必填）)并返回远程访问url，当用户需要将内容保存为文件或者把本地文件上传到远程时调用。"
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
                        case "iKevinAITasksService.AddOnceTask":
                            aiTools.Add(
                                AIFunctionFactory.Create(_kevinAITaskService.AddOnceTask,
                                new AIFunctionFactoryOptions { Name = "AddOnceTask", Description = "创建一个一次性任务：在指定的未来时间点（如明天上午九点、几小时后）执行一次后自动结束，不会重复执行，也无需移除。同名重复添加会覆盖旧任务等同于更新执行时间。当用户要求在某个具体时间点执行一次时使用" }
                            ));
                            break;
                        case "iKevinAITasksService.RemoveCronTask":
                            aiTools.Add(
                                AIFunctionFactory.Create(_kevinAITaskService.RemoveCronTask,
                                new AIFunctionFactoryOptions { Name = "RemoveCronTask", Description = "移除任务（同时兼容周期性任务和未执行的一次性任务），已执行完成的一次性任务会自动结束无需移除" }
                            ));
                            break;
                        case "iKevinAITasksService.TriggerCronTask":
                            aiTools.Add(
                                AIFunctionFactory.Create(_kevinAITaskService.TriggerCronTask,
                                new AIFunctionFactoryOptions { Name = "TriggerCronTask", Description = "立即触发某个任务一次（同时兼容周期性任务和未执行的一次性任务）" }
                            ));
                            break;
                        case "iKevinAITasksService.GetTaskList":
                            aiTools.Add(
                            AIFunctionFactory.Create(_kevinAITaskService.GetTaskList,
                            new AIFunctionFactoryOptions { Name = "GetTaskList", Description = "获取我的所有任务列表（包含周期性任务和未执行的一次性任务）" }
                        ));
                            break;

                        case "AIMsgService.SendDDToUserMsg":
                            aiTools.Add(
                            AIFunctionFactory.Create(_IAIMsgService.SendDDToUserMsg,
                            new AIFunctionFactoryOptions { Name = "SendDDToUserMsg", Description = "发送钉钉消息给其他用户， 用于把消息发送给指定用户的钉钉账户。以 ❌ 开头的错误信息。" }
                        ));
                            break;
                        case "WebSearchEngine.DoubaoSearchGlobalAsync":
                            aiTools.Add(
                            AIFunctionFactory.Create(_webSearchEngine.DoubaoSearchGlobalAsync,
                            new AIFunctionFactoryOptions { Name = "DoubaoSearchGlobalAsync", Description = "豆包联网搜索Global版本，覆盖全球站点，综合搜索效果更好。当需要联网搜索实时信息、新闻、资料时调用，返回搜索结果列表（标题/链接/发布时间/摘要），失败返回以 ❌ 开头的错误信息" }
                        ));
                            break;
                        case "WebSearchEngine.DoubaoSearchCustomAsync":
                            aiTools.Add(
                            AIFunctionFactory.Create(_webSearchEngine.DoubaoSearchCustomAsync,
                            new AIFunctionFactoryOptions { Name = "DoubaoSearchCustomAsync", Description = "豆包联网搜索Custom版本，时延低，控制更灵活，支持各行业高频搜索需求。当需要联网搜索实时信息、新闻、资料时调用，返回搜索结果列表（标题/链接/来源/发布时间/摘要），失败返回以 ❌ 开头的错误信息" }
                        ));
                            break;
                        case "AgentMemoryTools.SaveMemory":
                            aiTools.Add(
                            AIFunctionFactory.Create(_aiAgentMemoryService.SaveMemoryAsync,
                            new AIFunctionFactoryOptions { Name = "SaveMemory", Description = "保存用户的长期记忆。当用户表达个人偏好、习惯、重要事实，或明确要求“记住某事”时调用，保存成功后简短告知用户。失败返回以 ❌ 开头的错误信息" }
                        ));
                            break;
                        case "AgentMemoryTools.SearchMemory":
                            aiTools.Add(
                            AIFunctionFactory.Create(_aiAgentMemoryService.SearchMemoryAsync,
                            new AIFunctionFactoryOptions { Name = "SearchMemory", Description = "搜索当前用户的长期记忆。需要回忆用户偏好、历史事实、约定事项，或回答涉及“我之前说过/我喜欢”等内容时先调用本工具。失败返回以 ❌ 开头的错误信息" }
                        ));
                            break;
                        case "AgentMemoryTools.UpdateMemory":
                            aiTools.Add(
                            AIFunctionFactory.Create(_aiAgentMemoryService.UpdateMemoryAsync,
                            new AIFunctionFactoryOptions { Name = "UpdateMemory", Description = "更新已有的长期记忆。当之前保存的记忆内容发生变化（如偏好改变）时调用，记忆Id需要先通过 SearchMemory 搜索获取。失败返回以 ❌ 开头的错误信息" }
                        ));
                            break;
                        case "AgentMemoryTools.DeleteMemory":
                            aiTools.Add(
                            AIFunctionFactory.Create(_aiAgentMemoryService.DeleteMemoryAsync,
                            new AIFunctionFactoryOptions { Name = "DeleteMemory", Description = "删除不再需要的长期记忆。当用户明确要求忘记某事或记忆已失效时调用，记忆Id需要先通过 SearchMemory 搜索获取。失败返回以 ❌ 开头的错误信息" }
                        ));
                            break;
                    }
                }
            }
            return aiTools;
        }

        private async Task<List<AITool>> GetMcpTools(object data, List<AISkillToolManagementDto> AISkillToolManagementDtos)
        {
            var aiTools = new List<AITool>();
            if (AISkillToolManagementDtos.Count <= 0)
            {
                return aiTools;
            }
            #region 获取Authorization
            var Authorization = "";
            if (HttpContextAccessor != default && HttpContextAccessor.Current() != default)
            {
                if (HttpContextAccessor.Current().Request.Headers.ContainsKey("Authorization"))
                {
                    Authorization = HttpContextAccessor.Current().Request.Headers["Authorization"].ToString();
                }
                if (string.IsNullOrEmpty(Authorization) || !JwtToken.IsBearerValidJwt(Authorization))
                {
                    if (HttpContextAccessor.Current().Request.Query.ContainsKey("Authorization"))
                    {
                        Authorization = HttpContextAccessor.Current().Request.Query["Authorization"].ToString();
                    }
                }
            }
            else
            {
                var _data = data;
                if (_data != default)
                {
                    long UserId = 0;
                    var TenantId = 0;
                    var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(_data));
                    if (jsonDoc.RootElement.TryGetProperty("UserId", out var userIdEl))
                    {
                        userIdEl.TryGetInt64(out UserId);
                    }
                    if (jsonDoc.RootElement.TryGetProperty("TenantId", out var tenantEl))
                    {
                        tenantEl.TryGetInt32(out TenantId);
                    }
                    if (UserId > 0 && TenantId > 0)
                    {
                        Authorization = "Bearer " + await _authorizeService.GetTokenById(UserId, TenantId);
                    }
                }
            }


            #endregion

            foreach (var item in AISkillToolManagementDtos)
            {
                if (string.IsNullOrEmpty(item.McpType))
                    continue;

                IClientTransport transport = null; // 声明传输对象

                try
                {
                    // Mcp自动授权Headers
                    var dic = !string.IsNullOrEmpty(item.McpHeaders) ? item.McpHeaders.ToObject<Dictionary<string, string>>() : new Dictionary<string, string>();
                    if (!dic.ContainsKey("Authorization"))
                    {
                        dic.Add("Authorization", Authorization);
                    }
                    switch (item.McpType.ToLowerInvariant())
                    {
                        case "http":
                        case "https":
                            // ---- HTTP / StreamableHttp ----
                            if (string.IsNullOrEmpty(item.McpUrl))
                                throw new ArgumentException("HTTP 模式需要提供 McpUrl");
                            transport = new HttpClientTransport(new HttpClientTransportOptions
                            {
                                Endpoint = new Uri(item.McpUrl),
                                TransportMode = HttpTransportMode.StreamableHttp, // 推荐
                                AdditionalHeaders = dic
                            });
                            break;

                        case "sse":
                            // ---- Server-Sent Events ----
                            if (string.IsNullOrEmpty(item.McpUrl))
                                throw new ArgumentException("SSE 模式需要提供 McpUrl");
                            transport = new HttpClientTransport(new HttpClientTransportOptions
                            {
                                Endpoint = new Uri(item.McpUrl),
                                TransportMode = HttpTransportMode.Sse, // 使用 SSE 模式
                                AdditionalHeaders = dic
                            });
                            break;

                        case "stdio":
                            // ---- 标准输入输出（本地进程） ----
                            if (string.IsNullOrEmpty(item.McpCommand))
                                throw new ArgumentException("Stdio 模式需要提供 McpCommand");

                            var stdioOptions = new StdioClientTransportOptions
                            {
                                Command = item.McpCommand,
                                Arguments = item.McpArguments?.Split(",").ToList(),
                                EnvironmentVariables = item.McpEnvironment?.ToObject<Dictionary<string, string?>>() ?? default
                            };
                            transport = new StdioClientTransport(stdioOptions);
                            break;

                        default:
                            throw new NotSupportedException($"不支持的传输类型: {item.McpType}");
                    }

                    // 创建客户端并获取工具
                    // 注意：返回的 McpClientTool 是远程调用句柄，模型调用工具时仍需通过同一个客户端连接发送请求，
                    // 因此这里不能用 await using 立即释放客户端，否则 transport 被关闭，工具调用会报 A task was canceled
                    var mcpClient = await McpClient.CreateAsync(transport);
                    try
                    {
                        var mcpTools = await mcpClient.ListToolsAsync();
                        aiTools.AddRange(mcpTools.Cast<AITool>());
                    }
                    catch
                    {
                        // 拉取工具失败时释放客户端，避免泄漏连接/子进程
                        await mcpClient.DisposeAsync();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // 建议记录日志，继续处理下一个服务
                    Kevin.log4Net.LogHelper.logger.Error(ex + string.Format("获取 MCP 工具失败, 类型: {0}, URL: {1}", item.McpType, item.McpUrl));
                    // 可以选择抛出或跳过
                }
            }
            return aiTools;
        }

        public async Task<List<string>> GetAIAgentSkillsAsync(object data, string agentId)
        {
            var agentBindIds = (await _iAISkillToolBindIdService.GetListById(agentId)).Select(t => t.AISkillToolManagementId).ToList();
            var skills = (await _iAISkillToolManagementService.GetNotDataPerAllSkills()).Where(t => agentBindIds.Contains(t.Id)).ToList();
            return skills.Where(t => agentBindIds.Contains(t.Id)).Select(t => t.Name).ToList();
        }

        public async Task<List<AITool>> GetAIAgentToolsAsync(object data, string agentId)
        {
            var aiTools = new List<AITool>();
            var agentBindIds = (await _iAISkillToolBindIdService.GetListById(agentId)).Select(t => t.AISkillToolManagementId).ToList();
            var tools = (await _iAISkillToolManagementService.GetNotDataPerAllTools()).Where(t => agentBindIds.Contains(t.Id)).ToList();
            aiTools.AddRange(await GetAITools(data, tools.Select(t => t.ClassMethod ?? "").ToList()));
            return aiTools;
        }

        public async Task<List<string>> GetAllAIAgentSkillsAsync(object data)
        {
            return (await _iAISkillToolManagementService.GetNotDataPerAllSkills()).Select(t => t.Name).ToList();
        }

        public async Task<List<AITool>> GetAllAIAgentToolsAsync(object data)
        {
            var tools = (await _iAISkillToolManagementService.GetNotDataPerAllTools());
            return await GetAITools(data, tools.Select(t => t.ClassMethod ?? "").ToList());
        }

        public async Task<List<string>> GetUserAIAgentSkillsAsync(object data, string agentId, string userId)
        {
            return await GetAIAgentSkillsAsync(data, agentId);
        }

        public async Task<List<AITool>> GetUserAIAgentToolsAsync(object data, string agentId, string userId)
        {
            return await GetAIAgentToolsAsync(data, agentId);
        }

        public async Task<List<AITool>> GetAIAgentMcpToolsAsync(object data, string agentId)
        {
            var aiTools = new List<AITool>();
            var agentBindIds = (await _iAISkillToolBindIdService.GetListById(agentId)).Select(t => t.AISkillToolManagementId).ToList();
            var mcps = (await _iAISkillToolManagementService.GetNotDataPerAllMcps()).Where(t => agentBindIds.Contains(t.Id)).ToList();
            aiTools.AddRange(await GetMcpTools(data, mcps));
            return aiTools;
        }

        public async Task<List<AITool>> GetUserAIAgentMcpToolsAsync(object data, string agentId, string userId)
        {
            return await GetAIAgentMcpToolsAsync(data, agentId);
        }
    }
}
