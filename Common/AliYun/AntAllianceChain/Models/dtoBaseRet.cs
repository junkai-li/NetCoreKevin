namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 基础返回值
    /// </summary>
    public class dtoBaseRet
    {


        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool success { get; set; }



        /// <summary>
        /// 状态码
        /// </summary>
        public string code { get; set; }



        /// <summary>
        /// 交易 hash，后续可以使用 hash 来查询交易以及回执
        /// </summary>
        public string data { get; set; }
    }


}
