using kevin.Share.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IAuthorizeService : IBaseService
    {
        ///// <summary>
        ///// 获取Token认证信息
        ///// </summary>
        ///// <param name="login">登录信息集合</param>
        ///// <returns></returns>
        Task<string> GetToken([FromBody] dtoLogin login); 

        /// <summary>
        /// 利用手机号和短信验证码获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 为验证码</param>
        /// <returns></returns>
        Task<string> GetTokenBySms(dtoKeyValue keyValue);
        /// <summary>
        /// 发送短信验证手机号码所有权
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 可为空</param>
        /// <returns></returns>
        bool SendSmsVerifyPhone(dtoKeyValue keyValue); 


    }
}
