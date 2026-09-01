using kevin.Cache.Service;
using Kevin.Authentication.Jwt.Dto;
using Kevin.Authentication.Jwt.IService;
using Kevin.Common.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
            new Claim(JwtKeinClaimTypes.UserId, user.Id),
            new Claim(JwtKeinClaimTypes.issuperadmin, user.IsSuperAdmin.ToString()),
            new Claim(JwtKeinClaimTypes.Name, user.Name??""),
            new Claim(JwtKeinClaimTypes.CreatedTime,user.CreatedTime!=null?user.CreatedTime.Value.ToString("yyyy-MM-dd"):""),
            new Claim(JwtKeinClaimTypes.TenantId, user.TenantId.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                signingCredentials: credentials
            );
            var tokens = new JwtSecurityTokenHandler().WriteToken(token);
            //缓存刷新token
            if (_cacheService != default)
            {
                var expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));
                _cacheService.SetString(tokens, new RefreshTokenDto
                {
                    RefreshExpireTime = expires,
                    User = user
                }.ToJson(), timeOut: TimeSpan.FromDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!)));
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
                    _cacheService.Remove(tokenStr);
                    var refreshTokenInfo = reData.ToObject<RefreshTokenDto>();
                    var user = refreshTokenInfo.User;
                    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new Claim[]
                    {
                    new Claim(JwtKeinClaimTypes.UserId, user.Id),
                    new Claim(JwtKeinClaimTypes.issuperadmin, user.IsSuperAdmin.ToString()),
                    new Claim(JwtKeinClaimTypes.Name, user.Name??""),
                    new Claim(JwtKeinClaimTypes.CreatedTime,user.CreatedTime!=null?user.CreatedTime.Value.ToString("yyyy-MM-dd"):""),
                    new Claim(JwtKeinClaimTypes.TenantId, user.TenantId.ToString()),
                    };

                    var token = new JwtSecurityToken(
                        issuer: jwtSettings["Issuer"],
                        audience: jwtSettings["Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                        signingCredentials: credentials
                    );
                    var tokens = new JwtSecurityTokenHandler().WriteToken(token);
                    //缓存刷新token 
                    var expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!));
                    _cacheService.SetString(tokens, new RefreshTokenDto
                    {
                        RefreshExpireTime = expires,
                        User = user
                    }.ToJson(), timeOut: TimeSpan.FromDays(double.Parse(jwtSettings["RefreshTokenExpirationDays"]!)));
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
    }
}
