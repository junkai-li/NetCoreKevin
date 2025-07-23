namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 微信商户平台账户配置表
    /// </summary>
    public class TWeiXinKey : CD
    {


        /// <summary>
        /// WxAppId
        /// </summary>
        [StringLength(100)]
        public string WxAppId { get; set; }


        /// <summary>
        /// WxAppSecret
        /// </summary>
        [StringLength(100)]
        public string WxAppSecret { get; set; }


        /// <summary>
        /// MchId
        /// </summary>
        public string MchId { get; set; }


        /// <summary>
        /// MchKey
        /// </summary>]
        [StringLength(100)]
        public string MchKey { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// AppId 类型，['App','MiniApp','H5','Native']
        /// </summary>
        [StringLength(100)]
        public string Type { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        public string Remarks { get; set; }
    }
}
