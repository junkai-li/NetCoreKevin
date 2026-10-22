namespace kevin.AI.AgentFramework.TokenEstimation
{
    /// <summary>
    /// 全项目统一的 Token 估算口径：1 个字符 ≈ 0.6 Token。
    /// <para>
    /// 历史裁剪、自动压缩触发、补充上下文裁剪、上下文用量展示都走这里，避免各处各算一套导致口径漂移。
    /// 模型真实分词由模型决定（中文约 1 字≈1 Token，英文/JSON 约 3~4 字符≈1 Token），0.6 是折中值；
    /// 估算仍偏保守，用于预算判断足够。
    /// </para>
    /// </summary>
    public static class TokenEstimator
    {
        /// <summary>
        /// 每个字符折算的 Token 数（统一口径，改这里全局生效）
        /// </summary>
        public const double TokensPerCharacter = 0.6;

        /// <summary>
        /// 估算文本的 Token 数（向上取整，保证不低估）
        /// </summary>
        public static int EstimateTokens(string? text)
        {
            return string.IsNullOrEmpty(text) ? 0 : (int)Math.Ceiling(text.Length * TokensPerCharacter);
        }

        /// <summary>
        /// 按字符数估算 Token 数（向上取整）
        /// </summary>
        public static int EstimateTokensByCharCount(int charCount)
        {
            return charCount <= 0 ? 0 : (int)Math.Ceiling(charCount * TokensPerCharacter);
        }

        /// <summary>
        /// 把 Token 预算换算成可容纳的字符数（向下取整），用于按字符截断文本
        /// </summary>
        public static int ToCharCount(int tokenBudget)
        {
            return tokenBudget <= 0 ? 0 : (int)Math.Floor(tokenBudget / TokensPerCharacter);
        }
    }
}
