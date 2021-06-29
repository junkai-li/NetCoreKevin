namespace Common.AliYun.AntAllianceChain.Models
{


    /// <summary>
    /// 获取 Token 入参
    /// </summary>
    public class dtoGetToken
    {

        /// <summary>
        /// 链上账户对应的 accessId
        /// </summary>
        public string accessId { get; set; }



        /// <summary>
        /// 时间戳
        /// </summary>
        public string time { get; set; }



        /// <summary>
        /// sign(AccessId+now)签名的过程（Hex.encode(RSAWithSHA256(message))）
        /// </summary>
        public string secret { get; set; }

    }
}
