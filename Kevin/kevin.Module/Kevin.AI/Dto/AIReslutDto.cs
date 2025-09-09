using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.Dto
{
    public class Message
    {
        /// <summary>
        ///  
        /// </summary>
        public string? content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? role { get; set; }
    }

    public class ChoicesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string? finish_reason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Message? message { get; set; }
    }

    public class Usage
    {
        /// <summary>
        /// 
        /// </summary>
        public int completion_tokens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int prompt_tokens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total_tokens { get; set; }
    }

    public class AIReslutDto
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ChoicesItem>? choices { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int created { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? request_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Usage? usage { get; set; }
    }
}
