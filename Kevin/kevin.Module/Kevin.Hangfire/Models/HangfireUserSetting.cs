using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Hangfire.Models
{
    public class HangfireUserSetting
    {
        /// <summary>
        /// 账户登录名，默认为 admin
        /// </summary>
        public string Login { get; set; }= "admin";

        /// <summary>
        /// 账户密码，默认为 admin
        /// </summary>
        public string Password { get; set; }= "admin";
    }
}
