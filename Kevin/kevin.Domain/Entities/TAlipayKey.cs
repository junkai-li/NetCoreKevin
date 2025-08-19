

namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 支付宝平台账户配置表
    /// </summary>
    [Table("TAlipayKey")]
    [Description("支付宝平台账户配置表")]
    public partial class TAlipayKey : CD
    {


        /// <summary>
        /// AppId
        /// </summary>
        [MaxLength(200)]
        [Description("AppId")]
        public string AppId { get; set; }


        /// <summary>
        /// App私钥
        /// </summary>
        [MaxLength(100)]
        [Description("App私钥")]
        public string AppPrivateKey { get; set; }


        /// <summary>
        /// 支付宝公钥
        /// </summary>
        [StringLength(200)]
        [Description("支付宝公钥")]
        public string AlipayPublicKey { get; set; }


        /// <summary>
        /// 支付宝加密解密密钥
        /// </summary>
        [StringLength(200)]
        [Description("支付宝加密解密密钥")]
        public string AesKey { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        [StringLength(1000)]
        public string Remarks { get; set; }
    }
}
