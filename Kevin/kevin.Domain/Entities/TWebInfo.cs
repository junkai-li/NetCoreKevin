namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 网站信息配置表
    /// </summary>
    public class TWebInfo
    {

        /// <summary>
        /// 标识ID
        /// </summary>
        public Guid Id { get; set; }



        /// <summary>
        /// 网站域名
        /// </summary>
        public string WebUrl { get; set; }


        /// <summary>
        /// 管理者名称
        /// </summary>
        [StringLength(100)]
        public string ManagerName { get; set; }


        /// <summary>
        /// 管理者地址
        /// </summary>
        [StringLength(200)]
        public string ManagerAddress { get; set; }


        /// <summary>
        /// 管理者电话
        /// </summary>
        [StringLength(200)]
        public string ManagerPhone { get; set; }


        /// <summary>
        /// 管理者邮箱
        /// </summary>
        [StringLength(100)]
        public string ManagerEmail { get; set; }


        /// <summary>
        /// 网站备案号
        /// </summary>
        [StringLength(200)]
        public string RecordNumber { get; set; }


        /// <summary>
        /// SEO标题
        /// </summary>
        [StringLength(100)]
        public string SeoTitle { get; set; }


        /// <summary>
        /// SEO关键字
        /// </summary>
        [StringLength(200)]
        public string SeoKeyWords { get; set; }


        /// <summary>
        /// SEO描述
        /// </summary>
        [StringLength(500)]
        public string SeoDescription { get; set; }


        /// <summary>
        /// 网站底部代码
        /// </summary>
        public string FootCode { get; set; }
    }
}
