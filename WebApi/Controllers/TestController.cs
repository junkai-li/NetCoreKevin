using kevin.Cache.Service;
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

        [IocProperty]
        public ICacheService _cacheService { get; set; }

        [HttpGet("TestUserService")]
        public object TestUserService()
        {
            _userService.GetUser(new Guid());
            return default;
        }
        [HttpGet("TestCacheServiceOk")]
        public string TestCacheServiceOk()
        {
            _cacheService.SetString("1","ok");
            return _cacheService.GetString("1");
        }

        [HttpGet("TsetDynamicExpresso")]
        public bool TsetDynamicExpresso(string str)
        {
            var interpreter = new DynamicExpresso.Interpreter();
            return interpreter.Eval<bool>(str);
        }
        /// <summary>
        /// 健康检查接口
        /// </summary>
        /// <returns></returns>
        [HttpGet("HealthCheckGet")]
        public IActionResult HealthCheckGet()
        {
            return Ok();
        }
    }
}
