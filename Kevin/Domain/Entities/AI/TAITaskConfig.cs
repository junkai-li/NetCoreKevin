using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// TAITaskConfig
    /// </summary>
    [Table("TAITaskConfig")]
    [Description("AI 自动任务配置（任务权威注册表：Hangfire 只负责调度，暂停状态与恢复所需元数据落库）")]
    [Index(nameof(UserId))]
    [Index(nameof(UserId), nameof(Name))]
    public class TAITaskConfig : CUD
    {
        /// <summary>
        /// 任务归属用户Id
        /// </summary>
        [Description("任务归属用户Id")]
        public long UserId { get; set; }

        /// <summary>
        /// 任务名称（同用户下唯一；Hangfire jobId = userId + name）
        /// </summary>
        [Description("任务名称，同用户下唯一")]
        [MaxLength(200)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 任务类型：cron 周期性 / once 一次性
        /// </summary>
        [Description("任务类型：cron 周期性 / once 一次性")]
        [MaxLength(20)]
        public string TaskType { get; set; } = "cron";

        /// <summary>
        /// cron 表达式（周期性任务专用）
        /// </summary>
        [Description("cron 表达式（周期性任务专用）")]
        [MaxLength(100)]
        public string CronExpression { get; set; } = "";

        /// <summary>
        /// 一次性任务执行时间点
        /// </summary>
        [Description("一次性任务执行时间点")]
        public DateTime? ExecuteTime { get; set; }

        /// <summary>
        /// 任务内容（恢复/重新注册时回放用）
        /// </summary>
        [Description("任务内容")]
        public string TaskContent { get; set; } = "";

        /// <summary>
        /// 创建任务时的 AI 上下文快照（AIShareInfoDto JSON，恢复时回放）
        /// </summary>
        [Description("创建任务时的 AI 上下文快照 JSON")]
        public string TaskData { get; set; } = "";

        /// <summary>
        /// 是否暂停：暂停后 Hangfire 侧作业被移除，不再执行；恢复时按落库元数据重新注册
        /// </summary>
        [Description("是否暂停")]
        public bool IsPaused { get; set; }

        /// <summary>
        /// 最近一次实际执行时间
        /// </summary>
        [Description("最近一次实际执行时间")]
        public DateTime? LastRunTime { get; set; }
    }
}
