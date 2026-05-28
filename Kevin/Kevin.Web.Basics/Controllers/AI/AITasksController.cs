using DocumentFormat.OpenXml.Wordprocessing;
using kevin.AI.AgentFramework.Interfaces.Tasks;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Web.Basics.Controllers.AI
{
    /// <summary>
    /// AI自动任务管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("AI自动任务管理", "AITasks")]
    public class AITasksController : ControllerBase
    {
        private readonly IKevinAITaskService _service;
        public AITasksController(IKevinAITaskService service)
        {
            this._service = service;
        }
        /// <summary>
        /// 获取我的AI任务列表
        /// </summary> 
        /// <returns></returns>
        [HttpPost("GetTaskList")]
        [ActionDescription("获取我的AI任务列表")]
        [HttpLog("AI自动任务管理", "获取我的AI任务列表")]
        [SkipAuthority]
        public async Task<dtoPageList<string>> GetTaskList()
        {
            var result = (await _service.GetTaskList());
            return new dtoPageList<string>() { List = result, Total = result.Count };
        }

        /// <summary>
        /// 删除我的AI任务
        /// </summary> 
        /// <returns></returns>
        [HttpDelete("RemoveCronTask")]
        [ActionDescription("删除我的AI任务")]
        [HttpLog("AI自动任务管理", "删除我的AI任务")]
        [SkipAuthority]
        public async Task<bool> RemoveCronTask([FromQuery] string name)
        {
            var result = (await _service.RemoveCronTask(name));
            return result.Replace(name, "").Contains("移除定时任务成功");
        }

        /// <summary>
        ///执行我的AI任务
        /// </summary> 
        /// <returns></returns>
        [HttpGet("TriggerCronTask")]
        [ActionDescription("执行我的AI任务")]
        [HttpLog("AI自动任务管理", "执行我的AI任务")]
        [SkipAuthority]
        public async Task<bool> TriggerCronTask([FromQuery] string name)
        {
            var result = (await _service.TriggerCronTask(name));
            return result.Replace(name, "").Contains("执行定时任务成功");
        }
    }
}
