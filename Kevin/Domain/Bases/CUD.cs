using System;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Bases
{

    /// <summary>
    /// 创建，编辑，删除
    /// </summary>
    public class CUD:CD
    {


        /// <summary>
        /// 更新时间
        /// </summary> 
        [Description("更新时间")]
        public DateTime? UpdateTime { get; set; }

    }
}
