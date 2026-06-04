using System.Collections.Generic;

namespace Kevin.Common.Helper.DingDing.OA.Dto
{
    public class ProcessinstanceGetResultDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Process_instance process_instance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string request_id { get; set; }
    }
    public class Form_component_valuesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string component_type { get; set; }
        /// <summary>
        /// 请假天数
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
    }

    public class TasksItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string task_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string activity_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string task_result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string finish_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string taskid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
    }

    public class Operation_recordsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string operation_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string operation_result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
    }

    public class Process_instance
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Form_component_valuesItem> form_component_values { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> attached_process_instance_ids { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> cc_userids { get; set; }
        /// <summary>
        /// 测试2
        /// </summary>
        public string originator_dept_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string originator_userid { get; set; }
        /// <summary>
        /// 测试用户提交的请假
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string finish_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string originator_dept_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string business_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TasksItem> tasks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string biz_action { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Operation_recordsItem> operation_records { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
    } 

}
