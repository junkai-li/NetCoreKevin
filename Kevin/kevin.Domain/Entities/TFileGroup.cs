namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 文件分片上传状态记录表
    /// </summary>
    [Table("TFileGroup")]
    [Description("文件分片上传状态记录表")]
    public partial class TFileGroup :CUD
    {


        /// <summary>
        /// 文件ID
        /// </summary>
        [Description("文件ID")]
        public Guid FileId { get; set; }
        public virtual TFile File { get; set; }



        /// <summary>
        /// 文件唯一值
        /// </summary>
        [StringLength(300)]
        [Description("文件唯一值")]
        public string Unique { get; set; }


        /// <summary>
        /// 分片数
        /// </summary>
        [Description("分片数")]
        public int Slicing { get; set; }



        /// <summary>
        /// 合成状态
        /// </summary>
        [Description("合成状态")]
        public bool Issynthesis { get; set; }



        /// <summary>
        /// 是否已完整传输
        /// </summary>
        [Description("是否已完整传输")]
        public bool Isfull { get; set; }
    }
}
