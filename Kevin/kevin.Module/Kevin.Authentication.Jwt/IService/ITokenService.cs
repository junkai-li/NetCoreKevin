using Kevin.Authentication.Jwt.Dto;

namespace Kevin.Authentication.Jwt.IService
{
    public interface ITokenService
    {
        /// <summary>
        /// 获取token令牌
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string GenerateAccessToken(UserDto user);

        /// <summary>
        /// 刷新token令牌
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        string RefreshTokenAccessToken(string tokenStr);

        /// <summary>
        /// 校验token是否为本系统颁发（验证签名、颁发者、受众、有效期）
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        TokenValidationResultDto ValidateAccessToken(string tokenStr);
    }
}
