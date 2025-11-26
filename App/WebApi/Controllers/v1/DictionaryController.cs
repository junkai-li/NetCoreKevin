using kevin.Application.Services;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.System;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.WebApi.Controllers.v1
{
    /// <summary>
    /// 字典管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("系统管理", "System")]
    [MyModule("字典管理", "Dictionary")]
    public class DictionaryController : ControllerBase
    {
        private IDictionaryService _dictionaryService { get; set; }

        public DictionaryController(IHttpLogService httpLogService, IDictionaryService dictionaryService)
        {
            this._dictionaryService = dictionaryService;
        }

        [HttpPost("GetPageData")]
        [ActionDescription("获取系统字典列表")]
        [HttpLog("字典管理", "获取系统字典列表")]
        public async Task<dtoPageData<DictionaryDto>> GetPageData([FromBody] dtoPagePar<string> dtoPage)
        {
            dtoPage.Parameter = dtoPage.Parameter == default ?  "":dtoPage.Parameter; 
            var result = await _dictionaryService.GetPageData(dtoPage);
            return result;
        }

        [HttpGet("GetTypeList")] 
        [HttpLog("字典管理", "获取系统字典类型列表")]
        [SkipAuthority]
        public async Task<List<string>> GetTypeList()
        { 
            var result = await _dictionaryService.GetTypeList();
            return result;
        }

        [HttpGet("GetTypeWhereList")]
        [HttpLog("字典管理", "获取系统字典指定类型列表")]
        [SkipAuthority]
        public async Task<List<DictionaryDto>> GetTypeWhereList([FromQuery][Required] string type)
        {
            var result = await _dictionaryService.GetTypeWhereList(type);
            return result;
        }

        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑")]
        [HttpLog("字典管理", "新增或编辑")]
        public async Task<bool> AddEdit([FromBody] DictionaryDto data)
        {
            var result = await _dictionaryService.AddEdit(data);
            return result;
        }

        [ActionDescription("删除")]
        [HttpLog("字典管理", "删除")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] Guid Id)
        {
            var result = await _dictionaryService.Delete(Id);
            return result;
        }
    }
}
