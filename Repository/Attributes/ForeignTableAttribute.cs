using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Attributes
{

    /// <summary>
    /// 外连表名称
    /// </summary>
    public class ForeignTableAttribute : Attribute
    {

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

    }
}
