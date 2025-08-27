namespace Kvin.Integration.Tests
{
    [TestCaseOrderer(
    ordererTypeName: "Testing.Shared.PriorityOrderer",
    ordererAssemblyName: "Kvin.Integration.Tests")]
    public class AppTests : IClassFixture<WebApplicationFactory<WebApi.Program>>
    {
        private readonly WebApplicationFactory<WebApi.Program> _factory;

        private readonly HttpClient _client;

        public AppTests(WebApplicationFactory<WebApi.Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:9901");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-Forwarded-For", "127.0.0.1");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-API-Version", "1.0"); 
            AppGetToken.GetToken(ref _client);
        }

        [Fact, TestPriority(-5)]
        public async Task GetDb()
        {
            Console.WriteLine("GetDb");
            var response = await _client.GetAsync("/api/Authorize/GetDb");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        } 

        [Theory, TestPriority(-2)]
        [InlineData("admin")]
        public async Task GetTokenUser(string Name)
        {
           
            //获取登录信息
            var response2 = await _client.GetAsync("/api/User/GetUser");
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            var reslut2 = await response2.Content.ReadAsStringAsync();
            Assert.True(!reslut2.ToLower().Contains("errMsg"));
            Assert.True(JsonHelper.GetValueByKey(JsonHelper.GetValueByKey(reslut2, "data"), "name") == Name);
        } 
    }
}