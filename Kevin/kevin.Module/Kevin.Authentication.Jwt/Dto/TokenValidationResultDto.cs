namespace Kevin.Authentication.Jwt.Dto
{
    /// <summary>
    /// Token 校验结果
    /// </summary>
    public class TokenValidationResultDto
    {
        /// <summary>
        /// 是否有效（签名匹配且未过期）
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 校验失败原因
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Token 中携带的用户信息
        /// </summary>
        public UserDto User { get; set; }
    }
}
