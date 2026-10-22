using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Dto;

namespace kevin.AI.AgentFramework.TextLimits
{
    /// <summary>
    /// 全项目统一的“字符上限”口径：所有按字符截断的地方都从这里取上限、走这里的截断方法，
    /// 业务代码里不再散落 200000/20000/500 这类魔法数。
    /// <para>
    /// 注意：这里是字符数，不是 Token；Token 估算见 <see cref="TokenEstimation.TokenEstimator"/>。
    /// 入库类上限来自 <see cref="AIChatStorageSetting"/>（appsettings 的 AIChatStorageSetting 节，可热更新）；
    /// 其余是提示词、展示与工具结果的固定上限。
    /// </para>
    /// <para>
    /// 截断方法的长度语义：<see cref="AppendWithCap"/>、<see cref="ShrinkForDb"/> 把说明文字计入额度，返回值总长 ≤ maxChars；
    /// <see cref="Truncate"/>、<see cref="TruncateToolResult"/> 是在 maxChars 之后再追加截断提示，返回值可能略超 maxChars。
    /// </para>
    /// </summary>
    public static class CharLimits
    {
        #region 入库体积上限（可配置）

        /// <summary>单条提问内容入库上限（字符）：超限直接提示用户精简，不静默截断用户原文</summary>
        public static int AskContent => AIChatStorageSetting.Current.AskContentMaxLength;
        /// <summary>单条回答内容入库上限（字符）</summary>
        public static int AnswerContent => AIChatStorageSetting.Current.AnswerContentMaxLength;
        /// <summary>工具调用日志累计入库上限（字符）</summary>
        public static int ToolsLog => AIChatStorageSetting.Current.ToolsLogMaxLength;
        /// <summary>思考过程日志累计入库上限（字符）</summary>
        public static int ReasoningLog => AIChatStorageSetting.Current.ReasoningLogMaxLength;
        /// <summary>绑定日志（系统提示词、知识库检索、联网搜索结果）单条入库上限（字符）</summary>
        public static int BindLogContent => AIChatStorageSetting.Current.BindLogContentMaxLength;
        /// <summary>消息表单次批量入库的累计字符上限：超过时拆成多次提交</summary>
        public static int MaxCharsPerSave => AIChatStorageSetting.Current.MaxCharsPerSave;

        #endregion

        #region 工具与压缩上限

        /// <summary>工具结果写回历史时的兜底上限（字符）：ContentLengthLimit 配 0（不截断）时仍要防止 INSERT 撑爆 4MB 包</summary>
        public const int ToolResultStoreFallback = 200_000;
        /// <summary>技能脚本单个输出流保留的最大字符数（超出丢弃并标记截断）</summary>
        public const int ScriptOutputBuffer = 200_000;
        /// <summary>压缩摘要工具返回给模型的字符上限</summary>
        public const int CompactionToolPrompt = 20000;
        /// <summary>压缩摘要单条最少保留字符</summary>
        public const int CompactionSummaryMin = 2000;
        /// <summary>没有预算依据时的压缩摘要字符上限</summary>
        public const int CompactionSummaryNoBudget = 20000;

        #endregion

        #region 展示与外部接口上限

        /// <summary>推荐问题提示词里用户提问的截断长度</summary>
        public const int RecommendQuestionAsk = 500;
        /// <summary>会话名称展示上限</summary>
        public const int ChatName = 200;
        /// <summary>会话列表最后一条消息预览上限</summary>
        public const int ChatPreview = 300;
        /// <summary>OpenAI 兼容接口自动创建的会话名称长度</summary>
        public const int OpenAISessionName = 50;
        /// <summary>OpenAI 兼容接口工具续轮占位文案里工具结果预览长度</summary>
        public const int ToolTurnPlaceholderPreview = 120;
        /// <summary>失败原因展示上限</summary>
        public const int FailReason = 2000;
        /// <summary>外部 HTTP 接口错误响应写入异常信息时的截断长度</summary>
        public const int HttpErrorBody = 500;
        /// <summary>联网搜索 HTTP 错误响应写入异常信息时的截断长度</summary>
        public const int WebSearchErrorBody = 200;
        /// <summary>联网搜索结果摘要展示上限</summary>
        public const int SearchSnippetPreview = 500;

        #endregion

        #region 记忆

        /// <summary>单条记忆内容上限（字符）</summary>
        public const int MemoryContent = 4000;
        /// <summary>记忆关键词上限（字符；需与实体 TAIAgentMemory.Keywords 的 MaxLength 保持一致）</summary>
        public const int MemoryKeywords = 200;
        /// <summary>自动召回时用于检索的提问上限（字符）</summary>
        public const int AutoRecallQuery = 500;
        /// <summary>自动召回时单条记忆内容上限（字符）</summary>
        public const int AutoRecallContent = 400;
        /// <summary>重复记忆提示里展示已有内容的长度（字符）</summary>
        public const int MemoryDuplicatePreview = 80;

        #endregion

        /// <summary>
        /// 统一截断（超限时追加 StringHelper 的标准“输出已截断”提示）。
        /// maxChars ≤ 0 表示不限制。
        /// </summary>
        public static string Truncate(string? text, int maxChars)
        {
            if (string.IsNullOrEmpty(text) || maxChars <= 0 || text.Length <= maxChars) return text ?? "";
            return StringHelper.SubstringText(text, maxChars);
        }

        /// <summary>
        /// 统一截断（超限时追加指定后缀，用于展示类短文本）。
        /// maxChars ≤ 0 表示不限制。
        /// </summary>
        public static string Truncate(string? text, int maxChars, string suffix)
        {
            if (string.IsNullOrEmpty(text) || maxChars <= 0 || text.Length <= maxChars) return text ?? "";
            return StringHelper.SubstringText(text, maxChars, suffix);
        }

        /// <summary>
        /// 工具结果截断：超限时在开头加统一的“内容已截断”提示，避免模型把半截内容当成全文。
        /// maxChars ≤ 0 表示不限制。
        /// </summary>
        public static string TruncateToolResult(string? text, int maxChars)
        {
            text ??= "";
            if (maxChars <= 0 || text.Length <= maxChars) return text;
            return SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(text, maxChars);
        }

        /// <summary>
        /// 向累计日志追加文本：达到上限后不再追加（首次触顶时补一句说明）。
        /// 说明文字自身也占额度，保证返回值总长不超过 maxChars。
        /// </summary>
        public static string AppendWithCap(string? current, string? chunk, int maxChars)
        {
            current ??= "";
            if (string.IsNullOrEmpty(chunk) || maxChars <= 0) return current + chunk;
            if (current.Length >= maxChars) return current;
            var room = maxChars - current.Length;
            if (chunk.Length <= room) return current + chunk;
            var note = $"\n（日志已达 {maxChars} 字符入库上限，后续内容不再记录）";
            return current + chunk.Substring(0, Math.Max(0, room - note.Length)) + note;
        }

        /// <summary>
        /// 单条入库前按上限截断（不超限时原样返回），说明文字自身占额度。
        /// maxChars ≤ 0 表示不限制。
        /// </summary>
        public static string ShrinkForDb(string? text, int maxChars)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (maxChars <= 0 || text.Length <= maxChars) return text;
            var note = $"\n（内容已达 {maxChars} 字符入库上限，后续部分未入库）";
            return text.Substring(0, Math.Max(0, maxChars - note.Length)) + note;
        }
    }
}
