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
        public bool IsAISkills { get; set; } = false;
        /// <summary>
        /// 是否启用AI工具
        /// </summary>
        public bool IsAITools { get; set; } = false;

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetries { get; set; } = 3;
        /// <summary>
        /// AI请求超时时间，单位分钟
        /// </summary>
        public int NetworkTimeout { get; set; } = 10;

    }
}
