using AutoFixture.Xunit2;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices;
using Kevin.Testing.Shared.Attributes;
using Microsoft.AspNetCore.Http;
using Testing.Shared;

namespace Kevin.Unit.Kevin.Services
{
    public class HttpLogServiceTests
    {
        [Theory]
        [AutoFakeItEasy]
     
        public async Task Add_ShouldHandleInvalidParameters(
            [Frozen] IHttpContextAccessor mockHttpContextAccessor,
            [Frozen] IHttpLogRp mockHttpLogRp,
             [Frozen] IHttpLogService httpLogService, 
            string type, string remark)
        {
            if (mockHttpContextAccessor is null)
            {
                throw new ArgumentNullException(nameof(mockHttpContextAccessor));
            }

            if (mockHttpLogRp is null)
            {
                throw new ArgumentNullException(nameof(mockHttpLogRp));
            }

            if (httpLogService is null)
            {
                throw new ArgumentNullException(nameof(httpLogService));
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException($"“{nameof(type)}”不能为 null 或空。", nameof(type));
            }

            if (string.IsNullOrEmpty(remark))
            {
                throw new ArgumentException($"“{nameof(remark)}”不能为 null 或空。", nameof(remark));
            }
            // Act
            var result = await httpLogService.Add(type, remark);

            // Assert
            Assert.False(result); 
    }
}
}
