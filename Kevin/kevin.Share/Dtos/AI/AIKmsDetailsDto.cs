using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Enums;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIKmsDetailsDto : CUD_User_Dto
    {
        public long KmsId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary> 
        public long? FileId { get; set; }
        /// <summary>
        /// 数据类型 Text/Word/PDF/Markdown/Html
        /// </summary>
        public string? FileType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; } = "";
        /// <summary>
        /// 内容
        /// </summary>
        public string ContentName { get; set; } = "";
        /// <summary>
        /// 远程地址
        /// </summary>
        public string Url { get; set; } = "";
        /// <summary>
        /// 数据数量
        /// </summary>
        public int? DataCount { get; set; }
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
