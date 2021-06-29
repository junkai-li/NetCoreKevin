namespace Common.AliYun.AntAllianceChain.Models
{


    /// <summary>
    /// 交易查询入参
    /// </summary>
    public class dtoQueryTransAction
    {

        /// <summary>
        /// 链 ID
        /// </summary>
        public string bizid { get; set; }


        /// <summary>
        /// 执行方式，QUERYTRANSACTION(查询交易),QUERYRECEIPT(查询交易回执)
        /// </summary>
        public string method { get; set; }


        /// <summary>
        /// 交易 hash，16 进制字符串
        /// </summary>
        public string hash { get; set; }


        /// <summary>
        /// 链上账户对应的 accessId
        /// </summary>
        public string accessId { get; set; }


        /// <summary>
        /// 服务端返回的 token，有效期为30分钟。
        /// </summary>
        public string token { get; set; }
    }
}
