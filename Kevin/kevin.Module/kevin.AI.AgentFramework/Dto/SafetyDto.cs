using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Dto
{
    public class SafetyDto
    {
        /// <summary>
        /// 是否安全
        /// </summary>
        public bool IsSafe { get; set; } = true;

        /// <summary>
        /// 安全提示
        /// </summary>
        public string? SafetyMessage { get; set; }

        /// <summary>
        /// 相关详情
        /// </summary>

        public List<SafetyDetails>? SafetyDetails { get; set; } = new List<SafetyDetails>();

        /// <summary>
        /// 报告
        /// </summary>
        public string StageReports { get; set; } = "";
    }
    public class SafetyDetails
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; } = "";
        /// <summary>
        /// 安全类型等级 Critical High Medium
        /// </summary>
        public string Level { get; set; } = "";
        /// <summary>
        /// 建议
        /// </summary>
        public string? Suggestion { get; set; }
        /// <summary>
        /// 漏洞详情
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// 风险类型
        /// </summary>
        public string? RiskType { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// 代码开始行数
        /// </summary>
        public int? LineStart { get; set; }
        /// <summary>
        /// 代码结束行数
        /// </summary>
        public int? LineEnd { get; set; }
    }
}
