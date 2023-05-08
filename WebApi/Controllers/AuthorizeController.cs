using IdentityModel.Client;
using kevin.Domain.Kevin;
using kevin.Domain.Share.Interfaces;
using kevin.Ioc.IocAttrributes;
using kevin.Share.Dtos;
using Medallion.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.v1._;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Controllers.Bases;

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
        public IUserService _IUserService { get; set; }

        [IocProperty]
        public ICacheService _CacheService { get; set; }
        public AuthorizeController()
        {
        }

        /// <summary>
        /// GetErr
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetDb")]
        public string GetDb()
        {
            db.TUser.Count();
            return db.TUser.Count().ToString();

        }

        /// <summary>
        /// GetErr
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetRdisDb")]
        public string GetRdisDb()
        {
            return _CacheService.GetString("1");

        }

        ///// <summary>
        ///// 获取Token认证信息
        ///// </summary>
        ///// <param name="login">登录信息集合</param>
        ///// <returns></returns>
        [HttpPost("GetToken")]
        public async Task<string> GetToken([FromBody] dtoLogin login)
        {
            var clinet = new HttpClient();
            var disco = await clinet.GetDiscoveryDocumentAsync(Configuration["JwtOptions:Authority"]);
            if (disco.IsError)
            {
                throw new UserFriendlyException("登录异常");
            }

            var tokenResponse = await clinet.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = Configuration["IdentityServerInfo:ClientId"],
                ClientSecret = Configuration["IdentityServerInfo:ClientSecret"],
                Scope = Configuration["IdentityServerInfo:Scope"],
                UserName = login.Name,
                Password = login.PassWord,
            });
            if (tokenResponse.IsError)
            {
                throw new UserFriendlyException(tokenResponse.ErrorDescription);
            }
            //保存刷新令牌
            _CacheService.SetString(tokenResponse.AccessToken, tokenResponse.RefreshToken, TimeSpan.FromDays(2));
            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// 通过微信小程序Code获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为weixinkeyid, value 为 code</param>
        /// <returns></returns>
        [HttpPost("GetTokenByWeiXinMiniAppCode")]
        public async Task<string> GetTokenByWeiXinMiniAppCode([FromBody] dtoKeyValue keyValue)
        {

            var weixinkeyid = Guid.Parse(keyValue.Key.ToString());
            string code = keyValue.Value.ToString();

            var weixinkey = db.TWeiXinKey.Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);


            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;
            string sessionkey = wxinfo.sessionkey;

            var user = db.TUserBindWeixin.Where(t => t.WeiXinOpenId == openid).Select(t => t.User).FirstOrDefault();

            if (user == null)
            {

                bool isAction = false;
                while (isAction == false)
                {
                    var lock1 = distLock.AcquireLock("GetTokenByWeiXinMiniAppCode" + openid, TimeSpan.FromSeconds(5));
                    if (lock1 != null)
                    {
                        using (lock1)
                        {
                            isAction = true;
                            user = db.TUserBindWeixin.Where(t => t.WeiXinOpenId == openid).Select(t => t.User).FirstOrDefault();
                            if (user == null)
                            {
                                //注册一个只有基本信息的账户出来
                                user = new TUser();

                                user.Id = Guid.NewGuid();
                                user.IsDelete = false;
                                user.CreateTime = DateTime.Now;
                                user.Name = DateTime.Now.ToString() + "微信小程序新用户";
                                user.NickName = user.Name;
                                user.PassWord = Guid.NewGuid().ToString();

                                //开发时记得调整这个值
                                user.RoleId = default;

                                db.TUser.Add(user);

                                db.SaveChanges();

                                TUserBindWeixin userBind = new();
                                userBind.Id = Guid.NewGuid();
                                userBind.IsDelete = false;
                                userBind.CreateTime = DateTime.Now;
                                userBind.UserId = user.Id;
                                userBind.WeiXinKeyId = weixinkeyid;
                                userBind.WeiXinOpenId = openid;

                                db.TUserBindWeixin.Add(userBind);

                                db.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
            }
            var clinet = new HttpClient();
            var disco = await clinet.GetDiscoveryDocumentAsync(Configuration["JwtOptions:Authority"]);
            if (disco.IsError)
            {
                throw new UserFriendlyException("登录异常");
            }

            var tokenResponse = await clinet.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = Configuration["UMIdentityServerInfo:ClientId"],
                ClientSecret = Configuration["UMIdentityServerInfo:ClientSecret"],
                Scope = Configuration["UMIdentityServerInfo:Scope"],
                UserName = user.Id.ToString(),
                Password = user.Id.ToString(),
            });
            if (tokenResponse.IsError)
            {
                throw new UserFriendlyException(tokenResponse.ErrorDescription);
            }
            //保存刷新令牌
            _CacheService.SetString(tokenResponse.AccessToken, tokenResponse.RefreshToken, TimeSpan.FromDays(2));
            return tokenResponse.AccessToken;
        }


        /// <summary>
        /// 利用手机号和短信验证码获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 为验证码</param>
        /// <returns></returns>
        [HttpPost("GetTokenBySms")]
        public async Task<string> GetTokenBySms(dtoKeyValue keyValue)
        {
            if (Web.Auth.AuthorizeAction.SmsVerifyPhone(keyValue))
            {
                string phone = keyValue.Key.ToString();


                var user = db.TUser.Where(t => t.IsDelete == false && (t.Name == phone || t.Phone == phone) && t.RoleId == default).FirstOrDefault();

                if (user == null)
                {
                    //注册一个只有基本信息的账户出来

                    user = new TUser();

                    user.Id = Guid.NewGuid();
                    user.IsDelete = false;
                    user.CreateTime = DateTime.Now;
                    user.Name = DateTime.Now.ToString() + "手机短信新用户";
                    user.NickName = user.Name;
                    user.PassWord = Guid.NewGuid().ToString();
                    user.Phone = phone;

                    db.TUser.Add(user);

                    db.SaveChanges();
                }

                return await GetToken(new dtoLogin { Name = user.Name, PassWord = user.PassWord });
            }
            else
            {
                throw new UserFriendlyException("Authorize.GetTokenBySms.'New password is not allowed to be empty");
            }

        }



        /// <summary>
        /// 发送短信验证手机号码所有权
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 可为空</param>
        /// <returns></returns>
        [HttpPost("SendSmsVerifyPhone")]
        public bool SendSmsVerifyPhone(dtoKeyValue keyValue)
        {

            string phone = keyValue.Key.ToString();

            string key = "VerifyPhone_" + phone;

            if (String.IsNullOrEmpty(_CacheService.GetString(key)))
            {

                Random ran = new();
                string code = ran.Next(100000, 999999).ToString();

                var jsonCode = new
                {
                    code = code
                };

                Common.AliYun.SmsHelper sms = new();
                var smsStatus = sms.SendSms(phone, "短信模板编号", "短信签名", Common.Json.JsonHelper.ObjectToJSON(jsonCode));

                if (smsStatus)
                {
                    _CacheService.SetString(key, code, new TimeSpan(0, 0, 5, 0));

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// 通过微信APP Code获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为weixinkeyid, value 为 code</param>
        /// <returns></returns>
        [HttpPost("GetTokenByWeiXinAppCode")]
        public async Task<string> GetTokenByWeiXinAppCode(dtoKeyValue keyValue)
        {



            var weixinkeyid = Guid.Parse(keyValue.Key.ToString());
            string code = keyValue.Value.ToString();


            var wxInfo = db.TWeiXinKey.Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Web.Libraries.WeiXin.App.WeiXinHelper(wxInfo.WxAppId, wxInfo.WxAppSecret);

            var accseetoken = weiXinHelper.GetAccessToken(code).accessToken;

            var openid = weiXinHelper.GetAccessToken(code).openId;

            var userInfo = weiXinHelper.GetUserInfo(accseetoken, openid);

            var user = db.TUserBindWeixin.Where(t => t.IsDelete == false && t.WeiXinKeyId == weixinkeyid && t.WeiXinOpenId == userInfo.openid).Select(t => t.User).FirstOrDefault();

            if (user == null)
            {
                user = new TUser();
                user.Id = Guid.NewGuid();
                user.IsDelete = false;
                user.CreateTime = DateTime.Now;

                user.Name = userInfo.nickname;
                user.NickName = user.Name;
                user.PassWord = Guid.NewGuid().ToString();

                db.TUser.Add(user);
                db.SaveChanges();

                var bind = new TUserBindWeixin();
                bind.Id = Guid.NewGuid();
                bind.IsDelete = false;
                bind.CreateTime = DateTime.Now;

                bind.WeiXinKeyId = weixinkeyid;
                bind.UserId = user.Id;
                bind.WeiXinOpenId = openid;

                db.TUserBindWeixin.Add(bind);

                db.SaveChanges();
            }

            return await GetToken(new dtoLogin { Name = user.Name, PassWord = user.PassWord });

        }

    }
}
