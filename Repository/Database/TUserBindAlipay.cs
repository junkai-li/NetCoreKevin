using Repository.Bases;
using System;

namespace Repository.Database
{


    /// <summary>
    /// 用户和支付宝绑定关系表
    /// </summary>
    public class TUserBindAlipay :CD
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        public virtual TUser User { get; set; }


        /// <summary>
        /// 关联支付宝账户
        /// </summary>
        public Guid AlipayKeyId { get; set; }
        public virtual TAlipayKey AlipayKey { get; set; }



        /// <summary>
        /// 支付宝UserId
        /// </summary>
        public string AlipayUserId { get; set; }
    }
}
