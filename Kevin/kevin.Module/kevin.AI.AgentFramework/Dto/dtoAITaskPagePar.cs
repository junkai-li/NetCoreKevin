using System;
using System.Collections.Generic;
using System.Text;

namespace kevin.AI.AgentFramework.Dto
{
    /// <summary>
    /// AI 自动任务管理端查询入参（独立于 dtoPagePar&lt;T&gt;，避免泛型 Parameter 非空必填的模型绑定问题）
    /// </summary>
    public class dtoAITaskPagePar
    {
        /// <summary>
        /// 任务名称模糊搜索（可空）
        /// </summary>
        public string searchKey { get; set; } = "";

        /// <summary>
        /// 状态筛选：running 运行中 / paused 已暂停 / 空=全部
        /// </summary>
        public string state { get; set; } = "";

        /// <summary>
        /// 分页大小
        /// </summary>
        public int pageSize { get; set; } = 20;

        /// <summary>
        /// 分页页数
        /// </summary>
        public int pageNum { get; set; } = 1;
    }

    /// <summary>
    /// AI 自动任务列表项（管理端结构化返回，替代旧的字符串拼接解析）
    /// </summary>
    public class dtoAITaskItem
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// 任务类型：cron 周期性 / once 一次性
        /// </summary>
        public string taskType { get; set; } = "cron";

        /// <summary>
        /// cron 表达式（周期性任务）
        /// </summary>
        public string cron { get; set; } = "";

        /// <summary>
        /// 任务内容
        /// </summary>
        public string content { get; set; } = "";

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool isPaused { get; set; }

        /// <summary>
        /// 下次执行时间（周期性=Hangfire 计算值；一次性=执行时间点；暂停任务为空）
        /// </summary>
        public string next { get; set; } = "";

        /// <summary>
        /// 上次执行时间
        /// </summary>
        public string last { get; set; } = "";

        /// <summary>
        /// 时区
        /// </summary>
        public string timeZone { get; set; } = "";
    }

    /// <summary>
    /// AI 自动任务分页结果
    /// </summary>
    public class dtoAITaskPage
    {
        /// <summary>
        /// 数据总量
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<dtoAITaskItem> list { get; set; } = new List<dtoAITaskItem>();
    }
}
