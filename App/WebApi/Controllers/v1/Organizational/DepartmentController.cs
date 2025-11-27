using kevin.Domain.Interfaces.IServices.Organizational;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
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
    /// 部门管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController] 
    [Authorize]
    [MyArea("组织架构", "Organizational")]
    [MyModule("部门管理", "Department")]
    public class DepartmentController : ControllerBase
    {
        private IDepartmentService _departmentService { get; set; }

        public DepartmentController(IDepartmentService departmentService)
        {
            this._departmentService = departmentService;
        }

        [HttpPost("GetPageData")]
        [ActionDescription("获取部门列表")]
        [HttpLog("部门管理", "获取部门列表")]
        public async Task<dtoPageData<DepartmentDto>> GetPageData([FromBody] dtoPagePar<string> par)
        {
            var result = await _departmentService.GetPageData(par);
            return result;
        }

        [HttpPost("AddEdit")]
        [ActionDescription("新增或编辑部门")]
        [HttpLog("部门管理", "新增或编辑部门")]
        public async Task<bool> AddEdit([FromBody] DepartmentDto dtoRole)
        {
            var result = await _departmentService.AddEdit(dtoRole);
            return result;
        }

        [ActionDescription("删除部门")]
        [HttpLog("部门管理", "删除部门")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromQuery][Required] long Id)
        {
            var result = await _departmentService.Delete(Id);
            return result;
        }
    }
}
