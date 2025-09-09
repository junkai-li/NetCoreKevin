namespace kevin.Domain.Kevin
{
    /// <summary>
    /// 分片上传时的切片文件记录表
    /// </summary>
    [Table("TFileGroupFile")]
    [Description("分片上传时的切片文件记录表")]
    public partial class TFileGroupFile : CD
    {


        /// <summary>
        /// 文件ID
        /// </summary>
        [Description("文件ID")]
        public Guid FileId { get; set; }
        public virtual TFile? File { get; set; }


        /// <summary>
        /// 文件索引值
        /// </summary>
        [Description("文件索引值")]
        public int Index { get; set; }


        /// <summary>
        /// 文件保存路径
        /// </summary>
        [StringLength(300)]
        [Description("文件保存路径")]
        public string? Path { get; set; }

    }
}
