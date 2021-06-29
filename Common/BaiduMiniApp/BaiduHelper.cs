namespace Common.BaiduMiniApp
{
    public class BaiduHelper
    {

        string AppKey = "";

        string AppSecret = "";


        public BaiduHelper(string in_AppKey,string in_AppSecret)
        {
            AppKey = in_AppKey;
            AppSecret = in_AppSecret;
        }



        /// <summary>
        /// 获取用户OpenId 和 SessionKey
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public (string openid, string sessionkey) GetOpenIdAndSessionKey(string code)
        {
            string url = "https://spapi.baidu.com/oauth/jscode2sessionkey?code="+code+"&client_id="+AppKey+"&sk="+AppSecret+"";

            string httpret = HttpHelper.Get(url);

            string openid = Json.JsonHelper.GetValueByKey(httpret, "openid");

            string sessionkey = Json.JsonHelper.GetValueByKey(httpret, "session_key");

            return (openid, sessionkey);
        }

    }
}
