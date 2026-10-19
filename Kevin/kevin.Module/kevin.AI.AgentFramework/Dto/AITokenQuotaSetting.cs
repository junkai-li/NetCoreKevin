using Kevin.Common.Helper;
using Microsoft.Extensions.Primitives;

namespace kevin.AI.AgentFramework.Dto
{
    /// <summary>
    /// 用户月度 AI Token 额度配置（对应 appsettings 的 AITokenQuotaSetting 节，可不配置，默认 2 亿）。
    /// <para>
    /// 每月 1 日由定时任务按 <see cref="MonthlyQuota"/> 给所有用户颁发当月额度，
    /// 聊天前按当月已用额度校验、聊天完成后累加用量。
    /// </para>
    /// <para>
    /// 填 0 或负数视为“未配置”并沿用默认值 2 亿（额度为 0 会让所有用户无法聊天，不提供该能力）。
    /// 修改 appsettings 后随配置热重载生效，无需重启。
    /// </para>
    /// </summary>
    public class AITokenQuotaSetting
    {
        /// <summary>
        /// 配置节名称
        /// </summary>
        private const string SectionKey = "AITokenQuotaSetting";

        /// <summary>
        /// 每个用户每月默认颁发的额度（Token），默认 2 亿
        /// </summary>
        public long MonthlyQuota { get; set; } = 200_000_000;

        private static readonly object _lock = new();
        private static AITokenQuotaSetting? _cached;
        private static IChangeToken? _cachedToken;

        /// <summary>
        /// 当前生效的配置：进程内缓存，appsettings 变更后（配置提供程序热重载产生新的 reload token）自动重新读取
        /// </summary>
        public static AITokenQuotaSetting Current
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
        /// 读取配置节；配置未初始化（如单元测试）或节缺失时返回默认值
        /// </summary>
        private static AITokenQuotaSetting Load()
        {
            var setting = new AITokenQuotaSetting();
            try
            {
                var section = ConfigHelper.Configuration?.GetSection(SectionKey);
                if (section == null) return setting;
                setting.MonthlyQuota = ReadLong(section[nameof(MonthlyQuota)], setting.MonthlyQuota);
            }
            catch
            {
                return new AITokenQuotaSetting();
            }
            return setting;
        }

        /// <summary>
        /// 解析额度值，非法（非数字、≤0）时退回默认值
        /// </summary>
        private static long ReadLong(string? value, long defaultValue)
            => long.TryParse(value, out var result) && result > 0 ? result : defaultValue;
    }
}
