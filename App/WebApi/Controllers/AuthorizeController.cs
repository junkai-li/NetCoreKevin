using kevin.Cache.Service;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers
{


    /// <summary>
    /// 系统访问授权模块
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthorizeController : ApiControllerBase
    {
        [IocProperty]
        public IAuthorizeService _IAuthorizeService { get; set; }

        [IocProperty]
        public ICacheService _CacheService { get; set; }
        public AuthorizeController()
        {
        }  
        ///// <summary>
        ///// 获取Token认证信息
        ///// </summary>
        ///// <param name="login">登录信息集合</param>
        ///// <returns></returns>
        [HttpPost("GetToken")]
        [HttpLog("登录", "GetToken获取Token认证信息")]
        public async Task<string> GetToken([FromBody] dtoLogin login)
        {
            return await _IAuthorizeService.GetToken(login); 
        }

        /// <summary>
        /// 通过微信小程序Code获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为weixinkeyid, value 为 code</param>
        /// <returns></returns>
        [HttpPost("GetTokenByWeiXinMiniAppCode")]
        [HttpLog("登录", "GetTokenByWeiXinMiniAppCode通过微信小程序Code获取Token认证信息")]
        public async Task<string> GetTokenByWeiXinMiniAppCode([FromBody] dtoKeyValue keyValue)
        {

            return await _IAuthorizeService.GetTokenByWeiXinMiniAppCode(keyValue);
        }


        /// <summary>
        /// 利用手机号和短信验证码获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 为验证码</param>
        /// <returns></returns>
        [HttpPost("GetTokenBySms")]
        [HttpLog("登录", "GetTokenBySms利用手机号和短信验证码获取Token认证信息")]  
        public async Task<string> GetTokenBySms(dtoKeyValue keyValue)
        {
            return await _IAuthorizeService.GetTokenBySms(keyValue);

        }



        /// <summary>
        /// 发送短信验证手机号码所有权
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 可为空</param>
        /// <returns></returns>
        [HttpPost("SendSmsVerifyPhone")]
        public bool SendSmsVerifyPhone(dtoKeyValue keyValue)
        { 
            return   _IAuthorizeService.SendSmsVerifyPhone(keyValue);

        }


        /// <summary>
        /// 通过微信APP Code获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为weixinkeyid, value 为 code</param>
        /// <returns></returns>
        [HttpPost("GetTokenByWeiXinAppCode")]
        public async Task<string> GetTokenByWeiXinAppCode(dtoKeyValue keyValue)
        {
            return await _IAuthorizeService.GetTokenByWeiXinAppCode(keyValue); 
        }

    }
}
