namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 调用 Solidity 合约入参
    /// </summary>
    public class dtoCallSolidity
    {

        /// <summary>
        /// 业务方请求唯一标识，用于重试去重。
        /// </summary>
        public string orderId { get; set; }



        /// <summary>
        /// 链上账户名
        /// </summary>
        public string account { get; set; }



        /// <summary>
        /// 合约名
        /// </summary>
        public string contractName { get; set; }


        /// <summary>
        /// 方法签名,例如：sayName(string)
        /// </summary>
        public string methodSignature { get; set; }



        /// <summary>
        /// 实参列表，例如：[\"zhangx11iaodong\"]
        /// </summary>
        public string inputParamListStr { get; set; }



        /// <summary>
        /// 返回参数列表，例如：[\"string\"]
        /// </summary>
        public string outTypes { get; set; }



        /// <summary>
        /// 链上账户对应的 mykmsKeyId
        /// </summary>
        public string mykmsKeyId { get; set; }



        /// <summary>
        /// Method(固定：CALLCONTRACTBIZASYNC)
        /// </summary>
        public string method { get { return "CALLCONTRACTBIZASYNC"; } }



        /// <summary>
        /// 租户id，可通过管理控制台>账户信息查看
        /// </summary>
        public string tenantid { get; set; }



        /// <summary>
        /// 链 ID
        /// </summary>
        public string bizid { get; set; }



        /// <summary>
        /// 链上账户对应的 accessId
        /// </summary>
        public string accessId { get; set; }



        /// <summary>
        /// 服务端返回的 token，有效期为30分钟。
        /// </summary>
        public string token { get; set; }


        /// <summary>
        /// 设置本次交易合理的gaslimit，防止异常交易过度消耗gas，且需要大于本次交易可能消耗的gas，否则交易将执行失败，消耗的gas不返还
        /// </summary>
        public int gas { get; set; }
    }

}
