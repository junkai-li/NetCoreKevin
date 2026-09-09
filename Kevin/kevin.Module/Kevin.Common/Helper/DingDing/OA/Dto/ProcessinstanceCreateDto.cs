using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kevin.Common.Helper.DingDing.OA.Dto
{
    /// <summary>
    /// 发起审批实例请求（/topapi/processinstance/create）
    /// 可选项一律用可空类型并忽略null：钉钉对传空值的可选参数（如dept_id=0、approvers=null）会直接报参数错误，
    /// 而序列化走 JsonExtension.ToJson（Newtonsoft全局设置默认输出null），不加忽略标记就会把未赋值的可选参数一起发出去
    /// </summary>
    public class ProcessinstanceCreateDto
    {
        /// <summary>
        /// 应用标识（仅第三方企业应用指定审批模板时需要，企业内部应用不传）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? agent_id { get; set; }
        /// <summary>
        /// 审批模板唯一标识（processCode）
        /// </summary>
        public string process_code { get; set; }
        /// <summary>
        /// 审批实例发起人的钉钉userid
        /// </summary>
        public string originator_user_id { get; set; }
        /// <summary>
        /// 发起人所属部门ID（非企业内部门传-1）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? dept_id { get; set; }
        /// <summary>
        /// 审批人列表（不传则按模板配置的审批流程流转）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> approvers { get; set; }
        /// <summary>
        /// 抄送人列表
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> cc_list { get; set; }
        /// <summary>
        /// 抄送节点位置（START/FINISH）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cc_position { get; set; }
        /// <summary>
        /// 审批表单控件值
        /// </summary>
        public List<form_component_valuesItem> form_component_values { get; set; }
        /// <summary>
        /// 指定审批人（新版，层级用"|"分隔）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> approvers_v2 { get; set; }
    }

    public class form_component_valuesItem
    {
        /// <summary>
        /// 控件ID（按控件名称提交时可留空）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }
        /// <summary>
        /// 控件名称（须与审批模板中的控件标题一致）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 控件值（明细表格控件为行数组的JSON字符串）
        /// </summary>
        public string value { get; set; }
    }
}
