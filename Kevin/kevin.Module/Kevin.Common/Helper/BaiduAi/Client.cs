namespace Common.BaiduAi
{
    public class Client
    {
        public static Baidu.Aip.ImageSearch.ImageSearch ImageSearch()
        {
            var API_KEY = "";

            var SECRET_KEY = " ";

            Baidu.Aip.ImageSearch.ImageSearch client = new(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间

            return client;
        }





        public static Baidu.Aip.ImageClassify.ImageClassify ImageClassify()
        {
            var API_KEY = "";

            var SECRET_KEY = " ";

            Baidu.Aip.ImageClassify.ImageClassify client = new(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间

            return client;
        }
    }
}
