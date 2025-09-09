namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 计数表
    /// </summary>
    [Table("TCount")]
    [Description("计数表")]
    public partial class TCount:CUD
    {


        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(100)]
        [Description("标记")]
        public string? Tag { get; set; }



        /// <summary>
        /// 计数
        /// </summary>
        [Description("计数")]
        public int Count { get; set; }

    }
}
