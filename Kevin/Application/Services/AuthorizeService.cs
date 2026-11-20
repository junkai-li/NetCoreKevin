using kevin.Cache.Service;
using kevin.Domain.Share.Attributes;
using Kevin.Authentication.Jwt.IService;
using Kevin.Common.Helper;
using Kevin.SMS;
using Medallion.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository.Database;


namespace kevin.Application.Services
{
    public class AuthorizeService : BaseService, IAuthorizeService
    {
        public IUserService _IUserService { get; set; }
        public ICacheService _CacheService { get; set; }
        public IConfiguration Configuration { get; set; }
        public IDistributedLockProvider distLock { get; set; }
        private readonly KevinDbContext _db;

        public ITokenService _TokenService { get; set; }
        public ISMS _ISMS { get; set; }
        public IDictionaryService _DictionaryService { get; set; }
        public AuthorizeService(IHttpContextAccessor _httpContextAccessor, IUserService IUserService, ICacheService ICacheService, IConfiguration IConfiguration, IDistributedLockProvider IDistributedLockProvider, ISMS ISMS, ITokenService tokenService, KevinDbContext dbContext, IDictionaryService dictionaryService) : base(_httpContextAccessor)
        {
            this._IUserService = IUserService;
            this._CacheService = ICacheService;
            this.Configuration = IConfiguration;
            this.distLock = IDistributedLockProvider;
            this._ISMS = ISMS;
            this._TokenService = tokenService;
            this._db = dbContext;
            this._DictionaryService = dictionaryService;
        }

        ///// <summary>
        ///// 根据用户Id获取Token认证信息  （内部使用）
        ///// </summary>
        ///// <param name="login">登录信息集合</param>
        ///// <returns></returns> 
        public async Task<string> GetTokenById(long userId, long tenantId)
        {
            using KevinDbContext db = new KevinDbContext();
            var TTenant = db.Set<TTenant>().FirstOrDefault(t => t.Code == tenantId);
            if (TTenant == null)
            {
                throw new UserFriendlyException("租户不存在");
            }
            else
            {
                TTenant.IsInactiveCheck();
            }
            var user =  await _IUserService.GetSysUserWhereId(userId);
            var accessToken = _TokenService.GenerateAccessToken(new Kevin.Authentication.Jwt.Dto.UserDto
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                IsSuperAdmin = user.IsSuperAdmin,
                Password = user.PassWord,
                Phone = user.Phone,
                CreatedTime = user.CreateTime,
                TenantId = user.TenantId,
            });
            return accessToken ?? "获取AccessToken失败";
        }

        ///// <summary>
        ///// 获取Token认证信息
        ///// </summary>
        ///// <param name="login">登录信息集合</param>
        ///// <returns></returns> 
        public async Task<string> GetToken([FromBody] dtoLogin login)
        {
            ValidateTenant(login.TenantId);
            CheckIpLocked();
            try
            {
                var user = _IUserService.LoginUser(login.Name, login.PassWord, login.TenantId, login.PasswordHash ?? "");
                ClearIpFail();
                return GenerateTokenForUser(user);
            }
            catch (UserFriendlyException)
            {
                IncrementIpFail();
                throw;
            }
        }

        /// <summary>
        /// 根据用户信息生成Token
        /// </summary>
        private string GenerateTokenForUser(kevin.Domain.Share.Dtos.User.dtoUser user)
        {
            var accessToken = _TokenService.GenerateAccessToken(new Kevin.Authentication.Jwt.Dto.UserDto
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                IsSuperAdmin = user.IsSuperAdmin,
                Password = user.PassWord,
                Phone = user.Phone,
                CreatedTime = user.CreateTime,
                TenantId = user.TenantId,
            });
            return accessToken ?? "获取AccessToken失败";
        }

        /// <summary>
        /// 验证租户有效性
        /// </summary>
        private void ValidateTenant(Int32 tenantId)
        {
            var TTenant = _db.Set<TTenant>().FirstOrDefault(t => t.Code == tenantId);
            if (TTenant == null)
            {
                throw new UserFriendlyException("租户不存在");
            }
            TTenant.IsInactiveCheck();
        }

        /// <summary>
        /// 获取当前请求来源 IP
        /// </summary>
        private string GetUserIp()
        {
            return HttpContextAccessor.HttpContext?.GetUserIp() ?? "";
        }

        /// <summary>
        /// 登录前检查 IP 是否已被锁定
        /// </summary>
        private void CheckIpLocked()
        {
            var ip = GetUserIp();
            if (string.IsNullOrEmpty(ip))
            {
                return;
            }
            if (_CacheService.IsContainKey("login:lock:ip:" + ip))
            {
                throw new UserFriendlyException("该IP已被锁定，请稍后再试");
            }
        }

        /// <summary>
        /// 登录失败时累计 IP 失败次数，达到阈值锁定 IP（distLock 保证原子性，锁失败则降级跳过）
        /// </summary>
        private void IncrementIpFail()
        {
            var ip = GetUserIp();
            if (string.IsNullOrEmpty(ip))
            {
                return;
            }
            var lockHandle = distLock.TryAcquireLock("login:fail:ip:" + ip);
            if (lockHandle == null)
            {
                // 锁获取失败：降级跳过本次计数，不阻塞登录主流程
                return;
            }
            using (lockHandle)
            {
                int ipFailedCount = GetIpFailedCount();
                int ipLockMinutes = GetIpLockMinutes();
                var current = _CacheService.GetString("login:fail:ip:" + ip);
                int count = (int.TryParse(current, out var n) ? n : 0) + 1;
                if (count > ipFailedCount)
                {
                    _CacheService.SetString("login:lock:ip:" + ip, "1", new TimeSpan(0, ipLockMinutes, 0));
                    _CacheService.Remove("login:fail:ip:" + ip);
                }
                else
                {
                    _CacheService.SetString("login:fail:ip:" + ip, count.ToString(), new TimeSpan(0, ipLockMinutes, 0));
                }
            }
        }

        /// <summary>
        /// 登录成功后清除当前 IP 的失败计数
        /// </summary>
        private void ClearIpFail()
        {
            var ip = GetUserIp();
            if (string.IsNullOrEmpty(ip))
            {
                return;
            }
            _CacheService.Remove("login:fail:ip:" + ip);
        }

        /// <summary>
        /// 读取 IP 最大失败次数，配置缺失时回退默认 50
        /// </summary>
        private int GetIpFailedCount()
        {
            var value = _DictionaryService.GetValueByKey("LoginSecurity", "IpFailedCount");
            return int.TryParse(value, out var n) ? n : 50;
        }

        /// <summary>
        /// 读取 IP 锁定时长（分钟），配置缺失时回退默认 30
        /// </summary>
        private int GetIpLockMinutes()
        {
            var value = _DictionaryService.GetValueByKey("LoginSecurity", "IpLockMinutes");
            return int.TryParse(value, out var n) ? n : 30;
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
            // 登录前检查 IP 锁定（与 GetToken 编排一致）
            CheckIpLocked();

            if (Web.Auth.AuthorizeAction.SmsVerifyPhone(keyValue))
            {
                string phone = keyValue.Key.ToString() ?? "";
                var user = _db.Set<TUser>().Where(t => t.IsDelete == false && (t.Name == phone || t.Phone == phone) && t.IsSystem == false).FirstOrDefault();

                if (user == null)
                {
                    //注册一个只有基本信息的账户出来 
                    user = new TUser();
                    user.Id = SnowflakeIdService.GetNextId();
                    user.IsDelete = false;
                    user.CreateTime = DateTime.Now;
                    user.Name = DateTime.Now.ToString() + "手机短信新用户";
                    user.NickName = user.Name;
                    user.IsSystem = false;
                    // 短信注册以手机号作密码属历史行为（非目标，不纳入复杂度校验），直接写密码哈希绕过正则校验
                    user.PasswordHash = new HashHelper().SHA256Hash(phone);
                    _db.Set<TUser>().Add(user);
                    _db.SaveChanges();
                }

                try
                {
                    // 直接通过passwordHash登录，避免对已哈希密码再次哈希
                    var loggedInUser = _IUserService.LoginUser(user.Name ?? "", "", user.TenantId, user.PasswordHash ?? "");
                    ClearIpFail();
                    return GenerateTokenForUser(loggedInUser);
                }
                catch (UserFriendlyException)
                {
                    IncrementIpFail();
                    throw;
                }
            }
            else
            {
                throw new UserFriendlyException("Authorize.GetTokenBySms.'短信验证码校验失败");
            }

        }
        /// <summary>
        /// 退出登录：删除当前 token 的刷新凭证
        /// </summary>
        public bool Logout(string tokenStr)
        {
            return _TokenService.Logout(tokenStr);
        }

        /// <summary>
        /// 发送短信验证手机号码所有权
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 可为空</param>
        /// <returns></returns>
        [HttpPost("SendSmsVerifyPhone")]
        public bool SendSmsVerifyPhone(dtoKeyValue keyValue)
        {
            string phone = keyValue.Key.ToString() ?? "";
            string key = "VerifyPhone_" + phone;
            if (String.IsNullOrEmpty(_CacheService.GetString(key)))
            {
                Random ran = new();
                string code = ran.Next(100000, 999999).ToString();
                var jsonCode = new
                {
                    code = code
                };
                var smsStatus = _ISMS.SendSMS(phone, "短信模板编号", "短信签名", Common.Json.JsonHelper.ObjectToJSON(jsonCode));
                if (smsStatus)
                {
                    _CacheService.SetString(key, code, new TimeSpan(0, 0, 5, 0));
                    return true;
                }
            }
            return false;
        }
    }
}
