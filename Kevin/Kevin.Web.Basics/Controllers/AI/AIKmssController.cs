using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.Filters;

namespace Kevin.Web.Basics.AI
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
        [ActionDescription("获取AI知识库分页列表")]
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
        [CacheDataFilter<dtoPageList<AIKmssDto>>(TTL = 60, UseToken = true)]
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
        /// 获取知识库文件碎片分页列表（只读实时查询向量库）
        /// </summary>
        /// <param name="kmsId">知识库Id</param>
        /// <param name="sourceFile">来源文件名（导入成功后回写的 ContentName）</param>
        /// <param name="pageNum">页码，从 1 开始，默认 1</param>
        /// <param name="pageSize">每页条数，默认 10，上限 100</param>
        /// <returns>total 为碎片总数；list 为当前页碎片 [{ id, content, chunkIndex, category, createdAt }]；
        /// 集合不存在/Qdrant 异常/无碎片时 total=0 且 list 为空</returns>
        [HttpGet("GetKmsChunks")]
        [ActionDescription("获取知识库文件碎片列表")]
        [SkipAuthority]
        public async Task<KmsChunksDto> GetKmsChunks(
            [FromQuery][Required] long kmsId,
            [FromQuery][Required] string sourceFile,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetKmsChunks(kmsId, sourceFile, pageNum, pageSize);
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
