namespace Kevin.SMS
{
    public interface ISMS
    {


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="signName">签名</param>
        /// <param name="phone">接收方手机号</param>
        /// <param name="templateCode">模板编号</param>
        /// <param name="templateParams">模板参数</param>
        /// <returns></returns>
        public bool SendSMS(string signName, string phone, string templateCode, string templateParams);

    }
}