using kevin.Domain.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    /// <summary>
    /// App测试代码生成器
    /// </summary>
    [Table("TAppCodeTest")]
    [Description("App测试代码生成器")]
    public class TAppCodeTest : CUD_User
    {
        /// <summary>
        /// DemoName
        /// </summary>
        [StringLength(200)]
        [Description("DemoName")]
        public string DemoName { get; set; } = "";
    }
}
