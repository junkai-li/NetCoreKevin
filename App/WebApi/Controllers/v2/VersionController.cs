using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace App.WebApi.Controllers.v2
{
    /// <summary>
    /// 版本控制器
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    [SkipAuthority]
    public class VersionController : ApiControllerBase
    {
        /// <summary>
        /// 我是版本
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetVersion")]
        public string GetVersion()
        {
            return "我是版本2";
        }
    }
}
