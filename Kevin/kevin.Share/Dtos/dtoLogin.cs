using System.ComponentModel.DataAnnotations;

namespace kevin.Share.Dtos
{

    /// <summary>
    /// 登录信息
    /// </summary>
    public class dtoLogin
    {


        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不可以空")]
        public string Name { get; set; }


        /// <summary>
        /// PasswordHash
        /// </summary> 
        public string PasswordHash { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不可以为空")]
        public string PassWord { get; set; }

        /// <summary>
        /// 租户id
        /// </summary>
        [Required(ErrorMessage = "租户id不可以为空")]
        public int TenantId { get; set; }

    }
}
