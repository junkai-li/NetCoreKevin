using Kevin.Email.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sendParams">发送参数</param>
        /// <returns></returns>
         Task<bool> SendEmail(SendEmailDto sendParams);
    }
}
