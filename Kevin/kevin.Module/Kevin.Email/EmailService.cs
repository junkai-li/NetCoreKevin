using Kevin.Email.dto;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Email
{
    /// <summary>
    /// 邮件服务
    /// </summary>
    public class EmailService : IEmailService, IDisposable
    {
        private readonly EmailSetting _emailSetting;

        private readonly SmtpClient _smtpClient;
        public EmailService(IOptionsMonitor<EmailSetting> config)
        {
            _emailSetting = config.CurrentValue;
            _smtpClient = new(_emailSetting.SmtpServer, _emailSetting.Port);
            _smtpClient.Credentials = new NetworkCredential(_emailSetting.AccountName, _emailSetting.AccountPassword);
            _smtpClient.EnableSsl = true;
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> SendEmail(SendEmailDto email)
        {  
            using (MailMessage message = new())
            {
                foreach (var filePath in email.FileList)
                {
                    Attachment data = new(filePath.Key, MediaTypeNames.Application.Octet)
                    {
                        Name = filePath.Value
                    };
                    message.Attachments.Add(data);
                }
                message.From = new(_emailSetting.AccountName, email.FromDisplayName);
                foreach (var toAddress in email.ToAddresses)
                {
                    message.To.Add(new MailAddress(toAddress));
                }
                message.Subject = email.Subject;
                message.Body = email.Body;
                message.IsBodyHtml = email.IsBodyHtml;
                await _smtpClient.SendMailAsync(message);
            }
            return true; 
        }
    }
}
