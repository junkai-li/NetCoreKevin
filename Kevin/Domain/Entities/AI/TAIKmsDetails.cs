using kevin.Domain.Share.Enums;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 知识库文档详情
    /// </summary>

    [Table("TAIKmsDetails")]
    [Description("TAIKmsDetails")]
    public class TAIKmsDetails : CUD_User
    { 
        public long KmsId { get; set; }
        public virtual TAIKmss? Kms { get; set; } 

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; } = ""; 
        public long FileId { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; } = ""; 
        /// <summary>
        /// 数据数量
        /// </summary>
        public int? DataCount { get; set; }  
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ImportKmsStatus? Status { get; set; } = ImportKmsStatus.Loadding;  
        /// <summary>
        /// 异常消息
        /// </summary>
        public string? ErrorMessage { get; set; } = "";
    }
}
