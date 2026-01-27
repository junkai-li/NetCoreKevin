using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Email.dto
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    public class SendEmailDto
    {
        /// <summary>
        /// 发件昵称
        /// </summary>
        public string? FromDisplayName { get; set; }


        /// <summary>
        /// 收件人
        /// </summary>
        public List<string> ToAddresses { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get; set; } = "";


        /// <summary>
        /// 内瓤
        /// </summary>
        public string Body { get; set; } = "";


        /// <summary>
        /// 内容是否为Html
        /// </summary>
        public bool IsBodyHtml { get; set; }


        /// <summary>
        /// 附件集合 名称 url键值对
        /// </summary>
        public Dictionary<string, string> FileList { get; set; } = new Dictionary<string, string>();
    }
}
