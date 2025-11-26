namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 用户和微信绑定关系表
    /// </summary>

    [Table("TUserBindWeixin")]
    [Description("用户和微信绑定关系表")]
    public partial class TUserBindWeixin : CD
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public long UserId { get; set; }
        public virtual TUser? User { get; set; }


        /// <summary>
        /// 关联微信账户
        /// </summary>
        [Description("关联微信账户")]
        public long WeiXinKeyId { get; set; }
        public virtual TWeiXinKey? WeiXinKey { get; set; }



        /// <summary>
        /// 微信OpenId
        /// </summary>
        [StringLength(100)]
        [Description("微信OpenId")]
        public string? WeiXinOpenId { get; set; }
    }
}
