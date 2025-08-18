namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 计数表
    /// </summary>
    [Table("TCount")]
    public partial class TCount:CUD
    {


        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(100)]
        public string Tag { get; set; }



        /// <summary>
        /// 计数
        /// </summary>
        public int Count { get; set; }

    }
}
