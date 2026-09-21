using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Kevin.Web.Filters.TransactionScope.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kevin.Web.Basics.AI
{
    /// <summary>
    /// AI对话记录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI对话聊天记录管理", "AIChatHistorys")]
    public class AIChatHistorysController : ControllerBase
    {
        private IAIChatHistorysService _service { get; set; }

        public AIChatHistorysController(IAIChatHistorysService service)
        {
            this._service = service;
        }
        /// <summary>
        /// 获取AI对话聊天记录
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetPageData")]
        [ActionDescription("获取AI对话聊天记录")]
        [HttpLog("AI对话聊天记录管理", "获取AI对话聊天记录")]
        [SkipAuthority]
        public async Task<dtoPageData<AIChatHistorysDto>> GetPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetPageData(par);
            return result;
        }

        /// <summary>
        /// 新增AI对话聊天记录
        /// </summary>
        /// <param name="par"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpPost("Add")]
        [ActionDescription("新增AI对话聊天记录")]
        [HttpLog("AI对话聊天记录管理", "新增AI对话聊天记录")] 
        [SkipAuthority]
        public async Task<AIChatHistorysDto> Add([FromBody] AIChatHistorysDto par, CancellationToken cancellationToken)
        {
            var result = await _service.Add(par, cancellationToken: cancellationToken);
            return result;
        }
        // <summary>
        /// 新增AI对话聊天记录（SSE 流式输出）
        /// <para>与 <c>Add</c> 共用同一处理逻辑，仅把流式分片改为以 text/event-stream 写回响应；
        /// 响应事件名与 SignalR 通道一致：processmsg / aimsg / aIToolsContentMsg / aIReasoningContentMsg，
        /// 结束时下发 done（data 为最终回复记录 JSON），业务异常以 error 事件回传。</para>
        /// </summary>
        /// <param name="par"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("AddSSE")]
        [ActionDescription("新增AI对话聊天记录(SSE流式)")]
        [HttpLog("AI对话聊天记录管理", "新增AI对话聊天记录(SSE流式)")]
        [SkipAuthority]
        [global::Web.Filters.SkipResultFilter]
        public async Task AddSSE([FromBody] AIChatHistorysDto par, CancellationToken cancellationToken)
        {
            await _service.AddSSE(par, cancellationToken);
        }
        /// <summary>
        /// 删除AI对话聊天记录
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除AI对话聊天记录")]
        [HttpLog("AI对话聊天记录管理", "删除AI对话聊天记录")]
        [HttpDelete("Delete")]
        [Transactional]
        [SkipAuthority]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
