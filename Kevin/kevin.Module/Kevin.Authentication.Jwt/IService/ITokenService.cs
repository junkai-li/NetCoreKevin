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

        /// <summary>
        /// 退出登录：将当前 token 加入黑名单使其立即失效，并删除刷新凭证禁止续发
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        bool Logout(string tokenStr);

        /// <summary>
        /// 将 token 加入黑名单（退出登录时调用），该 token 立即失效，TTL 与其剩余有效期一致
        /// </summary>
        /// <param name="tokenStr"></param>
        void BlacklistToken(string tokenStr);

        /// <summary>
        /// 判断 token 是否已被拉黑（退出登录后）
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        bool IsTokenBlacklisted(string tokenStr);

        /// <summary>
        /// 吊销用户所有登录凭证（改密/封禁/删除用户时调用），旧 token 立即失效
        /// </summary>
        /// <param name="userId"></param>
        void RevokeUserTokens(string userId);

        /// <summary>
        /// 获取用户当前令牌版本号（不存在时为 0），供鉴权时与 token 内版本比对
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        long GetUserTokenVersion(string userId);
    }
}
