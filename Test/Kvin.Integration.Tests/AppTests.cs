namespace Kvin.Integration.Tests
{
    public class AppTests : IClassFixture<WebApplicationFactory<WebApi.Program>>
    {
        private readonly WebApplicationFactory<WebApi.Program> _factory;

        private readonly HttpClient _client;

        public AppTests(WebApplicationFactory<WebApi.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:9901");
        }

        [Fact, TestPriority(-5)]
        public async Task GetDb()
        {
            Console.WriteLine("GetDb");
            var response = await _client.GetAsync("/api/Authorize/GetDb");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory, TestPriority(-4)]
        [InlineData("admin", "admin", 1000)]
        public async Task GetToken(string Name= "admin", string PassWord= "admin", int TenantId= 1000)
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

        [Theory, TestPriority(-2)]
        [InlineData("admin")]
        public async Task GetTokenUser(string Name)
        {
          
            await GetToken();
            //获取登录信息
            var response2 = await _client.GetAsync("/api/User/GetUser");
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            var reslut2 = await response2.Content.ReadAsStringAsync();
            Assert.True(!reslut2.ToLower().Contains("errMsg"));
            Assert.True(JsonHelper.GetValueByKey(JsonHelper.GetValueByKey(reslut2, "data"), "name") == Name);
        } 
    }
}