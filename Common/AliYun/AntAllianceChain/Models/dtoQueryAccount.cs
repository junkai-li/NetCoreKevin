namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 查询账户-入参
    /// </summary>
    public class dtoQueryAccount
    {

        /// <summary>
        /// 链 ID
        /// </summary>
        public string bizid { get; set; }


        /// <summary>
        /// QUERYACCOUNT(查询账户)
        /// </summary>
        public string method { get { return "QUERYACCOUNT"; } }


        /// <summary>
        /// 内容为 {"queryAccount":"xxxx"}
        /// </summary>
        public string requestStr { get; set; }


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
