using Repository.Bases;

namespace Repository.Database
{


    /// <summary>
    /// 微信商户平台账户配置表
    /// </summary>
    public class TWeiXinKey : CD
    {


        /// <summary>
        /// WxAppId
        /// </summary>
        public string WxAppId { get; set; }


        /// <summary>
        /// WxAppSecret
        /// </summary>
        public string WxAppSecret { get; set; }


        /// <summary>
        /// MchId
        /// </summary>
        public string MchId { get; set; }


        /// <summary>
        /// MchKey
        /// </summary>
        public string MchKey { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// AppId 类型，['App','MiniApp','H5','Native']
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
