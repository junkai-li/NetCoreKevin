﻿using kevin.Domain.Share.Interfaces;
using kevin.Share.Dtos;
using Kevin.Common.App.Global;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Auth
{

    /// <summary>
    /// 认证模块静态方法
    /// </summary>
    public static class AuthorizeAction
    {


        /// <summary>
        /// 校验短信身份验证码
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 为验证码</param>
        /// <returns></returns>
        public static bool SmsVerifyPhone(dtoKeyValue keyValue)
        {
            string phone = keyValue.Key.ToString();

            string key = "VerifyPhone_" + phone;

            var code = GlobalServices.ServiceProvider.GetService<ICacheService>().GetString(key);

            if (string.IsNullOrEmpty(code) == false && code == keyValue.Value.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
