using kevin.Application.Services.AI;
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

namespace App.WebApi.Controllers.v1.AI
{
    /// <summary>
    /// AI知识库管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController] 
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI知识库管理", "AIKmss")]
    public class AIKmssController : ControllerBase
    {
        private IAIKmssService _service { get; set; }

        public AIKmssController(IAIKmssService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取AI知识库分页列表
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetMyPageData")]
        [ActionDescription("获取AI知识库列表")]
        [HttpLog("AI知识库管理", "获取AI知识库分页列表")]
        public async Task<dtoPageData<AIKmssDto>> GetPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetPageData(par);
            return result;
        }

        /// <summary>
        /// 获取AI知识库列表
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetList")]
        [ActionDescription("获取AI知识库列表")]
        [HttpLog("AI知识库管理", "获取AI知识库列表")]
        public async Task<dtoPageList<AIKmssDto>> GetList([FromBody] dtoPagePar<string> par)
        {
            var result = await _service.GetList(par);
            return result;
        }

        /// <summary>
        /// 获取AI知识库详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetDetails")]
        [ActionDescription("获取AI知识库详情")]
        [SkipAuthority]
        public async Task<AIKmssDto> GetDetails([FromQuery][Required] long id)
        {
            var result = await _service.GetDetails(id);
            return result;
        }

        /// <summary>
        /// 新增编辑AI知识库
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>

        [HttpPost("AddEdit")]
        [ActionDescription("新增编辑AI知识库")]
        [HttpLog("AI知识库管理", "新增编辑AI知识库")]
        public async Task<bool> AddEdit([FromBody] AIKmssDto par)
        {
            var result = await _service.AddEdit(par);
            return result;
        }
        /// <summary>
        /// 删除AI知识库
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除AI知识库")]
        [HttpLog("AI知识库管理", "删除AI知识库")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
