using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Web.Filters;

namespace App.WebApi.Controllers.v1.AI
{

    /// <summary>
    /// AI对话
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI对话管理", "AIChats")]
    public class AIChatsController : ControllerBase
    {
        private IAIChatsService _service { get; set; }

        public AIChatsController(IAIChatsService service)
        {
            this._service = service;
        }
        /// <summary>
        /// 获取我的AI对话列表
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetMyPageData")]
        [ActionDescription("获取我的AI对话列表")] 
        [HttpLog("AI对话管理", "获取我的AI对话列表")]
        public async Task<dtoPageData<AIChatsDto>> GetMyPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetMyPageData(par);
            return result;
        }

        /// <summary>
        /// 新增AI对话
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>

        [HttpPost("Add")]
        [ActionDescription("新增AI对话")]
        [HttpLog("AI对话管理", "新增AI对话")]
        public async Task<bool> Add([FromBody] AIChatsDto par)
        {
            var result = await _service.Add(par);
            return result;
        }
        /// <summary>
        /// 删除AI对话
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除AI对话")]
        [HttpLog("AI对话管理", "删除AI对话")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
