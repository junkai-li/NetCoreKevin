namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 查询最新块高-入参
    /// </summary>
    public class dtoQueryLastBlock
    {

        /// <summary>
        /// 链 ID
        /// </summary>
        public string bizid { get; set; }


        /// <summary>
        /// QUERYLASTBLOCK(查询最新块高)
        /// </summary>
        public string method { get; set; }


        /// <summary>
        /// 链上账户对应的 accessId
        /// </summary>
        public string accessId { get; set; }


        /// <summary>
        /// 服务端返回的 token，有效期为 2 小时。
        /// </summary>
        public string token { get; set; }

    }
}
