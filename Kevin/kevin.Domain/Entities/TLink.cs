namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 友情链接表
    /// </summary>
    [Table("TLink")]
    [Description("友情链接表")]
    public partial class TLink : CD_User
    {

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        [Description("名称")]
        public string? Name { get; set; }


        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(50)]
        [Description("地址")]
        public string? Url { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string? Remarks { get; set; }
    }
}
