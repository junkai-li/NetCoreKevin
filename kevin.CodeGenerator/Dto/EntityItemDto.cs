using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.CodeGenerator.Dto
{
    public class EntityItemDto
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
