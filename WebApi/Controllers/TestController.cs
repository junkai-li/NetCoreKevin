using Kevin.Web.Attributes.IocAttrributes.IocAttrributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.v1._;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ControllerBase
    {
        [IocProperty]
        public IUserService _userService { get; set; }

        [HttpGet]
       public object TestUserService() {
            _userService.GetUser(new Guid());
            return default;
        }
    }
}
