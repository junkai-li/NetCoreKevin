using kevin.Domain.Entities.AI;
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
    /// 智能体记忆管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("智能体记忆管理", "AIAgentMemories")]
    public class AIAgentMemoriesController : ControllerBase
    {
        private IAIAgentMemoryService _service { get; set; }

        public AIAgentMemoriesController(IAIAgentMemoryService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取智能体记忆列表（分页，支持 AIAppsId / 重要程度区间 / 关键词筛选，固定按当前用户过滤）
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetPageData")]
        [ActionDescription("获取智能体记忆")]
        [HttpLog("智能体记忆管理", "获取智能体记忆")]
        public async Task<dtoPageData<TAIAgentMemory>> GetPageData([FromBody] dtoAIAgentMemoryQuery par)
        {
            var result = await _service.GetPageData(par);
            return result;
        }

        /// <summary>
        /// 新增或编辑智能体记忆（AIAgentMemoryDto 白名单入参：归属三维与命中统计由服务端强制维护，编辑时不可变更；新增/编辑均同步 Qdrant 向量）
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑智能体记忆")]
        [HttpLog("智能体记忆管理", "新增或编辑智能体记忆")]
        public async Task<bool> AddEdit([FromBody] AIAgentMemoryDto par)
        {
            var result = await _service.AddEdit(par);
            return result;
        }

        /// <summary>
        /// 删除智能体记忆（软删 + 同步删除 Qdrant 向量）
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除智能体记忆")]
        [HttpLog("智能体记忆管理", "删除智能体记忆")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }

        /// <summary>
        /// 按智能体分组统计当前用户的有效记忆数（左栏记忆数徽标）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCountsByAIApps")]
        [ActionDescription("获取智能体记忆计数")]
        [HttpLog("智能体记忆管理", "获取智能体记忆计数")]
        public async Task<List<dtoAIAgentMemoryCount>> GetCountsByAIApps()
        {
            var result = await _service.GetCountGroupByAIApps();
            return result;
        }
    }
}
