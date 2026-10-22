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
        /// 预算算不出正数时的兜底比例：退化为提问上限的 60%。
        /// 不能退化成 0：0 在历史裁剪里的语义是“不限制”，长会话会因此彻底失去保护。
        /// </summary>
        public const double FallbackAskBudgetRatio = 0.6;

        /// <summary>
        /// 单条二进制内容（图片/音频/文件）的估算输入开销（Token，不按字符折算）：
        /// 真实 token 数由模型分块策略决定，这里取一个与体积无关的保守固定值。
        /// </summary>
        public const int BinaryContentTokens = 1000;

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

        /// <summary>
        /// 计算可用提问预算的原始差值 = 提问上限 − 系统提示词 − 回答预留。
        /// 返回值可能 ≤0：由调用方决定“放不下就不放”（如补充上下文裁剪），或改用 <see cref="ResolveAskBudget"/> 兜底。
        /// 上限 ≤0 表示模型未配置提问预算，返回 0。
        /// </summary>
        public static int ComputeAskBudget(int maxAskPromptSize, int answerTokens, string? systemPrompt = null)
        {
            if (maxAskPromptSize <= 0) return 0;
            return maxAskPromptSize - EstimateTokens(systemPrompt) - answerTokens;
        }

        /// <summary>
        /// 解析可用提问预算：差值 ≤0 时退化为提问上限的 <see cref="FallbackAskBudgetRatio"/>，上限 ≤0 返回 0（不限制）。
        /// 历史裁剪、压缩触发、上下文展示统一走这里，避免各处各写一份 60% 兜底。
        /// </summary>
        public static int ResolveAskBudget(int maxAskPromptSize, int answerTokens, string? systemPrompt = null)
        {
            if (maxAskPromptSize <= 0) return 0;
            var budget = ComputeAskBudget(maxAskPromptSize, answerTokens, systemPrompt);
            return budget > 0 ? budget : (int)(maxAskPromptSize * FallbackAskBudgetRatio);
        }
    }
}
