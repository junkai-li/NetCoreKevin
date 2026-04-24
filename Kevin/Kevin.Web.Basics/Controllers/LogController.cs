using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos.System;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kevin.Web.Basics.Controllers
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("系统管理", "System")]
    [MyModule("操作日志", "Log")] 
    public class LogController : ControllerBase
    {
        private IHttpLogService _httpLogService { get; set; }

        private IOSLogService _osLogService { get; set; }

        public LogController(IHttpLogService httpLogService, IOSLogService osLogService)
        {
            this._httpLogService = httpLogService;
            _osLogService = osLogService;
        }

        [HttpPost("GetHttpLogPageData")]
        [ActionDescription("获取请求操作日志")]
        [HttpLog("操作日志", "获取请求操作日志")]
        public async Task<dtoPageData<HttpLogDto>> GetHttpLogPageData([FromBody] dtoPageData<HttpLogDto> dtoPage)
        {
            var result = await _httpLogService.GetPageData(dtoPage);
            return result;
        }

        [HttpPost("GetOSLogPageData")]
        [ActionDescription("获取关键数据变动日志")]
        [HttpLog("操作日志", "获取关键数据变动日志")]
        public async Task<dtoPageData<OSLogDto>> GetOSLogPageData([FromBody] dtoPageData<OSLogDto> dtoPage)
        {
            var result = await _osLogService.GetPageData(dtoPage);
            return result;
        }
    }
}
