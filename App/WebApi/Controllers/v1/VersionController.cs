using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Bases;

namespace App.WebApi.Controllers.v1
{
    /// <summary>
    /// 版本控制器
    /// </summary> 
    [ApiVersion("1.0")]
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
            return "我是版本1";
        }
    }
}
