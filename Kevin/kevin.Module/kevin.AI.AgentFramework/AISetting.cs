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
        /// </summary>
        public Action<string> StreameCallback { get; set; } = default;

        /// <summary>
        /// 工具流式请求回调
        /// </summary>
        public Action<string> ToolStreameCallback { get; set; } = default;

        /// <summary>
        /// 思考过程流式请求回调
        /// </summary>
        public Action<string> ReasoningStreameCallback { get; set; } = default;

        /// <summary>
        /// Auto模式下的备选模型信息列表，当主模型失败时自动随机切换到下一个未使用过的模型
        /// </summary>
        public List<AIFallbackModel> FallbackModels { get; set; } = new();

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
