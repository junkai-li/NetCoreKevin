using Repository.Bases;
using System;

namespace Repository.Database
{


    /// <summary>
    /// 用户和微信绑定关系表
    /// </summary>
    public class TUserBindWeixin : CD
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
        public string WeiXinOpenId { get; set; }
    }
}
