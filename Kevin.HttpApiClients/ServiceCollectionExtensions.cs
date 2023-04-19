using Microsoft.Extensions.DependencyInjection;

namespace kevin.HttpApiClients
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinHttpApiClients(this IServiceCollection service)
        {
            #region 基本
            service.AddHttpClient("", options =>
            {
                options.DefaultRequestVersion = new Version("2.0");
                options.DefaultRequestHeaders.Add("Accept", "*/*");
                options.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
                options.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false
            });


            service.AddHttpClient("SkipSsl", options =>
            {
                options.DefaultRequestVersion = new Version("2.0");
                options.DefaultRequestHeaders.Add("Accept", "*/*");
                options.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
                options.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });
            #endregion
        }
    }
}
