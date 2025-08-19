namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 网站信息配置表
    /// </summary>
    [Table("TWebInfo")]
    [Description("网站信息配置表")]
    public partial class TWebInfo: CD
    {

        /// <summary>
        /// 网站域名
        /// </summary>
        [Description("网站域名")]
        public string WebUrl { get; set; }


        /// <summary>
        /// 管理者名称
        /// </summary>
        [StringLength(100)]
        [Description("管理者名称")]
        public string ManagerName { get; set; }


        /// <summary>
        /// 管理者地址
        /// </summary>
        [Description("管理者地址")]
        [StringLength(200)]
        public string ManagerAddress { get; set; }


        /// <summary>
        /// 管理者电话
        /// </summary>
        [Description("管理者电话")]
        [StringLength(200)]
        public string ManagerPhone { get; set; }


        /// <summary>
        /// 管理者邮箱
        /// </summary>
        [Description("管理者邮箱")]
        [StringLength(100)]
        public string ManagerEmail { get; set; }


        /// <summary>
        /// 网站备案号
        /// </summary>
        [StringLength(200)]
        [Description("网站备案号")]
        public string RecordNumber { get; set; }


        /// <summary>
        /// SEO标题
        /// </summary>
        [StringLength(100)]
        [Description("SEO标题")]
        public string SeoTitle { get; set; }


        /// <summary>
        /// SEO关键字
        /// </summary>
        [StringLength(200)]
        [Description("SEO关键字")]
        public string SeoKeyWords { get; set; }


        /// <summary>
        /// SEO描述
        /// </summary>
        [StringLength(500)]
        [Description("SEO描述")]
        public string SeoDescription { get; set; }


        /// <summary>
        /// 网站底部代码
        /// </summary>
        [Description("网站底部代码")]
        public string FootCode { get; set; }
    }
}
