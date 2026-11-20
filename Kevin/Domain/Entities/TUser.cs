using Kevin.Common.Helper;
using Web.Global.Exceptions;

namespace kevin.Domain.Entities
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
        /// 状态 true启动 false禁用
        /// </summary>
        [Description("状态")]
        public bool Status { get; set; } = true;

        /// <summary>
        ///最近登陆时间
        /// </summary>
        [Description("最近登陆时间")]
        public DateTime? RecentLoginTime { get; set; }

        /// <summary>
        /// 密码Hash
        /// </summary>
        [Description("密码Hash")]
        public string? PasswordHash { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [Description("是否超级管理员")]
        public virtual bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 第三方关联Id：钉钉、微信、QQ等
        /// </summary> 
        [Description("第三方关联Id：钉钉、微信、QQ等")]
        public string? CorrelationId { get; set; } = "";

        /// <summary>
        /// 登录失败次数
        /// </summary>
        [Description("登录失败次数")]
        public int FailedCount { get; set; } = 0;

        /// <summary>
        /// 账号锁定截止时间，null 表示未锁定
        /// </summary>
        [Description("账号锁定截止时间")]
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// 密码复杂度默认正则（数字/大写/小写/特殊符号至少三种，长度>=8）
        /// </summary>
        private const string DefaultPasswordRegex = "^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$|^(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$";

        public void ChangePassword(string newPassword, string? passwordRegex = null)
        {
            var regex = string.IsNullOrWhiteSpace(passwordRegex) ? DefaultPasswordRegex : passwordRegex;
            if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword ?? "", regex))
            {
                throw new UserFriendlyException("密码不符合复杂度要求");
            }
            PasswordHash = new HashHelper().SHA256Hash(newPassword);
        }

        public bool ValidatePassword(string password)
        {
            return PasswordHash == new HashHelper().SHA256Hash(password);
        }
        /// <summary>
        /// 删除校验
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteCheck()
        {
            if (this.IsSuperAdmin)
            {
                throw new InvalidOperationException("超级管理员不能删除");
            }
        }

    }
}
