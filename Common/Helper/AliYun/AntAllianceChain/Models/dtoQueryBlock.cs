namespace Common.AliYun.AntAllianceChain.Models
{


    /// <summary>
    /// 查询块请求参数
    /// </summary>
    public class dtoQueryBlock
    {

        /// <summary>
        /// 链 ID
        /// </summary>
        public string bizid { get; set; }


        /// <summary>
        /// QUERYBLOCK(查询块头),QUERYBLOCKBODY(查询块体)
        /// </summary>
        public string method { get; set; }


        /// <summary>
        /// 块高
        /// </summary>
        public int requestStr { get; set; }


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
