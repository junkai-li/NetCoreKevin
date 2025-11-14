using Kevin.Common.Helper;
using Microsoft.AspNetCore.Identity;

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
        [Description("用户名")]
        public string? Name { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        [Description("昵称")]
        public string? NickName { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        [Description("手机号")]
        public string? Phone { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        public string? Email { get; set; }

        /// <summary>
        /// 是否系统用户
        /// </summary>
        public bool IsSystem { get; set; } = true;

        /// <summary>
        /// 密码Hash
        /// </summary>
        [Description("密码Hash")]
        public   string? PasswordHash { get; set; } 
         
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [Description("是否超级管理员")]
        public virtual bool IsSuperAdmin { get; set; }

        public void ChangePassword(string newPassword)
        {
            PasswordHash = new HashHelper().SHA256Hash(newPassword);
        }

        public bool ValidatePassword(string password)
        {
            return PasswordHash == new HashHelper().SHA256Hash(password);
        }

    }
}
