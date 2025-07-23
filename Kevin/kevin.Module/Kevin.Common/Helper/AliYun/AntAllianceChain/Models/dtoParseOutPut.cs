namespace Common.AliYun.AntAllianceChain.Models
{


    /// <summary>
    /// 展示解析output入参
    /// </summary>
    public class dtoParseOutPut
    {


        /// <summary>
        /// 链 ID
        /// </summary>
        public string bizid { get; set; }


        /// <summary>
        /// 执行方式
        /// </summary>
        public string method { get { return "PARSEOUTPUT"; } }



        /// <summary>
        /// 租户id，可通过管理控制台>账户信息查看
        /// </summary>
        public string tenantid { get; set; }



        /// <summary>
        /// 业务方请求唯一标识，用于重试去重。
        /// </summary>
        public string orderId { get; set; }



        /// <summary>
        /// 合约类型 ：EVM/WASM
        /// </summary>
        public string vmTypeEnum { get; set; }



        /// <summary>
        /// receipt的output字段进行hex编码，合约执行结果的二进制表示
        /// </summary>
        public string content { get; set; }



        /// <summary>
        /// 解析格式，json表示，比如 [\”bool\”,\”int\”,\”int[]\”]
        /// </summary>
        public string abi { get; set; }



        /// <summary>
        /// 链上账户对应的 mykmsKeyId
        /// </summary>
        public string mykmsKeyId { get; set; }



        /// <summary>
        /// 服务端返回的 token，有效期为30分钟。
        /// </summary>
        public string token { get; set; }



        /// <summary>
        /// 链上账户对应的 accessId
        /// </summary>
        public string accessId { get; set; }


        /// <summary>
        /// 链上账户名
        /// </summary>
        public string account { get; set; }

    }
}
