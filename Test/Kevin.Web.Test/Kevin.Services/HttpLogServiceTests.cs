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
            string type, string remark, CancellationToken cancellationToken)
        { 
            // Act
            var result = await httpLogService.Add(type, remark);

            // Assert
            Assert.False(result); 
    }
}
}
