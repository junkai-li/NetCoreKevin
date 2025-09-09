﻿using kevin.Domain.Bases;
using kevin.Domain.Kevin;

namespace kevin.Domain.Bases
{


    /// <summary>
    /// 创建，编辑，删除，并关联了用户
    /// </summary>
    public class CUD_User : CUD
    {


        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public Guid CreateUserId { get; set; }
        public virtual TUser? CreateUser { get; set; }


        /// <summary>
        /// 编辑人ID
        /// </summary>
        [Description("编辑人ID")]
        public Guid? UpdateUserId { get; set; }
        public virtual TUser? UpdateUser { get; set; }


        /// <summary>
        /// 删除人ID
        /// </summary>
        [Description("删除人ID")]
        public Guid? DeleteUserId { get; set; }
        public virtual TUser? DeleteUser { get; set; }


    }
}
