using kevin.Domain.Share.Dtos.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIAppsDto : CUD_User_Dto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required]
        public string Describe { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Required]
        public string Icon { get; set; } = "windows";

        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 会话模型ID
        /// </summary>
        [Required]
        public string? ChatModelID { get; set; }

        /// <summary>
        /// Embedding 模型Id
        /// </summary>
        [Required]
        public string? EmbeddingModelID { get; set; }

        public string? RerankModelID { get; set; }


        public string? ImageModelID { get; set; }
        /// <summary>
        /// 温度
        /// </summary> 
        [DefaultValue(70)]
        public double Temperature { get; set; } = 70f;

        /// <summary>
        /// 知识库ID列表
        /// </summary>
        [MaxLength(1000)]
        public string? KmsIdList { get; set; }

        /// <summary>
        /// API调用秘钥
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// 相似度
        /// </summary> 
        [DefaultValue(60)]
        public double Relevance { get; set; } = 60f;

        /// <summary>
        /// 提问最大token数
        /// </summary> 
        [DefaultValue(2048)]
        public int MaxAskPromptSize { get; set; } = 2048;
        /// <summary>
        /// 向量匹配数
        /// </summary> 
        [DefaultValue(3)]
        public int MaxMatchesCount { get; set; } = 3;


        [DefaultValue(20)]
        public int RerankCount { get; set; } = 20;
        /// <summary>
        /// 回答最大token数
        /// </summary> 
        [DefaultValue(2048)]
        public int AnswerTokens { get; set; } = 2048;

        /// <summary>
        /// 提示词绑定
        /// </summary>
        public long AIPromptID { get; set; }

        public virtual string? AIPromptName { get; set; }
    }
}
