using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Kevin.Web.Filters.TransactionScope.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.WebApi.Controllers.v1.AI
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
        public async Task<dtoPageData<AIChatHistorysDto>> GetPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetPageData(par);
            return result;
        }

        /// <summary>
        /// 新增AI对话聊天记录
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>

        [HttpPost("Add")]
        [ActionDescription("新增AI对话聊天记录")]
        [HttpLog("AI对话聊天记录管理", "新增AI对话聊天记录")]
        [Transactional]
        public async Task<AIChatHistorysDto> Add([FromBody] AIChatHistorysDto par)
        {
            var result = await _service.Add(par);
            return result;
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
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
