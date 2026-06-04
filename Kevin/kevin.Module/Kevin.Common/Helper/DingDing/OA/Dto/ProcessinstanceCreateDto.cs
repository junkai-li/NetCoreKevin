using System.Collections.Generic;

namespace Kevin.Common.Helper.DingDing.OA.Dto
{
    public class ProcessinstanceCreateDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int agent_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string process_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string originator_user_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> approvers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> cc_list { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cc_position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<form_component_valuesItem> form_component_values { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> approvers_v2 { get; set; }
    }

    public class form_component_valuesItem
            {
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
}
}
