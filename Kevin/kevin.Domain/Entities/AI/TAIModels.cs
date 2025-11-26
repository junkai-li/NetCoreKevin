using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI模型配置
    /// </summary>
    [Table("TAIModels")]
    [Description("AI模型配置")]
    public partial class TAIModels: CUD_User
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
        [Required]
        [Description("模型秘钥")]
        public string ModelKey { get; set; } = "";
        /// <summary>
        /// 部署名，azure需要使用
        /// </summary>

        [Required]
        [Description("部署名，azure需要使用")]
        public string ModelDescription { get; set; }
    }
}
