namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 用户表
    /// </summary>
    [Table("TUser")]
    [Description("用户表")]
    public partial class TUser : CUD
    {

        /// <summary>
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        [Description("用户名")]
        public string Name { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        [Description("昵称")]
        public string NickName { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        [Description("手机号")]
        public string Phone { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        public string Email { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")]
        public string PassWord { get; set; }


        /// <summary>
        /// 角色信息
        /// </summary>
        [Description("角色信息")]
        public Guid RoleId { get; set; }
        public virtual TRole Role { get; set; }


        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [Description("是否超级管理员")]
        public virtual bool IsSuperAdmin { get; set; }

    }
}
