using Kevin.Common.Helper;
using Microsoft.Extensions.Primitives;

namespace kevin.AI.AgentFramework.Dto
{
    /// <summary>
    /// OpenAI 协议兼容接口的非鉴权设置项（appsettings.json 的 OpenAICompatibleSetting）：
    /// 外部客户端用 Authorization: Bearer {Key} 调用 /v1/chat/completions 等接口。
    /// <para>
    /// 注意：ApiKey 列表（ApiKeys）已下线，Key 改为由 OpenAICompatibleKeyController 维护、
    /// 落库到 TAIOpenAICompatibleKey（AES 加密）。本类只保留网关运行所需的非鉴权设置项。
    /// </para>
    /// </summary>
    public class OpenAICompatibleSetting
    {
        /// <summary>配置节名称</summary>
        private const string SectionKey = "OpenAICompatibleSetting";

        /// <summary>会话映射（会话标识 → 内部会话）的有效期，单位小时；过期后同一会话标识会自动新建会话</summary>
        public int SessionExpirationHours { get; set; } = 24;

        /// <summary>是否把模型思考过程以 reasoning_content 字段流式下发（不支持的客户端会自动忽略该字段）</summary>
        public bool ReasoningOutput { get; set; } = true;

        /// <summary>是否隐藏 AI 聊天对话（null=不限制，true=隐藏，false=显示）</summary>
        public bool AIChatIsHidden { get; set; } = true;

        private static readonly object _lock = new();
        private static OpenAICompatibleSetting? _cached;
        private static IChangeToken? _cachedToken;

        /// <summary>
        /// 当前生效的配置：进程内缓存，appsettings 变更后（配置提供程序热重载产生新的 reload token）自动重新读取
        /// </summary>
        public static OpenAICompatibleSetting Current
        {
            get
            {
                var token = ConfigHelper.Configuration?.GetReloadToken();
                var cached = _cached;
                if (cached != null && ReferenceEquals(_cachedToken, token)) return cached;
                lock (_lock)
                {
                    token = ConfigHelper.Configuration?.GetReloadToken();
                    cached = _cached;
                    if (cached != null && ReferenceEquals(_cachedToken, token)) return cached;
                    _cached = Load();
                    _cachedToken = token;
                    return _cached;
                }
            }
        }

        /// <summary>读取配置节；配置未初始化（如单元测试）或节缺失时返回默认值</summary>
        private static OpenAICompatibleSetting Load()
        {
            var setting = new OpenAICompatibleSetting();
            try
            {
                var section = ConfigHelper.Configuration?.GetSection(SectionKey);
                if (section == null) return setting;
                if (int.TryParse(section[nameof(SessionExpirationHours)], out var hours) && hours > 0)
                    setting.SessionExpirationHours = hours;
                if (bool.TryParse(section[nameof(ReasoningOutput)], out var reasoning))
                    setting.ReasoningOutput = reasoning;
                if (bool.TryParse(section[nameof(AIChatIsHidden)], out var hidden))
                    setting.AIChatIsHidden = hidden;
            }
            catch
            {
                return new OpenAICompatibleSetting();
            }
            return setting;
        }
    }
}
