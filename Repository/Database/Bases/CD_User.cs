using Repository.Bases;
using System;

namespace Repository.Database.Bases
{

    /// <summary>
    /// 创建，编辑，删除，并关联了用户
    /// </summary>
    public class CD_User : CD
    {

        /// <summary>
        /// 创建人ID
        /// </summary>
        public Guid CreateUserId { get; set; }
        public virtual TUser CreateUser { get; set; }



        /// <summary>
        /// 删除人ID
        /// </summary>
        public Guid? DeleteUserId { get; set; }
        public virtual TUser DeleteUser { get; set; }
    }
}
