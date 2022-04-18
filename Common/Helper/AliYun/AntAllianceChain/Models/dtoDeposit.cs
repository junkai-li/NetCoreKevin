namespace Common.AliYun.AntAllianceChain.Models
{


    /// <summary>
    /// 存证方法入参
    /// </summary>
    public class dtoDeposit
    {

        /// <summary>
        /// 业务方请求唯一标识，用于重试去重
        /// </summary>
        public string orderId { get; set; }



        /// <summary>
        /// 链 ID
        /// </summary>
        public string bizid { get; set; }



        /// <summary>
        /// 链上账户名
        /// </summary>
        public string account { get; set; }



        /// <summary>
        /// 存证内容
        /// </summary>
        public string content { get; set; }



        /// <summary>
        /// 租户id，可通过管理控制台>账户信息查看
        /// </summary>
        public string tenantid { get; set; }



        /// <summary>
        /// 创建链上账户时使用的 mykmsKeyId
        /// </summary>
        public string mykmsKeyId { get; set; }



        /// <summary>
        /// Method：DEPOSIT
        /// </summary>
        public string method { get { return "DEPOSIT"; } }



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
