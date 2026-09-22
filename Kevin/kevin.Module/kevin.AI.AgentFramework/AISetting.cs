namespace Kevin.AI.Dto
{
    public class AISetting
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string AIUrl { get; set; } = "https://dashscope.aliyuncs.com/compatible-mode/v1/";


        /// <summary>
        /// 账户私钥
        /// </summary>
        public string AIKeySecret { get; set; } = "*****";

        /// <summary>
        /// 默认模型
        /// </summary>
        public string AIDefaultModel { get; set; } = "deepseek-v3";
        /// <summary>
        /// //是否开启AI请求日志
        /// </summary>
        public bool IsHttpLog { get; set; } = false;
        /// <summary>
        /// 是否启用AI技能
        /// </summary>
        public bool IsAISkills { get; set; } = true;
        /// <summary>
        /// 是否启用AI工具
        /// </summary>
        public bool IsAITools { get; set; } = true;
        /// <summary>
        /// 是否启用Mcp工具
        /// </summary>
        public bool IsMcpTools { get; set; } = true;

        /// <summary>
        /// 是否启用记忆功能
        /// </summary>
        public bool IsMemory { get; set; } = false;

        /// <summary>
        /// 是否启用知识库功能
        /// </summary>
        public bool IsKnowledgeBase { get; set; } = true;

        /// <summary>
        /// 是否允许音频输入：由 <c>AIModels.AIModelType.HasFlag(AudioUnderstanding)</c> 决定，
        /// 关闭时即便用户上传了音频也会走"不支持"分支，避免把音频塞给不懂音频的模型触发 400
        /// </summary>
        public bool EnableAudioInput { get; set; } = false;

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetries { get; set; } = 3;
        /// <summary>
        /// AI请求超时时间，单位分钟
        /// </summary>
        public int NetworkTimeout { get; set; } = 30;

        /// <summary>
        /// 是否开启流式请求
        /// </summary>
        public bool IsStreame { get; set; } = true;

        /// <summary>
        /// 流式请求回调
        /// <para>
        /// 用 Func&lt;string, Task&gt; 而不是 Action&lt;string&gt;：调用侧会 await 它，推送异常才能回到模型调用的 try/catch 里。
        /// Action 上挂 async lambda 等于 async void，其内部异常不会被任何调用方接管，
        /// 而是直接抛到线程池变成进程级 Unhandled exception（一次 Redis 抖动就能把整个服务干掉）。
        /// </para>
        /// </summary>
        public Func<string, Task>? StreameCallback { get; set; }

        /// <summary>
        /// 工具流式请求回调（语义同 <see cref="StreameCallback"/>）
        /// </summary>
        public Func<string, Task>? ToolStreameCallback { get; set; }

        /// <summary>
        /// 思考过程流式请求回调（语义同 <see cref="StreameCallback"/>）
        /// </summary>
        public Func<string, Task>? ReasoningStreameCallback { get; set; }

        /// <summary>
        /// Auto模式下的备选模型信息列表，当主模型失败时自动随机切换到下一个未使用过的模型
        /// </summary>
        public List<AIFallbackModel> FallbackModels { get; set; } = new();

        /// <summary>
        /// 重试耗尽（或参数类错误直接失败）后的原始异常。
        /// <para>
        /// 模型异常仍会以友好文案流式输出，但调用方需要区分“正常回复”与“报错文案”，
        /// 因此这里把异常本身带出去，由调用方落库为失败态并提供重试入口。
        /// </para>
        /// </summary>
        public Exception? LastError { get; set; }

        /// <summary>
        /// 本次调用实际尝试次数（含首次），失败时用于展示“重试了几次仍失败”
        /// </summary>
        public int AttemptCount { get; set; } = 1;

    }

    /// <summary>
    /// Auto模式备选模型信息
    /// </summary>
    public class AIFallbackModel
    {
        /// <summary>
        /// 模型API地址
        /// </summary>
        public string AIUrl { get; set; } = string.Empty;
        /// <summary>
        /// 模型密钥
        /// </summary>
        public string AIKeySecret { get; set; } = string.Empty;
        /// <summary>
        /// 模型名称
        /// </summary>
        public string AIDefaultModel { get; set; } = string.Empty;
        /// <summary>
        /// 提问最大token数（模型上下文窗口预算）
        /// </summary>
        public int MaxAskPromptSize { get; set; } = 131072;
        /// <summary>
        /// 回答最大token数（模型最大输出长度）
        /// </summary>
        public int AnswerTokens { get; set; } = 8192;
    }
}
