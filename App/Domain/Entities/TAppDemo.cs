using kevin.Domain.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    /// <summary>
    /// AppDemo
    /// </summary>
    [Table("TAppDemo")]
    [Description("App演示")]
    public class TAppDemo : CUD_User
    {
        /// <summary>
        /// DemoName
        /// </summary>
        [StringLength(200)]
        [Description("DemoName")]
        public string DemoName { get; set; } = ""; 
    }
}
