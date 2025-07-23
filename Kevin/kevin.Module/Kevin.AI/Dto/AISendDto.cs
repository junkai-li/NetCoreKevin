using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.Dto
{
    public class AISendDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<MessagesItem> messages { get; set; }

        /// <summary>
        /// 是否开启联网搜索
        /// </summary>
        public bool enable_search { get; set; }
    }
    public class MessagesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string role { get; set; }
        /// <summary>
        ///  
        /// </summary>
        public string content { get; set; }
    }
}
