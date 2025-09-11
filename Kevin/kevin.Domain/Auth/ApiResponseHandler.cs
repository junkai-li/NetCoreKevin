using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Encodings.Web;

/// <summary>
/// 重新401
/// </summary>
namespace Web.Auth
{
    public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(JsonConvert.SerializeObject(new { code = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized, msg = "errmsg", errMsg = "很抱歉，您无权访问该接口，请确保已经登录!" }));
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;
            await Response.WriteAsync(JsonConvert.SerializeObject(new { code = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden, msg = "errmsg", errMsg = "很抱歉，您的访问权限等级不够，联系管理员!" }));
        }
    }
}
