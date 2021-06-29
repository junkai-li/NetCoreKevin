using Repository.Bases;

namespace Repository.Database
{


    /// <summary>
    /// 支付宝平台账户配置表
    /// </summary>
    public class TAlipayKey : CD
    {


        /// <summary>
        /// AppId
        /// </summary>
        public string AppId { get; set; }


        /// <summary>
        /// App私钥
        /// </summary>
        public string AppPrivateKey { get; set; }


        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public string AlipayPublicKey { get; set; }


        /// <summary>
        /// 支付宝加密解密密钥
        /// </summary>
        public string AesKey { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
