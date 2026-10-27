using kevin.Application.Services.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permisson.Attributes;
using Kevin.log4Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using Web.Filters;
using Web.Global.Exceptions;

namespace Kevin.Web.Basics.Controllers.AI.OpenAI
{
    /// <summary>
    /// OpenAI 协议兼容接口：外部客户端以 OpenAI Chat Completions 协议接入平台智能体。
    /// <para>
    /// 鉴权：Authorization: Bearer {api_key}（用户级密钥落库，由 <see cref="OpenAICompatibleApiKeyFilter"/> 校验并换发内部凭证，
    /// 请求内自动换成该用户的身份，权限/MCP/落库行为与网页端一致）。
    /// </para>
    /// <para>
    /// 接口：GET /v1/models（列出当前身份可用的智能体）、POST /v1/chat/completions（stream 支持流式/非流式）。
    /// 会话衔接与协议解析由 <see cref="IOpenAICompatibleChatService"/> 处理，本控制器只负责 HTTP 协议输入输出。
    /// </para>
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/v1")]
    [AllowAnonymous]
    [SkipAuthority]
    [SkipResultFilter]
    [OpenAICompatibleApiKeyFilter]
    public class OpenAICompatibleController : ControllerBase
    {
        private static readonly JsonSerializerOptions ProtocolJsonOptions = new()
        {
            // 中文内容按原文下发，不转成 \uXXXX 转义
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private readonly IOpenAICompatibleChatService _chatService;
        private readonly IAIAppsService _aIAAppsService;

        public OpenAICompatibleController(
            IOpenAICompatibleChatService chatService,
            IAIAppsService aIAAppsService)
        {
            _chatService = chatService;
            _aIAAppsService = aIAAppsService;
        }

        /// <summary>
        /// 模型列表：把当前身份可用的智能体映射为 OpenAI 模型条目，客户端据此选择 model。
        /// </summary>
        [HttpGet("models")]
        public async Task Models()
        {
            var myApps = await _aIAAppsService.GetMyALLList();
            var list = new OpenAICompatibleModelList
            {
                Data = myApps.Select(t => new OpenAICompatibleModelItem
                {
                    // 模型 Id 默认即智能体名称，客户端原样回传以定位智能体；名称为空时退回智能体 Id
                    Id = string.IsNullOrWhiteSpace(t.Name) ? t.Id.ToString() : t.Name.Trim(),
                    Created = t.CreateTime == default ? 0 : new DateTimeOffset(t.CreateTime).ToUnixTimeSeconds(),
                    OwnedBy = "net_ai_web",
                }).ToList()
            };
            await WriteJsonAsync(list, StatusCodes.Status200OK);
        }

        /// <summary>
        /// 聊天补全：与网页端共用同一套智能体处理管线。
        /// stream=true 时把回答分片翻译成 OpenAI 流式帧，否则返回完整 chat.completion。
        /// </summary>
        [HttpPost("chat/completions")]
        public async Task ChatCompletions([FromBody] OpenAICompatibleChatRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var headerConversationId = Request.Headers["X-Conversation-Id"].ToString();
                if (req.Stream)
                {
                    // 流式：响应头先落地，服务把分片按 chat.completion.chunk 帧实时写入
                    SetSseHeaders();
                    await _chatService.ChatStreamAsync(req, headerConversationId, cancellationToken);
                    return;
                }

                // 非流式：服务返回完整结果，此处翻译成 chat.completion
                var outcome = await _chatService.ChatAsync(req, headerConversationId, cancellationToken);
                var payload = new OpenAICompatibleCompletion
                {
                    Id = "chatcmpl-" + Guid.NewGuid().ToString("N"),
                    Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Model = req.Model!,
                    Choices = new List<OpenAICompatibleCompletionChoice>
                    {
                        new()
                        {
                            // 透传工具调用轮按 OpenAI 协议以 tool_calls 收尾，客户端执行后回传 role=tool 结果继续
                            FinishReason = outcome.ToolCalls is { Count: > 0 } ? "tool_calls" : "stop",
                            Message = new OpenAICompatibleCompletionMessage
                            {
                                Content = outcome.Content,
                                ReasoningContent = outcome.ReasoningContent,
                                ToolCalls = outcome.ToolCalls,
                            }
                        }
                    },
                    Usage = new OpenAICompatibleUsage
                    {
                        PromptTokens = outcome.PromptTokens,
                        CompletionTokens = outcome.CompletionTokens,
                        TotalTokens = outcome.TotalTokens,
                    }
                };
                await WriteJsonAsync(payload, StatusCodes.Status200OK);
            }
            catch (OpenAIProtocolException ex)
            {
                await WriteProtocolErrorAsync(ex);
            }
            catch (UserFriendlyException ex)
            {
                await WriteProtocolErrorAsync(new OpenAIProtocolException(400, ex.Message));
            }
            catch (OperationCanceledException)
            {
                // 客户端主动断开：响应已无意义，静默结束（管线内部会自行落一条失败记录供网页端查阅）
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error("OpenAI兼容接口处理异常:" + ex.Message, ex);
                await WriteProtocolErrorAsync(new OpenAIProtocolException(500, "服务内部异常：" + ex.Message, "server_error"));
            }
        }

        #region 响应输出

        private void SetSseHeaders()
        {
            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";
            // 反向代理默认缓冲响应会破坏流式实时性（与 AddSSE 一致）
            Response.Headers["X-Accel-Buffering"] = "no";
        }

        private async Task WriteJsonAsync(object payload, int statusCode)
        {
            Response.StatusCode = statusCode;
            Response.ContentType = "application/json; charset=utf-8";
            await Response.WriteAsync(JsonSerializer.Serialize(payload, ProtocolJsonOptions));
        }

        /// <summary>协议异常 → OpenAI 风格 JSON 错误（流式中途的错误已在服务内以 error 数据帧下发，不会走到这里）</summary>
        private async Task WriteProtocolErrorAsync(OpenAIProtocolException ex)
        {
            try
            {
                await WriteJsonAsync(new OpenAICompatibleErrorResponse
                {
                    Error = new OpenAICompatibleErrorBody { Message = ex.Message, Type = ex.ErrType, Code = ex.Code }
                }, ex.StatusCode);
            }
            catch (Exception writeEx)
            {
                // 客户端可能已断开：错误提示送不到就算了，避免二次异常盖掉原始日志
                LogHelper.logger.Info("OpenAI兼容接口错误响应写出失败（已忽略）：" + writeEx.Message);
            }
        }

        #endregion
    }
}
