namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 字典信息表
    /// </summary>
    [Table("TDictionary")]
    public partial class TDictionary : CUD_User
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
        /// 备注信息
        /// </summary>
        [StringLength(500)]
        [Description("备注信息")]
        public string? Remarks { get; set; }

        /// <summary>
        /// 是否系统内置
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }
    }
}
