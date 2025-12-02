using kevin.Application.Services.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Interfaces.IServices.Organizational;
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
    /// AI应用管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI应用管理", "AIApps")] 
    public class AIAppsController : ControllerBase
    { 
        private IAIAppsService _service { get; set; }

        public AIAppsController(IAIAppsService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取AI应用列表
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetPageData")]
        [ActionDescription("获取AI应用")]
        [HttpLog("AI应用管理", "获取AI应用")]
        public async Task<dtoPageData<AIAppsDto>> GetPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetPageData(par);
            return result;
        }


        [HttpGet("GetALLList")]
        [ActionDescription("获取AI应用列表")]
        [HttpLog("AI应用管理", "获取AI应用列表")]
        [CacheDataFilter<List<AIAppsDto>>(TTL = 60, UseToken = false)]
        public async Task<List<AIAppsDto>> GetALLList()
        {
            var result = await _service.GetALLList();
            return result;
        }
        /// <summary>
        /// 新增或编辑AI应用
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑AI应用")]
        [HttpLog("AI应用管理", "新增或编辑AI应用")]
        public async Task<bool> AddEdit([FromBody] AIAppsDto par)
        {
            var result = await _service.AddEdit(par);
            return result;
        }
        /// <summary>
        /// 删除AI应用
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除AI应用")]
        [HttpLog("AI应用管理", "删除AI应用")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
