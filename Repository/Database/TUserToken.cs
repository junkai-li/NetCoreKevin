using Repository.Bases;
using System;

namespace Repository.Database
{

    /// <summary>
    /// 用户Token记录表
    /// </summary>
    public class TUserToken : CD
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        public virtual TUser User { get; set; }



        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }


        /// <summary>
        /// 上一次有效的 tokenid
        /// </summary>
        public Guid? LastId { get; set; }

    }
}
