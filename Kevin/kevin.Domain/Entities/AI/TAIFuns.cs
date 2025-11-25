using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI方法
    /// </summary>
    public partial class TAIFuns : CUD_User
    {
        /// <summary>
        /// 接口描述
        /// </summary>
        [Required]
        public string Path { get; set; }
    }
}
