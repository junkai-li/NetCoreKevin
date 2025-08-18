namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 用户和微信绑定关系表
    /// </summary>

    [Table("TUserBindWeixin")]
    public partial class TUserBindWeixin : CD
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        public virtual TUser User { get; set; }


        /// <summary>
        /// 关联微信账户
        /// </summary>
        public Guid WeiXinKeyId { get; set; }
        public virtual TWeiXinKey WeiXinKey { get; set; }



        /// <summary>
        /// 微信OpenId
        /// </summary>
        [StringLength(100)]
        public string WeiXinOpenId { get; set; }
    }
}
