using kevin.Application;
using kevin.CodeGenerator;
using kevin.CodeGenerator.Dto;
using kevin.Domain.Interfaces.IServices;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.WebApi.Controllers
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiVersionNeutral]
    public class CodeGeneratorController : ControllerBase
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
        public async Task<List<string>> GetAreaNames()
        {
            return await _codeGeneratorService.GetAreaNames();
        }

        /// <summary>
        /// 获取区域下的实体列表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetAreaNameEntityItems")]
        public async Task<List<EntityItemDto>> GetAreaNameEntityItems([FromQuery][Required] string name)
        {
            return await _codeGeneratorService.GetAreaNameEntityItems(name);
        }
    }
}
