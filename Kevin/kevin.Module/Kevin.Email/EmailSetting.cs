using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Email
{
    /// <summary>
    /// 邮件配置
    /// </summary>
    public class EmailSetting
    {
        /// <summary>
        /// SMTP服务器地址
        /// </summary>
        public string SmtpServer { get; set; }= string.Empty;

        /// <summary>
        /// SMTP账户 
        /// </summary>
        public string AccountName { get; set; } = string.Empty;

        /// <summary>
        /// SMTP密码
        /// </summary>
        public string AccountPassword { get; set; } = string.Empty;

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 587;


    }
}
