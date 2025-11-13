using IdentityModel.Client;
using kevin.Cache.Service;
using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using kevin.Permission.Interfaces;
using kevin.Share.Dtos;
using Kevin.Common.Helper;
using Medallion.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository.Database;
using Web.Global.Exceptions;


namespace kevin.Application.Services
{
    public class AuthorizeService : BaseService, IAuthorizeService
    { 
        public IUserService _IUserService { get; set; } 
        public ICacheService _CacheService { get; set; } 
        public IConfiguration Configuration { get; set; } 
        public IDistributedLockProvider distLock { get; set; }
        public AuthorizeService(IHttpContextAccessor _httpContextAccessor,IUserService IUserService, ICacheService ICacheService, IConfiguration IConfiguration, IDistributedLockProvider IDistributedLockProvider) : base(_httpContextAccessor)
        {
            this._IUserService = IUserService;
            this._CacheService = ICacheService;
            this.Configuration = IConfiguration;
            this.distLock = IDistributedLockProvider;
        }

        ///// <summary>
        ///// 获取Token认证信息
        ///// </summary>
        ///// <param name="login">登录信息集合</param>
        ///// <returns></returns> 
        public async Task<string> GetToken([FromBody] dtoLogin login)
        {
            using KevinDbContext db = new KevinDbContext();
            var TTenant = db.Set<TTenant>().FirstOrDefault(t => t.Code == login.TenantId);
            if (TTenant == null)
            {
                throw new UserFriendlyException("租户不存在");
            }
            else
            {
                if (TTenant.IsDelete)
                {
                    throw new UserFriendlyException("租户已删除");
                }
                if (TTenant.Status == kevin.Domain.Share.Enums.TenantStatusEnums.Inactive)
                {
                    throw new UserFriendlyException("租户已失效");
                }
            }
            var user = _IUserService.LoginUser(login.Name, login.PassWord, login.TenantId, login.PasswordHash);
            var clinet = new HttpClient();
            var disco = await clinet.GetDiscoveryDocumentAsync(Configuration["JwtOptions:Authority"]);
            if (disco.IsError)
            {
                throw new UserFriendlyException("登录异常");
            }
            //var par = new Parameters();
            //par.Add("TenantId", login.TenantId.ToString());
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
        /// 通过微信小程序Code获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为weixinkeyid, value 为 code</param>
        /// <returns></returns>
        [HttpPost("GetTokenByWeiXinMiniAppCode")]
        [HttpLog("登录", "GetTokenByWeiXinMiniAppCode通过微信小程序Code获取Token认证信息")]
        public async Task<string> GetTokenByWeiXinMiniAppCode([FromBody] dtoKeyValue keyValue)
        {
            using KevinDbContext db = new KevinDbContext();
            var weixinkeyid = Guid.Parse(keyValue.Key.ToString());
            string code = keyValue.Value.ToString();

            var weixinkey = db.Set<TWeiXinKey>().Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);


            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;
            string sessionkey = wxinfo.sessionkey;

            var user = db.Set<TUserBindWeixin>().Where(t => t.WeiXinOpenId == openid).Select(t => t.User).FirstOrDefault();

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
                            user = db.Set<TUserBindWeixin>().Where(t => t.WeiXinOpenId == openid).Select(t => t.User).FirstOrDefault();
                            if (user == null)
                            {
                                //注册一个只有基本信息的账户出来
                                user = new TUser();

                                user.Id = Guid.NewGuid();
                                user.IsDelete = false;
                                user.CreateTime = DateTime.Now;
                                user.Name = DateTime.Now.ToString() + "微信小程序新用户";
                                user.NickName = user.Name;
                                user.ChangePassword(Guid.NewGuid().ToString());

                                //开发时记得调整这个值
                                user.RoleId = default;

                                db.Set<TUser>().Add(user);

                                db.SaveChanges();

                                TUserBindWeixin userBind = new();
                                userBind.Id = Guid.NewGuid();
                                userBind.IsDelete = false;
                                userBind.CreateTime = DateTime.Now;
                                userBind.UserId = user.Id;
                                userBind.WeiXinKeyId = weixinkeyid;
                                userBind.WeiXinOpenId = openid;

                                db.Set<TUserBindWeixin>().Add(userBind);

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
        [HttpLog("登录", "GetTokenBySms利用手机号和短信验证码获取Token认证信息")]
        public async Task<string> GetTokenBySms(dtoKeyValue keyValue)
        {
            using KevinDbContext db = new KevinDbContext();
            if (Web.Auth.AuthorizeAction.SmsVerifyPhone(keyValue))
            {
                string phone = keyValue.Key.ToString();


                var user = db.Set<TUser>().Where(t => t.IsDelete == false && (t.Name == phone || t.Phone == phone) && t.RoleId == default).FirstOrDefault();

                if (user == null)
                {
                    //注册一个只有基本信息的账户出来

                    user = new TUser();

                    user.Id = Guid.NewGuid();
                    user.IsDelete = false;
                    user.CreateTime = DateTime.Now;
                    user.Name = DateTime.Now.ToString() + "手机短信新用户";
                    user.NickName = user.Name;
                    user.ChangePassword(phone);

                    db.Set<TUser>().Add(user);

                    db.SaveChanges();
                }

                return await GetToken(new dtoLogin { Name = user.Name, PassWord = user.PasswordHash });
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
            using KevinDbContext db = new KevinDbContext();
            var weixinkeyid = Guid.Parse(keyValue.Key.ToString());
            string code = keyValue.Value.ToString();
            var wxInfo = db.Set<TWeiXinKey>().Where(t => t.Id == weixinkeyid).FirstOrDefault();
            var weiXinHelper = new Web.Libraries.WeiXin.App.WeiXinHelper(wxInfo.WxAppId, wxInfo.WxAppSecret);
            var accseetoken = weiXinHelper.GetAccessToken(code).accessToken;
            var openid = weiXinHelper.GetAccessToken(code).openId;
            var userInfo = weiXinHelper.GetUserInfo(accseetoken, openid);
            var user = db.Set<TUserBindWeixin>().Where(t => t.IsDelete == false && t.WeiXinKeyId == weixinkeyid && t.WeiXinOpenId == userInfo.openid).Select(t => t.User).FirstOrDefault();

            if (user == null)
            {
                user = new TUser();
                user.Id = Guid.NewGuid();
                user.IsDelete = false;
                user.CreateTime = DateTime.Now;
                user.Name = userInfo.nickname;
                user.NickName = user.Name;
                user.PasswordHash = new HashHelper().SHA256Hash(Guid.NewGuid().ToString());

                db.Set<TUser>().Add(user);
                db.SaveChanges();

                var bind = new TUserBindWeixin();
                bind.Id = Guid.NewGuid();
                bind.IsDelete = false;
                bind.CreateTime = DateTime.Now;

                bind.WeiXinKeyId = weixinkeyid;
                bind.UserId = user.Id;
                bind.WeiXinOpenId = openid;

                db.Set<TUserBindWeixin>().Add(bind);

                db.SaveChanges();
            }

            return await GetToken(new dtoLogin { Name = user.Name, PassWord = user.PasswordHash });

        }
    }
}
