using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Repository.Database;
using System;
using System.Linq;
using WebApi.Filters;
using WebApi.Models.v1.User;

namespace WebApi.Controllers.v1
{


    /// <summary>
    /// 用户数据操作控制器
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [Authorize]
    [JwtTokenVerify]
    [ApiController]
    public class UserController : ControllerBase
    {


        private readonly dbContext db;

        public UserController(dbContext context)
        {
            db = context;
        }


        /// <summary>
        /// 获取微信小程序OpenId
        /// </summary>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        /// <param name="code">微信临时code</param>
        /// <returns>openid,userid</returns>
        /// <remarks>传入租户ID和微信临时 code 获取 openid，如果 openid 在系统有中对应用户，则一并返回用户的ID值，否则用户ID值为空</remarks>
        [HttpGet("GetWeiXinMiniAppOpenId")]
        public string GetWeiXinMiniAppOpenId(Guid weixinkeyid, string code)
        {


            var weixinkey = db.TWeiXinKey.Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);

            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;

            return openid;


        }



        /// <summary>
        /// 获取微信小程序手机号
        /// </summary>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据</param>
        /// <param name="code">微信临时code</param>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        [HttpGet("GetWeiXinMiniAppPhone")]
        public string GetWeiXinMiniAppPhone(string iv, string encryptedData, string code, Guid weixinkeyid)
        {


            var weixinkey = db.TWeiXinKey.Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);


            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;
            string sessionkey = wxinfo.sessionkey;


            var strJson = Libraries.WeiXin.MiniApp.WeiXinHelper.DecryptionData(encryptedData, sessionkey, iv);

            var user = db.TUserBindWeixin.Where(t => t.WeiXinOpenId == openid & t.WeiXinKeyId == weixinkeyid).Select(t => t.User).FirstOrDefault();

            user.Phone = Common.Json.JsonHelper.GetValueByKey(strJson, "phoneNumber");

            db.SaveChanges();

            return user.Phone;
        }



        /// <summary>
        /// 通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [HttpGet("GetUser")]
        [CacheData(TTL = 60, UseToken = true)]
        public dtoUser GetUser(Guid userId)
        {

            if (userId == default)
            {
                userId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("userId"));
            }

            var user = db.TUser.Where(t => t.Id == userId && t.IsDelete == false).Select(t => new dtoUser
            {
                Name = t.Name,
                NickName = t.NickName,
                Phone = t.Phone,
                Email = t.Email,
                Role = t.Role.Name,
                CreateTime = t.CreateTime
            }).FirstOrDefault();

            return user;
        }



        /// <summary>
        /// 通过短信验证码修改账户手机号
        /// </summary>
        /// <param name="keyValue">key 为新手机号，value 为短信验证码</param>
        /// <returns></returns>
        [HttpPost("EditUserPhoneBySms")]
        public bool EditUserPhoneBySms([FromBody] dtoKeyValue keyValue)
        {

            if (Actions.AuthorizeAction.SmsVerifyPhone(keyValue))
            {
                var userId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("userId"));

                string phone = keyValue.Key.ToString();


                var checkPhone = db.TUser.Where(t => t.Id != userId && t.Phone == phone).Count();

                var user = db.TUser.Where(t => t.Id == userId).FirstOrDefault();


                var isMergeUser = false;

                if (isMergeUser)
                {
                    //获取目标手机号绑定的账户ID
                    var phoneUserId = db.TUser.Where(t => t.Phone == phone).Select(t => t.Id).FirstOrDefault();

                    user.Phone = phone;

                    db.SaveChanges();

                    //如果目标手机号绑定用户，则进行数据合并动作
                    if (phoneUserId != default)
                    {
                        //将手机号对应的用户移除，合并数据到新的账号
                        Common.DBHelper.MergeUser(phoneUserId, user.Id);
                    }

                    return true;
                }
                else
                {
                    if (checkPhone == 0)
                    {
                        user.Phone = phone;

                        db.SaveChanges();

                        return true;
                    }
                    else
                    {
                        HttpContext.Response.StatusCode = 400;

                        HttpContext.Items.Add("errMsg", "User.EditUserPhoneBySms.'The target mobile number has been bound by another account'");

                        return false;
                    }
                }


            }
            else
            {
                HttpContext.Response.StatusCode = 400;

                HttpContext.Items.Add("errMsg", "User.EditUserPhoneBySms.'Error in SMS verification code'");

                return false;
            }
        }




        /// <summary>
        /// 通过短信验证码修改账户密码</summary>
        /// <param name="keyValue">key为新密码，value为短信验证码</param>
        /// <returns></returns>
        [HttpPost("EditUserPassWordBySms")]
        public bool EditUserPassWordBySms([FromBody] dtoKeyValue keyValue)
        {

            var userId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("userId"));

            string phone = db.TUser.Where(t => t.Id == userId).Select(t => t.Phone).FirstOrDefault();

            string smsCode = keyValue.Value.ToString();

            var checkSms = Actions.AuthorizeAction.SmsVerifyPhone(new dtoKeyValue { Key = phone, Value = smsCode });

            if (checkSms)
            {
                string password = keyValue.Key.ToString();

                if (!string.IsNullOrEmpty(password))
                {
                    var user = db.TUser.Where(t => t.Id == userId).FirstOrDefault();

                    user.PassWord = password;

                    db.SaveChanges();


                    var tokenList = db.TUserToken.Where(t => t.UserId == userId).ToList();

                    db.TUserToken.RemoveRange(tokenList);

                    db.SaveChanges();

                    return true;
                }
                else
                {
                    HttpContext.Response.StatusCode = 400;

                    HttpContext.Items.Add("errMsg", "User.EditUserPassWordBySms.'New password is not allowed to be empty'");

                    return false;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;

                HttpContext.Items.Add("errMsg", "User.EditUserPassWordBySms.'Error in SMS verification code'");

                return false;
            }

        }

    }
}