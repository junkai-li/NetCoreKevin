using kevin.Application;
using kevin.CodeGenerator;
using kevin.CodeGenerator.Dto;
using kevin.Domain.Interfaces.IServices;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace App.WebApi.Controllers
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [MyArea("系统管理", "System")]
    [MyModule("代码生成器", "CodeGenerator")]
    [Authorize] 
    public class CodeGeneratorController : ApiControllerBase
    {
        private ICodeGeneratorService _codeGeneratorService;
        public CodeGeneratorController(ICodeGeneratorService codeGeneratorService)
        {
            this._codeGeneratorService = codeGeneratorService;
        }

        /// <summary>
        /// 获取区域列表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetAreaNames")]
        [ActionDescription("获取区域列表")]
        public async Task<List<string>> GetAreaNames()
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            }
            return await _codeGeneratorService.GetAreaNames();
        }

        /// <summary>
        /// 获取区域下的实体列表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetAreaNameEntityItems")]
        [ActionDescription("获取区域下的实体列表")]
        public async Task<List<EntityItemDto>> GetAreaNameEntityItems([FromQuery][Required] string name)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            }
            return await _codeGeneratorService.GetAreaNameEntityItems(name);
        }

        /// <summary>
        /// 生成仓储和服务基础代码
        /// </summary> 
        /// <returns></returns>
        [HttpPost("BulidCode")]
        [ActionDescription("生成仓储和服务基础代码")]
        public async Task<bool> BulidCode([FromBody] List<EntityItemDto> data)
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("非超级管理员无权限操作");
            }
            return await _codeGeneratorService.BulidCode(data);
        }
    }
}
