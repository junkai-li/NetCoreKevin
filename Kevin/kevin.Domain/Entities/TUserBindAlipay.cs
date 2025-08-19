namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 用户和支付宝绑定关系表
    /// </summary>
    [Table("TUserBindAlipay")]
    [Description("用户和支付宝绑定关系表")]
    public   partial  class TUserBindAlipay :CD
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public Guid UserId { get; set; }
        public virtual TUser User { get; set; }


        /// <summary>
        /// 关联支付宝账户
        /// </summary>
        [Description("关联支付宝账户")]
        public Guid AlipayKeyId { get; set; }
        public virtual TAlipayKey AlipayKey { get; set; }



        /// <summary>
        /// 支付宝UserId
        /// </summary>
        [StringLength(100)]
        [Description("支付宝UserId")]
        public string AlipayUserId { get; set; }
    }
}
