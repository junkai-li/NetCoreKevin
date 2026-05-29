using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Kevin.SignalR.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kevin.Web.Basics.Controllers
{
    /// <summary>
    /// SignalR相关API服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [SkipAuthority]
    [MyArea("SignalR管理", "SignalR")]
    [MyModule("SignalR消息管理", "SignalRMsg")]
    public class SignalRController : ApiControllerBase
    {
        private ISignalRMsgService _service { get; set; }
        public SignalRController(ISignalRMsgService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 发送公告消息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("SendPublicMsg")]
        [ActionDescription("发送公告消息")]
        [SkipAuthority]
        public async Task<bool> SendPublicMsg([Required] string method, [Required] string msg)
        {
            await _service.SendPublicMsg(method, msg);
            return true;
        }
        /// <summary>
        /// 私发信息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("SendConnIdMsg")]
        [ActionDescription("私发信息")]
        [SkipAuthority]
        public async Task<bool> SendConnIdMsg([Required] string method, [Required] string msg, [Required] string connId)
        {
            await _service.SendConnIdMsg(method, connId, msg);
            return true;
        }
        /// <summary>
        /// 获取租户所有身份id
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetTenantIdentityIds")]
        [ActionDescription("获取租户所有身份id")]
        [SkipAuthority]
        public List<string> GetTenantIdentityIds()
        {
            return _service.GetTenantIdentityIds(CurrentUser.TenantId);

        }

        /// <summary>
        /// 私发信息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("SendIdentityIdMsg")]
        [ActionDescription("私发信息")]
        [SkipAuthority]
        public async Task<bool> SendIdentityIdMsg([Required] string method, [Required] string msg, [Required] string identityId)
        {
            await _service.SendIdentityIdMsg(method, identityId, msg);
            return true;
        }

        /// <summary>
        /// 批量私发信息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("SendIdentityIdsMsg")]
        [ActionDescription("批量私发信息")]
        [SkipAuthority]
        public async Task<bool> SendIdentityIdsMsg([Required] string method, [Required] string msg, [Required] List<string> identityIds)
        {
            await _service.SendIdentityIdsMsg(method, identityIds, msg);
            return true;
        }



    }
}
