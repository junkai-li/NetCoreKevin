using kevin.Application.Services;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Msg;
using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Dtos.User;
using kevin.Domain.Share.Enums;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using kevin.Share.Dtos.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Web.Filters;

namespace App.WebApi.Controllers.v1
{
    /// <summary>
    /// 消息管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("系统管理", "System")]
    [MyModule("消息管理", "Message")]
    public class MessageController : ApiControllerBase
    {
        private IMessageService _messageService { get; set; }

        public MessageController(IMessageService messageService)
        {
            this._messageService = messageService;
        }

        [HttpPost("GetAnnouncementPageData")]
        [ActionDescription("获取公司公告数据")]
        [HttpLog("消息管理", "获取公司公告数据")]
        public async Task<dtoPageData<MessageDto>> GetAnnouncementPageData([FromBody] dtoPagePar<MsgGetPageDataParDto> dtoPage)
        {
            dtoPage.Parameter = dtoPage.Parameter == default ? new MsgGetPageDataParDto() : dtoPage.Parameter;
            dtoPage.Parameter.SysMsgType = MessageType.Announcement;
            var result = await _messageService.GetPageData(dtoPage);
            return result;
        }

        [HttpPost("GetPrivateUserPageData")]
        [ActionDescription("获取我的私人私信数据")]
        [HttpLog("消息管理", "获取我的私人私信数据")]
        public async Task<dtoPageData<MessageDto>> GetPrivateUserPageData([FromBody] dtoPagePar<MsgGetPageDataParDto> dtoPage)
        {
            dtoPage.Parameter = dtoPage.Parameter == default ? new MsgGetPageDataParDto() : dtoPage.Parameter;
            dtoPage.Parameter.SysMsgType = MessageType.PrivateUser;
            dtoPage.Parameter.UserId = CurrentUser.UserId.ToString();
            var result = await _messageService.GetPageData(dtoPage);
            return result;
        }

        [HttpPost("GetPrivateUserSystemPageData")]
        [ActionDescription("获取我的系统私信消息")]
        [HttpLog("消息管理", "获取我的系统私信消息")]
        public async Task<dtoPageData<MessageDto>> GetPrivateUserSystemPageData([FromBody] dtoPagePar<MsgGetPageDataParDto> dtoPage)
        {
            dtoPage.Parameter = dtoPage.Parameter == default ? new MsgGetPageDataParDto() : dtoPage.Parameter;
            dtoPage.Parameter.SysMsgType = MessageType.PrivateUserSystem;
            dtoPage.Parameter.UserId = CurrentUser.UserId.ToString();
            var result = await _messageService.GetPageData(dtoPage);
            return result;
        }
        [HttpPost("GetSystemPageData")]
        [ActionDescription("获取系统消息")]
        [HttpLog("消息管理", "获取系统消息")]
        public async Task<dtoPageData<MessageDto>> GetSystemPageData([FromBody] dtoPagePar<MsgGetPageDataParDto> dtoPage)
        {
            dtoPage.Parameter = dtoPage.Parameter == default ? new MsgGetPageDataParDto() : dtoPage.Parameter;
            dtoPage.Parameter.SysMsgType = MessageType.System;
            var result = await _messageService.GetPageData(dtoPage);
            return result;
        }

        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑消息")]
        [HttpLog("消息管理", "新增或编辑消息")]
        public async Task<bool> AddEdit([FromBody] MessageDto dtoRole)
        {
            var result = await _messageService.AddEdit(dtoRole);
            return result;
        }

        [ActionDescription("删除消息")]
        [HttpLog("角色管理", "删除消息")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _messageService.Delete(Id);
            return result;
        }
        /// <summary>
        /// 获取我的未读消息数量 十分钟延迟
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        [HttpGet("GetMyNoReadCount")]
        [SkipAuthority]
        [CacheDataFilter<int>(TTL = 3600, UseToken = true)]
        public async Task<int> GetMyNoReadCount([FromQuery] MessageType messageType = MessageType.All)
        {
            var result = await _messageService.GetMyNoReadCount(messageType);
            return result;
        }
    }
}
