using kevin.Cache.Service;
using kevin.Domain.Share.Attributes;
using Kevin.Authentication.Jwt.IService;
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
        public AuthorizeService(IHttpContextAccessor _httpContextAccessor, IUserService IUserService, ICacheService ICacheService, IConfiguration IConfiguration, IDistributedLockProvider IDistributedLockProvider, ISMS ISMS, ITokenService tokenService, KevinDbContext dbContext) : base(_httpContextAccessor)
        {
            this._IUserService = IUserService;
            this._CacheService = ICacheService;
            this.Configuration = IConfiguration;
            this.distLock = IDistributedLockProvider;
            this._ISMS = ISMS;
            this._TokenService = tokenService;
            this._db = dbContext;
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
            var user = _IUserService.LoginUser(login.Name, login.PassWord, login.TenantId, login.PasswordHash ?? "");
            return GenerateTokenForUser(user);
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
        /// 利用手机号和短信验证码获取Token认证信息
        /// </summary>
        /// <param name="keyValue">key 为手机号，value 为验证码</param>
        /// <returns></returns>
        [HttpPost("GetTokenBySms")]
        [HttpLog("登录", "GetTokenBySms利用手机号和短信验证码获取Token认证信息")]
        public async Task<string> GetTokenBySms(dtoKeyValue keyValue)
        {
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
                    user.ChangePassword(phone);
                    _db.Set<TUser>().Add(user);
                    _db.SaveChanges();
                }
                // 直接通过passwordHash登录，避免对已哈希密码再次哈希
                var loggedInUser = _IUserService.LoginUser(user.Name ?? "", "", user.TenantId, user.PasswordHash ?? "");
                return GenerateTokenForUser(loggedInUser);
            }
            else
            {
                throw new UserFriendlyException("Authorize.GetTokenBySms.'短信验证码校验失败");
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
