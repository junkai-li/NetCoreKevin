using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kvin.Integration.Tests
{
    public static class AppGetToken
    {
        public static void GetToken(ref HttpClient _client,string Name = "admin", string PassWord = "admin", int TenantId = 1000)
        {
            var data = new
            {
                Name = Name,
                PassWord = PassWord,
                TenantId = TenantId,
            }.ToJson().ToString();
            using Stream dataStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            using HttpContent content = new StreamContent(dataStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentType.CharSet = "utf-8";
            var response = _client.PostAsync("/api/Authorize/GetToken", content).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reslut = response.Content.ReadAsStringAsync().Result;
            Assert.True(!reslut.ToLower().Contains("errMsg"));
            //登录
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + JsonHelper.GetValueByKey(reslut, "data"));
        }
    }
}
