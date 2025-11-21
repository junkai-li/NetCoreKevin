namespace kevin.Domain.Kevin
{
    [Table("TRolePermission")]
    [Description("角色权限表")]
    public partial class TRolePermission: CD_User
    {
        /// <summary>
        /// 角色编号;
        /// </summary>
        [Description("角色编号")]
        public virtual Guid RoleId { get; set; }
        /// <summary>
        /// 角色编号;
        /// </summary> 
        public virtual TRole? Role { get; set; }

        /// <summary>
        /// 权限编号;
        /// </summary>
        [Description("权限编号")]
        public virtual string?  PermissionId { get; set; }

        /// <summary>
        /// 权限编号;
        /// </summary>
        public virtual TPermission? Permission { get; set; }
    }
}
