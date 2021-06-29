using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public static class EmailHelper
    {


        /// <summary>
        /// 邮件发送方法
        /// </summary>
        /// <param name="receiveAccount">收件人账户</param>
        /// <param name="zhuti">标题</param>
        /// <param name="neirong">内容</param>
        public static bool Send(string receiveAccount, string title, string content)
        {

            try
            {

                string sendAccount = "XXXX@qq.com";
                string sendAccountPwd = "密码";

                string server = "smtp.qq.com";

                var client = new SmtpClient();
                client.Host = server;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(sendAccount, sendAccountPwd);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                var message = new MailMessage(sendAccount, receiveAccount);
                message.Subject = title;
                message.Body = content;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                client.Send(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
