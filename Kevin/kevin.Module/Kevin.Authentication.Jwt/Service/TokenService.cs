using kevin.Cache.Service;
using Kevin.Authentication.Jwt.Dto;
using Kevin.Authentication.Jwt.IService;
using Kevin.Common.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kevin.Authentication.Jwt.Service
{
    internal class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cacheService;
        private readonly IConfigurationSection jwtSettings;

        /// <summary>令牌版本号所在 Redis 连接（懒加载，单例服务内只初始化一次）</summary>
        private IDatabase? _versionDb;

        /// <summary>用户令牌版本号缓存前缀（改密/封禁时自增，旧 token 比对不通过即失效）</summary>
        private const string TokenVersionKeyPrefix = "Kevin_UserTokenVersion_";

        /// <summary>用户刷新凭证索引缓存前缀（值为该用户全部刷新凭证 key 的 JSON 列表）</summary>
        private const string TokenIndexKeyPrefix = "Kevin_UserTokensIndex_";

        /// <summary>已退出登录的 access token 黑名单前缀（值为 "1"，TTL 与该 token 剩余有效期一致）</summary>
        private const string TokenBlacklistKeyPrefix = "Kevin_TokenBlacklist_";

        public TokenService(IConfiguration config, ICacheService cacheService)
        {
            _config = config;
            _cacheService = cacheService;
            jwtSettings = _config.GetSection("Jwt");
        }
        /// <summary>
        /// 生成访问token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateAccessToken(UserDto user)
        {
            var tokenVersion = GetUserTokenVersion(user.Id);
            var tokens = WriteToken(user, tokenVersion);
            //缓存刷新token
            if (_cacheService != default)
            {
                var expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));
                _cacheService.SetString(tokens, new RefreshTokenDto
                {
                    RefreshExpireTime = expires,
                    User = user,
                    TokenVersion = tokenVersion
                }.ToJson(), timeOut: TimeSpan.FromDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!)));
                AddTokenToUserIndex(user.Id, tokens);
            }
            return tokens;
        }
        /// <summary>
        /// 根据Token生成新的Token
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public string RefreshTokenAccessToken(string tokenStr)
        {
            if (_cacheService != default && !string.IsNullOrEmpty(tokenStr))
            {
                var reData = _cacheService.GetString(tokenStr);
                if (!string.IsNullOrEmpty(reData))
                {
                    var refreshTokenInfo = reData.ToObject<RefreshTokenDto>();
                    var user = refreshTokenInfo.User;
                    // 改密/封禁后版本号已递增，旧凭证不允许再换发
                    if (refreshTokenInfo.TokenVersion != GetUserTokenVersion(user.Id))
                    {
                        _cacheService.Remove(tokenStr);
                        return "";
                    }
                    _cacheService.Remove(tokenStr);
                    var tokenVersion = GetUserTokenVersion(user.Id);
                    var tokens = WriteToken(user, tokenVersion);
                    //缓存刷新token 
                    var expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));
                    _cacheService.SetString(tokens, new RefreshTokenDto
                    {
                        RefreshExpireTime = expires,
                        User = user,
                        TokenVersion = tokenVersion
                    }.ToJson(), timeOut: TimeSpan.FromDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!)));
                    // 索引维护：旧凭证已作废，移除后登记新凭证
                    RemoveTokenFromUserIndex(user.Id, tokenStr);
                    AddTokenToUserIndex(user.Id, tokens);
                    return tokens;
                }
            }
            return "";
        }
        /// <summary>
        /// 校验token是否为本系统颁发（用同一把密钥验证签名，并校验颁发者、受众、有效期）
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public TokenValidationResultDto ValidateAccessToken(string tokenStr)
        {
            var result = new TokenValidationResultDto();
            if (string.IsNullOrEmpty(tokenStr))
            {
                result.Message = "token不能为空";
                return result;
            }
            try
            {
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero // 消除默认5分钟偏移，使过期时间严格
                };
                var handler = new JwtSecurityTokenHandler();
                // 关闭内置 Claim 映射，保证 userid/name 等自定义 claim 名称不被转换
                handler.MapInboundClaims = false;
                var principal = handler.ValidateToken(tokenStr, validationParameters, out _);
                result.IsValid = true;
                result.User = new UserDto
                {
                    Id = principal.FindFirst(JwtKeinClaimTypes.UserId)?.Value ?? "",
                    Name = principal.FindFirst(JwtKeinClaimTypes.Name)?.Value ?? "",
                    IsSuperAdmin = bool.TryParse(principal.FindFirst(JwtKeinClaimTypes.issuperadmin)?.Value, out var isSuperAdmin) && isSuperAdmin,
                    TenantId = int.TryParse(principal.FindFirst(JwtKeinClaimTypes.TenantId)?.Value, out var tenantId) ? tenantId : 0,
                    CreatedTime = DateTime.TryParse(principal.FindFirst(JwtKeinClaimTypes.CreatedTime)?.Value, out var createdTime) ? createdTime : null
                };
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 退出登录：将当前 token 加入黑名单使其立即失效，并删除刷新凭证禁止续发
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public bool Logout(string tokenStr)
        {
            if (string.IsNullOrEmpty(tokenStr))
            {
                return false;
            }
            try
            {
                // 拉黑当前 token：TTL 与其剩余有效期一致，过期自动清理
                BlacklistToken(tokenStr);
                if (_cacheService == default)
                {
                    return true;
                }
                // 从刷新凭证中取出用户信息，用于维护索引
                var reData = _cacheService.GetString(tokenStr);
                if (!string.IsNullOrEmpty(reData))
                {
                    var userId = reData.ToObject<RefreshTokenDto>()?.User?.Id;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        RemoveTokenFromUserIndex(userId, tokenStr);
                    }
                }
                return _cacheService.Remove(tokenStr);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将 token 加入黑名单（TTL 与其剩余有效期一致，到期自动清理）
        /// </summary>
        /// <param name="tokenStr"></param>
        public void BlacklistToken(string tokenStr)
        {
            try
            {
                var db = GetVersionDatabase();
                if (db == null || string.IsNullOrEmpty(tokenStr))
                {
                    return;
                }
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenStr);
                var ttl = jwt.ValidTo - DateTime.UtcNow;
                if (ttl <= TimeSpan.Zero)
                {
                    return;
                }
                db.StringSet(TokenBlacklistKeyPrefix + tokenStr, "1", ttl);
            }
            catch
            {
                // 解析失败（非法 token）时不拉黑，也不影响退出主流程
            }
        }

        /// <summary>
        /// 判断 token 是否已被拉黑（退出登录后）
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public bool IsTokenBlacklisted(string tokenStr)
        {
            try
            {
                var db = GetVersionDatabase();
                if (db == null || string.IsNullOrEmpty(tokenStr))
                {
                    return false;
                }
                return db.KeyExists(TokenBlacklistKeyPrefix + tokenStr);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 吊销用户所有登录凭证（改密/封禁/删除用户时调用）：版本号自增使旧 access token 立即失效，
        /// 并删除全部刷新凭证使旧 token 无法再续发
        /// </summary>
        /// <param name="userId"></param>
        public void RevokeUserTokens(string userId)
        {
            if (_cacheService == default || string.IsNullOrEmpty(userId))
            {
                return;
            }
            try
            {
                IncrementUserTokenVersion(userId);
                var tokens = _cacheService.GetObjectOrDefault<List<string>>(TokenIndexKeyPrefix + userId);
                if (tokens != null)
                {
                    foreach (var token in tokens)
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            _cacheService.Remove(token);
                        }
                    }
                }
                _cacheService.Remove(TokenIndexKeyPrefix + userId);
            }
            catch
            {
                Console.WriteLine("吊销用户令牌失败，userId：" + userId);
            }
        }

        /// <summary>
        /// 获取用户当前令牌版本号（不存在时为 0），供鉴权时与 token 内版本比对。
        /// 版本号存 Redis 而非内存缓存：多实例共享，且容器重启后不丢失，旧 token 不会失效后“复活”
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public long GetUserTokenVersion(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }
            try
            {
                var db = GetVersionDatabase();
                if (db == null)
                {
                    return 0;
                }
                var value = db.StringGet(TokenVersionKeyPrefix + userId);
                return value.TryParse(out long version) ? version : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 签发 JWT（含令牌版本号 claim）
        /// </summary>
        private string WriteToken(UserDto user, long tokenVersion)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
            new Claim(JwtKeinClaimTypes.UserId, user.Id),
            new Claim(JwtKeinClaimTypes.issuperadmin, user.IsSuperAdmin.ToString()),
            new Claim(JwtKeinClaimTypes.Name, user.Name??""),
            new Claim(JwtKeinClaimTypes.CreatedTime,user.CreatedTime!=null?user.CreatedTime.Value.ToString("yyyy-MM-dd"):""),
            new Claim(JwtKeinClaimTypes.TenantId, user.TenantId.ToString()),
            new Claim(JwtKeinClaimTypes.TokenVersion, tokenVersion.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 用户令牌版本号自增（Redis 原子自增，多实例并发安全）
        /// </summary>
        private long IncrementUserTokenVersion(string userId)
        {
            var db = GetVersionDatabase();
            if (db == null)
            {
                return 0;
            }
            return db.StringIncrement(TokenVersionKeyPrefix + userId);
        }

        /// <summary>
        /// 令牌版本号的 Redis 连接（懒加载；连接失败返回 null，由调用方降级放行）
        /// </summary>
        private IDatabase? GetVersionDatabase()
        {
            if (_versionDb != null)
            {
                return _versionDb;
            }
            try
            {
                var connectionString = _config.GetConnectionString("redisConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return null;
                }
                _versionDb = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
                return _versionDb;
            }
            catch
            {
                Console.WriteLine("令牌版本号 Redis 连接失败，版本校验降级为放行");
                return null;
            }
        }

        /// <summary>
        /// 把凭证登记到用户索引（顺手清理已失效的凭证记录，避免索引无限增长）
        /// </summary>
        private void AddTokenToUserIndex(string userId, string token)
        {
            try
            {
                var key = TokenIndexKeyPrefix + userId;
                var list = _cacheService.GetObjectOrDefault<List<string>>(key) ?? new List<string>();
                list.RemoveAll(t => string.IsNullOrEmpty(_cacheService.GetString(t)));
                if (!list.Contains(token))
                {
                    list.Add(token);
                }
                _cacheService.SetObject(key, list);
            }
            catch
            {
                // 索引仅用于吊销时反查凭证，维护失败不影响登录/换发主流程
            }
        }

        /// <summary>
        /// 从用户索引移除凭证
        /// </summary>
        private void RemoveTokenFromUserIndex(string userId, string token)
        {
            try
            {
                var key = TokenIndexKeyPrefix + userId;
                var list = _cacheService.GetObjectOrDefault<List<string>>(key);
                if (list == null)
                {
                    return;
                }
                list.Remove(token);
                _cacheService.SetObject(key, list);
            }
            catch
            {
                // 索引仅用于吊销时反查凭证，维护失败不影响登录/换发主流程
            }
        }
    }
}
