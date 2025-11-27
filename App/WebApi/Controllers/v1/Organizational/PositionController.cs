using kevin.Application.Services;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Interfaces.IServices.Organizational;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Domain.Share.Dtos.System;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.WebApi.Controllers.v1.Organizational
{
    /// <summary>
    /// 岗位管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("组织架构", "Organizational")]
    [MyModule("岗位管理", "Position")]
    public class PositionController : ControllerBase
    {
        private  IPositionService _positionService { get; set; }

        public PositionController(IPositionService positionService)
        {
            this._positionService = positionService;
        }

        [HttpPost("GetPageData")]
        [ActionDescription("获取岗位列表")]
        [HttpLog("岗位管理", "获取岗位列表")]
        public async Task<dtoPageData<PositionDto>> GetPageData([FromBody] dtoPagePar<string> par)
        { 
            var result = await _positionService.GetPageData(par);
            return result;
        }

        [HttpGet("GetPositionTree")]
        [ActionDescription("获取岗位树形结构")]
        [HttpLog("岗位管理", "获取岗位树形结构")]
        public async Task<PositionDto> GetPositionTree()
        {
            var result = await _positionService.GetPositionTree();
            return result;
        }

        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑岗位")]
        [HttpLog("岗位管理", "新增或编辑岗位")]
        public async Task<bool> AddEdit([FromBody] PositionDto dtoRole)
        {
            var result = await _positionService.AddEdit(dtoRole);
            return result;
        }

        [ActionDescription("删除岗位")]
        [HttpLog("岗位管理", "删除岗位")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _positionService.Delete(Id);
            return result;
        }

    }
}
