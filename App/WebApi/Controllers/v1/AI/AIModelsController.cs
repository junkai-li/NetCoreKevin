using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
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
    /// AI模型管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI模型管理", "AIModels")]
    public class AIModelsController : ControllerBase
    {
        private IAIModelsService _service { get; set; }

        public AIModelsController(IAIModelsService service)
        {
            this._service = service;
        }
        /// <summary>
        /// 获取AI模型
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetPageData")]
        [ActionDescription("获取AI模型")]
        [HttpLog("AI模型管理", "获取AI模型")]
        public async Task<dtoPageData<AIModelsDto>> GetPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetPageData(par);
            return result;
        }

        [HttpGet("GetALLList")]
        [ActionDescription("获取AI模型列表")]
        [HttpLog("AI模型管理", "获取AI模型列表")]
        [CacheDataFilter<List<AIModelsDto>>(TTL = 60, UseToken = false)]
        public async Task<List<AIModelsDto>> GetALLList([FromQuery] int Type = 1)
        {
            var result = await _service.GetALLList(Type);
            return result;
        }
        /// <summary>
        /// 新增或编辑AI模型
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>

        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑AI模型")]
        [HttpLog("AI模型管理", "新增或编辑AI模型")]
        public async Task<bool> AddEdit([FromBody] AIModelsDto par)
        {
            var result = await _service.AddEdit(par);
            return result;
        }
        /// <summary>
        /// 删除AI模型
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除AI模型")]
        [HttpLog("AI模型管理", "删除AI模型")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
