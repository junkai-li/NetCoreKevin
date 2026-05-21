using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Filters;

namespace Kevin.Web.Basics.Controllers.AI
{
    /// <summary>
    /// AI技能工具管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI技能工具管理", "AISkillToolManagement")]
    public class AISkillToolManagementController : ControllerBase
    {
        private IAISkillToolManagementService _service { get; set; }

        public AISkillToolManagementController(IAISkillToolManagementService service)
        {
            this._service = service;
        }
        /// <summary>
        /// 获取AI技能工具列表
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("GetPageData")]
        [ActionDescription("获取AI技能工具列表")]
        [HttpLog("AI技能工具管理", "获取AI技能工具列表")]
        public async Task<dtoPageData<AISkillToolManagementDto>> GetPageData([FromBody] dtoPagePar<int> par)
        {
            var result = await _service.GetPageData(par);
            return result;
        } 
        /// <summary>
        /// 新增或编辑AI技能工具
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑AI技能工具")]
        [HttpLog("AI技能工具管理", "新增或编辑AI技能工具")]
        public async Task<bool> AddEdit([FromBody] AISkillToolManagementDto par)
        {
            var result = await _service.AddEdit(par);
            return result;
        }

        /// <summary>
        /// 删除AI技能工具
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ActionDescription("删除AI技能工具")]
        [HttpLog("AI技能工具管理", "删除AI技能工具")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _service.Delete(Id);
            return result;
        }
    }
}
