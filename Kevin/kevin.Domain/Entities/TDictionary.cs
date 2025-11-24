namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 字典信息表
    /// </summary>
    [Table("TDictionary")]
    public partial class TDictionary : CD
    {


        /// <summary>
        /// 键
        /// </summary>
        [StringLength(200)]
        [Description("键")]
        public string? Key { get; set; }



        /// <summary>
        /// 值
        /// </summary>
        [StringLength(200)]
        [Description("值")]
        public string? Value { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(100)]
        [Description("类型")]
        public string? Type { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }
    }
}
