using Repository.Bases;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Database
{

    /// <summary>
    /// 角色信息表
    /// </summary>
    public class TRole : CD
    {

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remarks { get; set; }



        /// <summary>
        /// 该角色所有用户
        /// </summary>
        [InverseProperty("Role")]
        public virtual List<TUser> RoleUserList { get; set; }


    }
}
