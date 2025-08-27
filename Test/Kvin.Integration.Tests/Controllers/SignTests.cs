using AutoFixture.Xunit2;

namespace Kvin.Integration.Tests.Controllers
{
    [TestCaseOrderer(
    ordererTypeName: "Testing.Shared.PriorityOrderer",
    ordererAssemblyName: "Kvin.Integration.Tests.Controllers")]
    public class SignTests : IClassFixture<WebApplicationFactory<WebApi.Program>>
    {
        private readonly WebApplicationFactory<WebApi.Program> _factory;

        private readonly HttpClient _client;
        public SignTests(WebApplicationFactory<WebApi.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:9901");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-Forwarded-For", "127.0.0.1");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-API-Version", "1.0");
            AppGetToken.GetToken(ref _client);
        } 
 

        [Theory, TestPriority(-3)]
        [AutoData]
        public async Task AddSign(string Table, Guid TableId, string Sign)
        {  
            var data = new
            {
                Table = Table,
                TableId = TableId,
                Sign = Sign,
            }.ToJson().ToString();
            using Stream dataStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            using HttpContent content = new StreamContent(dataStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentType.CharSet = "utf-8";
            var response = await _client.PostAsync("/api/Sign/AddSign", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reslut = await response.Content.ReadAsStringAsync();
            Assert.True(!reslut.ToLower().Contains("errMsg"));
        }

        [Theory, TestPriority(3)]
        [AutoData]
        public async Task GetSignCount(string Table, Guid TableId, string Sign)
        { 
           await AddSign(Table, TableId, Sign);
            //获取新增count
            var response2 = await _client.GetAsync($"/api/Sign/GetSignCount?table={Table}&tableId={TableId}&sign={Sign}");
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            var reslut2 = await response2.Content.ReadAsStringAsync();
            Assert.True(!reslut2.ToLower().Contains("errMsg"));
            Assert.True(JsonHelper.GetValueByKey(reslut2, "data").ToTryInt32() > 0); 
        }

        [Theory, TestPriority(4)]
        [AutoData]
        public async Task DeleteSign(string Table, Guid TableId, string Sign)
        {  
            var response2 = await _client.DeleteAsync($"/api/Sign/DeleteSign?table={Table}&tableId={TableId}&sign={Sign}");
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            var reslut2 = await response2.Content.ReadAsStringAsync();
            Assert.True(!reslut2.ToLower().Contains("errMsg"));
            Assert.True(JsonHelper.GetValueByKey(reslut2, "data").ToBoolean()); 
        }
    }
}
