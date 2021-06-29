using System;

namespace Repository.Database
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
        public string ManagerName { get; set; }


        /// <summary>
        /// 管理者地址
        /// </summary>
        public string ManagerAddress { get; set; }


        /// <summary>
        /// 管理者电话
        /// </summary>
        public string ManagerPhone { get; set; }


        /// <summary>
        /// 管理者邮箱
        /// </summary>
        public string ManagerEmail { get; set; }


        /// <summary>
        /// 网站备案号
        /// </summary>
        public string RecordNumber { get; set; }


        /// <summary>
        /// SEO标题
        /// </summary>
        public string SeoTitle { get; set; }


        /// <summary>
        /// SEO关键字
        /// </summary>
        public string SeoKeyWords { get; set; }


        /// <summary>
        /// SEO描述
        /// </summary>
        public string SeoDescription { get; set; }


        /// <summary>
        /// 网站底部代码
        /// </summary>
        public string FootCode { get; set; }
    }
}
