using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.JwtBearer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace WebApi.Libraries.Verify
{
    public class JwtToken
    {

        /// <summary>
        /// 通过Key获取Claims中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetClaims(string key)
        {

            try
            {
                var Authorization = Http.HttpContext.Current().Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var securityToken = new JwtSecurityToken(Authorization);

                var value = securityToken.Claims.ToList().Where(t => t.Type == key).FirstOrDefault().Value;

                return value;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// 生成一个Token
        /// </summary>
        /// <returns></returns>
        public static string GetToken(Claim[] claims)
        {
            var conf = Libraries.Start.StartConfiguration.configuration;

            var jwtSettings = new JwtSettings();
            conf.Bind("JwtSettings", jwtSettings);

            //对称秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

            //签名证书(秘钥，加密算法)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //生成token
            var token = new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(30), creds);

            var ret = new JwtSecurityTokenHandler().WriteToken(token);

            return ret;
        }

    }
}
