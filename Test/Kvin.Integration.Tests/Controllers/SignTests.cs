using AutoFixture.Xunit2;

namespace Kvin.Integration.Tests.Controllers
{
    public class SignTests : IClassFixture<WebApplicationFactory<WebApi.Program>>
    {
        private readonly WebApplicationFactory<WebApi.Program> _factory;

        private readonly HttpClient _client;
        public SignTests(WebApplicationFactory<WebApi.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:9901");
        }

        [Theory, TestPriority(-4)]
        [InlineData("admin", "admin", 1000)]
        public async Task GetToken(string Name = "admin", string PassWord = "admin", int TenantId = 1000)
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
            var response = await _client.PostAsync("/api/Authorize/GetToken", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reslut = await response.Content.ReadAsStringAsync();
            Assert.True(!reslut.ToLower().Contains("errMsg"));
            //登录
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + JsonHelper.GetValueByKey(reslut, "data"));
        }

        public static string _Table;
        public static Guid _TableId;
        public static string _Sign;

        [Theory, TestPriority(-3)]
        [AutoData]
        public async Task AddSign(string Table, Guid TableId, string Sign)
        {
            _Table = Table;
            _TableId = TableId;
            _Sign = Sign;
            await GetToken(); 
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

        [Fact, TestPriority(3)] 
        public async Task GetSignCount()
        {
            await GetToken();
            //获取登录信息
            var response2 = await _client.GetAsync($"/api/User/GetSignCount?table={_Table}&tableId={_TableId}&sign={_Sign}");
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            var reslut2 = await response2.Content.ReadAsStringAsync();
            Assert.True(!reslut2.ToLower().Contains("errMsg"));
            Assert.True(JsonHelper.GetValueByKey(reslut2, "data").ToTryInt32() > 0);
        }
    }
}
