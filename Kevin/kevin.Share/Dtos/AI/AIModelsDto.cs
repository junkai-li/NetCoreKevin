using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIModelsDto : CUD_User_Dto
    {
        /// <summary>
        /// AI类型
        /// </summary>

        [Required]
        [Description("AI类型")]
        public AIType AIType { get; set; } = AIType.OpenAI;
        /// <summary>
        /// 模型类型
        /// </summary>
        [Required]
        [Description("模型类型")]
        public AIModelType AIModelType { get; set; } = AIModelType.Chat;
        /// <summary>
        /// 模型地址
        /// </summary>
        [Required]
        [Description("模型地址")]
        public string EndPoint { get; set; } = "";
        /// <summary>
        /// 模型名称
        /// </summary>
        [Required]
        [Description("模型名称")]
        public string ModelName { get; set; } = "";
        /// <summary>
        /// 模型秘钥
        /// </summary> 
        [Description("模型秘钥")]
        public string ModelKey { get; set; } = "";
        /// <summary>
        /// 部署名，azure需要使用
        /// </summary>

        [Required]
        [Description("部署名，azure需要使用")]
        public string ModelDescription { get; set; }

        /// <summary>
        /// 矢量值大小
        /// </summary> 
        [Description("矢量值大小")]
        public int EmbeddingValueSize { get; set; } = 2048;

        /// <summary>
        /// 提问最大token数（模型上下文窗口预算）
        /// </summary> 
        [Description("提问最大token数")]
        [DefaultValue(131072)]
        public int MaxAskPromptSize { get; set; } = 131072;

        /// <summary>
        /// 回答最大token数（模型最大输出长度）
        /// </summary>
        [Description("回答最大token数")]
        [DefaultValue(8192)]
        public int AnswerTokens { get; set; } = 8192;
    }
}
