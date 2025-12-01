using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Dtos.Organizational;
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
    /// AI提示词管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI提示词管理", "AIPrompts")]
    public class AIPromptsController : ControllerBase
    {
        private IAIPromptsService _service { get; set; }

        public AIPromptsController(IAIPromptsService service)
        {
            this._service = service;
        }
        /// <summary>
        /// 获取提示词列表
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetPageData")]
        [ActionDescription("获取提示词列表")]
        [HttpLog("提示词管理", "获取提示词列表")]
        public async Task<dtoPageData<AIPromptsDto>> GetPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetPageData(par);
            return result;
        }

        [HttpGet("GetALLList")]
        [ActionDescription("获取提示词列表")]
        [HttpLog("提示词管理", "获取提示词列表")]
        [CacheDataFilter<List<AIPromptsDto>>(TTL = 60, UseToken = false)]
        public async Task<List<AIPromptsDto>> GetALLList()
        {
            var result = await _service.GetALLList();
            return result;
        }
        /// <summary>
        /// 新增或编辑提示词
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑提示词")]
        [HttpLog("提示词管理", "新增或编辑提示词")]
        public async Task<bool> AddEdit([FromBody] AIPromptsDto par)
        {
            var result = await _service.AddEdit(par);
            return result;
        }

        /// <summary>
        /// 删除提示词
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除提示词")]
        [HttpLog("提示词管理", "删除提示词")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
