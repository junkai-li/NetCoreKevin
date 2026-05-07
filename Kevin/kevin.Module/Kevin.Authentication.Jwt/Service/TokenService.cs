using Kevin.Authentication.Jwt.Dto;
using Kevin.Authentication.Jwt.IService;
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

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(UserDto user)
        {
            var jwtSettings = _config.GetSection("Jwt");
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
