using kevin.Domain.Attributes;

namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 角色信息表
    /// </summary>
    [Description("角色信息表")]
    [Table("TRole")]
    public partial class TRole : CD
    {

        /// <summary>
        /// 角色名称
        /// </summary>
        [ForeignName]
        [StringLength(100)]
        [Description("角色名称")]
        public string Name { get; set; }


        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(500)]
        [Description("备注信息")]
        public string Remarks { get; set; }



        /// <summary>
        /// 该角色所有用户
        /// </summary>
        [InverseProperty("Role")]
        public virtual List<TUser> RoleUserList { get; set; }


    }
}
