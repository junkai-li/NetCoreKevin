using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Mvc;
using Web.Global.Exceptions;

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
            throw new UserFriendlyException("我是版本2"); 
        }
    }
}
