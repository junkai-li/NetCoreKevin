using kevin.Application;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Kevin.SignalR.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace App.WebApi.Controllers
{
    /// <summary>
    /// SignalR相关API服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SignalRController : ApiControllerBase
    {
        [IocProperty]
        public ISignalRMsgService _service { get; set; }

        /// <summary>
        /// 发送公告消息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("SendPublicMsg")]
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
        public List<string> GetTenantIdentityIds()
        {
            return _service.GetTenantIdentityIds(CurrentUser.TenantId);

        }

        /// <summary>
        /// 私发信息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("SendIdentityIdMsg")]
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
        public async Task<bool> SendIdentityIdsMsg([Required] string method, [Required] string msg, [Required] List<string> identityIds)
        {
            await _service.SendIdentityIdsMsg(method, identityIds, msg);
            return true;
        }



    }
}
