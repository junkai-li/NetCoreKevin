using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.Domain.Share.Attributes;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            // 与服务层成功文案对齐（服务层为"移除任务成功"，失败为"移除定时任务失败"）
            return result.Replace(name, "").Contains("移除任务成功");
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
            // 与服务层成功文案对齐（服务层为"执行任务成功"，失败为"执行任务失败/执行定时任务失败"）
            return result.Replace(name, "").Contains("执行任务成功");
        }

        /// <summary>
        /// 分页查询我的AI任务（结构化返回：类型/暂停状态/下次与上次执行时间；权威源 TAITaskConfig，state 支持 running/paused/空）
        /// </summary>
        [HttpPost("GetMyTaskPage")]
        [ActionDescription("分页查询我的AI任务")]
        [HttpLog("AI自动任务管理", "分页查询我的AI任务")]
        [SkipAuthority]
        public async Task<dtoPageList<dtoAITaskItem>> GetMyTaskPage([FromBody] dtoAITaskPagePar par)
        {
            var result = await _service.GetMyTaskPage(par ?? new dtoAITaskPagePar());
            return new dtoPageList<dtoAITaskItem> { List = result.list, Total = result.total };
        }

        /// <summary>
        /// 暂停我的AI任务（暂停后不再执行，任务列表保留，可随时恢复）
        /// </summary>
        [HttpPost("PauseTask")]
        [ActionDescription("暂停我的AI任务")]
        [HttpLog("AI自动任务管理", "暂停我的AI任务")]
        [SkipAuthority]
        public async Task<bool> PauseTask([FromQuery] string name)
        {
            var result = await _service.PauseTask(name);
            return result.Replace(name, "").Contains("暂停任务成功");
        }

        /// <summary>
        /// 恢复我的AI任务（按落库配置重新注册到调度器）
        /// </summary>
        [HttpPost("ResumeTask")]
        [ActionDescription("恢复我的AI任务")]
        [HttpLog("AI自动任务管理", "恢复我的AI任务")]
        [SkipAuthority]
        public async Task<bool> ResumeTask([FromQuery] string name)
        {
            var result = await _service.ResumeTask(name);
            return result.Replace(name, "").Contains("恢复任务成功");
        }
    }
}
