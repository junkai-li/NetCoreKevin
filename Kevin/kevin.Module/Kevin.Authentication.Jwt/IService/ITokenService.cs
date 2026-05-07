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
    }
}
