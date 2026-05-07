using Kevin.Email.dto;

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
