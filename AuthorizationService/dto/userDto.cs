using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService
{
    public class userDto
    {

        public string Id { get; set; } 
        public   string Name { get; set; } 
        public  string Password { get; set; } 
        public  string Phone { get; set; }

        /// <summary>
        /// 创建人;
        /// </summary> 
        public   string CreatedBy { get; set; } 

        public virtual DateTime? CreatedTime { get; set; }

    }
}
