using Kevin.Common.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Kevin.Common.App
{
    public class JwtToken
    {
        /// <summary>
        /// 通过Key获取Claims中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetClaims(string key, IHttpContextAccessor httpContext)
        {

            try
            {
                if (httpContext != default && httpContext.HttpContext != default )
                {
                    var jwt = ConfigHelper.GetSection("Jwt");
                    var Authorization = httpContext.Current().Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrEmpty(Authorization) || !IsBearerValidJwt(Authorization))
                    {
                        Authorization = httpContext.Current().Request.Query["Authorization"].ToString();
                    }
                    var securityToken = new JwtSecurityToken(Authorization.Replace("Bearer ", ""));

                    var value = securityToken.Claims.ToList().Where(t => t.Type.ToLower() == key.ToLower()).FirstOrDefault().Value;

                    return value;
                }
                return  default;
            }
            catch
            {
                return default;
            }
        }
        /// <summary>
        /// 简单验证JWT的有效性 ，适用于从Authorization头提取的Token
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="secretKey"></param>
        /// <param name="validIssuer"></param>
        /// <param name="validAudience"></param>
        /// <returns></returns>
        public static bool IsBearerValidJwt(string authorizationHeader)
        {
            // 1. 检查 Authorization 头格式
            if (string.IsNullOrEmpty(authorizationHeader) ||
                !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return false;

            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
                return false; 
            return true;
        }


        /// <summary>
        /// 生成一个Token
        /// </summary>
        /// <returns></returns>
        //[Obsolete("已使用IDS代替", false)]
        //public static string GetToken(Claim[] claims)
        //{
        //    var conf = StartConfiguration.configuration;

        //    var jwtSettings = StartConfiguration.configuration.GetSection("JwtSettings").Get<SignalrSetting>();

        //    //对称秘钥
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        //    //签名证书(秘钥，加密算法)
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    //生成token
        //    var token = new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(30), creds);

        //    var ret = new JwtSecurityTokenHandler().WriteToken(token);

        //    return ret;
        //}

    }
}
