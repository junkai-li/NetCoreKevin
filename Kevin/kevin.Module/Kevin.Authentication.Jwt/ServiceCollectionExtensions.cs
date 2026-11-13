using Kevin.Authentication.Jwt.Dto;
using Kevin.Authentication.Jwt.IService;
using Kevin.Authentication.Jwt.Service;
using Kevin.Common.Extension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Kevin.Authentication.Jwt
{
    public static class ServiceCollectionExtensions
    {
        public static void AddKevinAuthenticationJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // 读取 JWT 配置
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);
            // 添加认证服务
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // 消除默认5分钟偏移，使过期时间严格
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var tokenService = context.HttpContext.RequestServices.GetService<ITokenService>();
                        // 黑名单校验：退出登录的 token 立即失效
                        var rawToken = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                        if (tokenService != null && !string.IsNullOrEmpty(rawToken))
                        {
                            try
                            {
                                if (tokenService.IsTokenBlacklisted(rawToken))
                                {
                                    context.Fail("已退出登录，请重新登录");
                                    return Task.CompletedTask;
                                }
                            }
                            catch
                            {
                                // Redis 异常时放行，避免故障放大为全站不可用
                            }
                        }
                        // 令牌版本号校验：改密/封禁后版本号自增，旧 token 立即失效
                        var userId = context.Principal?.FindFirst(JwtKeinClaimTypes.UserId)?.Value;
                        var tokenVersion = context.Principal?.FindFirst(JwtKeinClaimTypes.TokenVersion)?.Value;
                        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(tokenVersion))
                        {
                            try
                            {
                                if (tokenService != null && tokenVersion.ToTryInt64() != tokenService.GetUserTokenVersion(userId))
                                {
                                    context.Fail("登录状态已失效，请重新登录");
                                }
                            }
                            catch
                            {
                                // 缓存读取异常时放行，避免 Redis 故障放大为全站不可用（刷新机制同样依赖 Redis）
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddSingleton<ITokenService, TokenService>();
        }
    }
}
