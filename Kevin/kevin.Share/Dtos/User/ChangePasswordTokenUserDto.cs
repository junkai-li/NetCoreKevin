using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.User
{
    /// <summary>
    /// 变更当前登陆人密码
    /// </summary>
    public class ChangePasswordTokenUserDto
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required]
        public string OldPwd { get; set; } = "";


        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        public string NewPwd { get; set; } = "";


    }
}
