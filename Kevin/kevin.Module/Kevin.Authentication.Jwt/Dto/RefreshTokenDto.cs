namespace Kevin.Authentication.Jwt.Dto
{
    public class RefreshTokenDto
    {

        /// <summary>
        /// 刷新Token过期时间
        /// </summary>
        public DateTime RefreshExpireTime { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserDto User { get; set; }

        /// <summary>
        /// 签发时的令牌版本号：换发时与当前版本比对，不一致（改密/封禁后）则拒绝换发
        /// </summary>
        public long TokenVersion { get; set; }
    }
}
