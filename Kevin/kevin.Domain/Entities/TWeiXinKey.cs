namespace kevin.Domain.Entities
{


    /// <summary>
    /// 微信商户平台账户配置表
    /// </summary>
    [Table("TWeiXinKey")]
    [Description("微信商户平台账户配置表")]
    public partial class TWeiXinKey : CD
    { 
        /// <summary>
        /// WxAppId
        /// </summary>
        [StringLength(100)]
        [Description("WxAppId")]
        public string? WxAppId { get; set; }


        /// <summary>
        /// WxAppSecret
        /// </summary>
        [StringLength(100)]
        [Description("WxAppSecret")]
        public string? WxAppSecret { get; set; }


        /// <summary>
        /// MchId
        /// </summary>
        [Description("MchId")]
        public string? MchId { get; set; }


        /// <summary>
        /// MchKey
        /// </summary>]
        [StringLength(100)]
        [Description("MchKey")]
        public string? MchKey { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }


        /// <summary>
        /// AppId 类型，['App','MiniApp','H5','Native']
        /// </summary>
        [StringLength(100)]
        [Description("AppId 类型，['App','MiniApp','H5','Native']")]
        public string? Type { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        [Description("备注")]
        public string? Remarks { get; set; }
    }
}
