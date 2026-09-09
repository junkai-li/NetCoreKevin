using Kevin.Common.Helper;
using Microsoft.Extensions.Primitives;

namespace kevin.AI.AgentFramework.Dto
{
    /// <summary>
    /// AI 对话内容入库体积限制（对应 appsettings 的 AIChatStorageSetting 节，可不配置，全部走此处默认值）。
    /// <para>
    /// 约束来源是 MySQL 的 max_allowed_packet（默认 4MB）：它限制的是单条 SQL 的包体积而不是字段类型，
    /// 所以哪怕字段是 longtext，一条塞了几百万字符的 INSERT 也会以 DbUpdateException（Error submitting 4MB packet）失败，
    /// 并把整个对话请求一起打挂。中文按 3 字节/字估算，40 万字符约 1.2MB。
    /// </para>
    /// <para>
    /// 填 0 或负数视为“未配置”并沿用默认值：这些上限是防请求失败的兜底，不提供整体关闭的能力
    /// （上限取 0 在历史Token预算等逻辑里会被当成“不限制”，守卫会整体失效）。
    /// 修改 appsettings 后随配置热重载生效，无需重启。
    /// </para>
    /// </summary>
    public class AIChatStorageSetting
    {
        /// <summary>
        /// 配置节名称
        /// </summary>
        private const string SectionKey = "AIChatStorageSetting";

        /// <summary>
        /// 单条提问内容入库上限（字符）：超限直接提示用户精简，不静默截断用户原文
        /// </summary>
        public int AskContentMaxLength { get; set; } = 400000;
        /// <summary>
        /// 单条回答内容入库上限（字符）
        /// </summary>
        public int AnswerContentMaxLength { get; set; } = 400000;
        /// <summary>
        /// 工具调用日志累计入库上限（字符）：流式事件是 += 累加的，不封顶一个回合就能堆几十万字
        /// </summary>
        public int ToolsLogMaxLength { get; set; } = 300000;
        /// <summary>
        /// 思考过程日志累计入库上限（字符）
        /// </summary>
        public int ReasoningLogMaxLength { get; set; } = 150000;
        /// <summary>
        /// 绑定日志（系统提示词、知识库检索、联网搜索结果）单条入库上限（字符）
        /// </summary>
        public int BindLogContentMaxLength { get; set; } = 200000;
        /// <summary>
        /// 消息表单次批量入库的累计字符上限：EF 会把一次 SaveChanges 的多条 INSERT 批处理成一条 SQL 发送，
        /// 超过该值时拆成多次提交
        /// </summary>
        public int MaxCharsPerSave { get; set; } = 800000;

        private static readonly object _lock = new();
        private static AIChatStorageSetting? _cached;
        private static IChangeToken? _cachedToken;

        /// <summary>
        /// 当前生效的配置：进程内缓存，appsettings 变更后（配置提供程序热重载产生新的 reload token）自动重新读取，
        /// 避免在流式回调这种每条消息都命中的路径上反复解析配置
        /// </summary>
        public static AIChatStorageSetting Current
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

        /// <summary>
        /// 读取配置节；配置未初始化（如单元测试）或节缺失时返回默认值，入库守卫不能反过来让请求出错
        /// </summary>
        private static AIChatStorageSetting Load()
        {
            var setting = new AIChatStorageSetting();
            try
            {
                var section = ConfigHelper.Configuration?.GetSection(SectionKey);
                if (section == null) return setting;
                setting.AskContentMaxLength = ReadInt(section[nameof(AskContentMaxLength)], setting.AskContentMaxLength);
                setting.AnswerContentMaxLength = ReadInt(section[nameof(AnswerContentMaxLength)], setting.AnswerContentMaxLength);
                setting.ToolsLogMaxLength = ReadInt(section[nameof(ToolsLogMaxLength)], setting.ToolsLogMaxLength);
                setting.ReasoningLogMaxLength = ReadInt(section[nameof(ReasoningLogMaxLength)], setting.ReasoningLogMaxLength);
                setting.BindLogContentMaxLength = ReadInt(section[nameof(BindLogContentMaxLength)], setting.BindLogContentMaxLength);
                setting.MaxCharsPerSave = ReadInt(section[nameof(MaxCharsPerSave)], setting.MaxCharsPerSave);
            }
            catch
            {
                return new AIChatStorageSetting();
            }
            return setting;
        }

        /// <summary>
        /// 解析上限值，非法（非数字、≤0）时退回默认值
        /// </summary>
        private static int ReadInt(string? value, int defaultValue)
            => int.TryParse(value, out var result) && result > 0 ? result : defaultValue;
    }
}
