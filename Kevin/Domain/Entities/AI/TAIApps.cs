using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// TAIApps
    /// </summary>
    [Table("TAIApps")]
    [Description("AIAPP")]
    public partial class TAIApps : CUD_User
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public String Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required]
        public String Describe { get; set; } = "";

        /// <summary>
        /// 图标
        /// </summary>
        [Required]
        public String Icon { get; set; } = "windows";

        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public String Type { get; set; }

        /// <summary>
        /// 会话模型ID
        /// </summary>
        [Required]
        [Description("会话模型ID")]
        public String? ChatModelID { get; set; } 
        [MaxLength(200)]
        public String? RerankModelID { get; set; } 
        /// <summary>
        /// 温度 温度越低0.1-2.0，答案越确定；温度越高，答案越随机
        /// </summary> 
        [DefaultValue(70)]
        [Description("温度")]
        public Double Temperature { get; set; } = 70f; 

        /// <summary>
        /// 知识库ID
        /// </summary> 
        [Description("知识库ID")]
        public long? KmsId { get; set; }
        public TAIKmss? Kms { get; set; }
        /// <summary>
        /// API调用秘钥
        /// </summary>
        [MaxLength(100)]
        [Description("API调用秘钥")]
        public String? SecretKey { get; set; }

        /// <summary>
        /// 相似度
        /// </summary> 
        [DefaultValue(60)]
        [Description("相似度")]
        public Double Relevance { get; set; } = 60f;

        /// <summary>
        /// 提问最大token数
        /// </summary> 
        [DefaultValue(2048)]
        [Description("提问最大token数")]
        public int MaxAskPromptSize { get; set; } = 2048;
        /// <summary>
        /// 向量匹配数
        /// </summary> 
        [DefaultValue(3)]
        [Description("向量匹配数")]
        public int MaxMatchesCount { get; set; } = 3;


        [DefaultValue(20)]
        [Description("RerankCount")]
        public int RerankCount { get; set; } = 20;
        /// <summary>
        /// 回答最大token数
        /// </summary> 
        [Description("回答最大token数")]
        [DefaultValue(2048)]
        public int AnswerTokens { get; set; } = 2048;

        /// <summary>
        /// 提示词绑定
        /// </summary>
        [Description("提示词Id")]
        public long AIPromptID { get; set; }

        public virtual TAIPrompts? AIPrompt { get; set; }

        /// <summary>
        /// 输出消息类型 1.非流式文本 2.流式文本 3.图片 4.音频 5.视频 6.文件 7.链接 8.卡片 
        /// </summary>
        [Required]
        public int MsgType { get; set; } = 1;
    }
}
