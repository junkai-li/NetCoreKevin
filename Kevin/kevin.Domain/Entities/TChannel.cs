namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 频道信息表
    /// </summary>
    [Table("TChannel")]
    [Description("频道信息表")]
    public partial class TChannel : CD_User
    {

        /// <summary>
        /// 频道名称
        /// </summary>
        [StringLength(100)]
        [Description("频道名称")]
        public string? Name { get; set; }

        [Description("排序")]
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        [Description("备注")]
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }




        /// <summary>
        /// 所包含的类别记录数据
        /// </summary>
        public virtual List<TCategory>? TCategory { get; set; }
    }
}
