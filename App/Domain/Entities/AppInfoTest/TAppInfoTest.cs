using kevin.Domain.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.AppInfoTest
{
    /// <summary>
    /// TAppInfo
    /// </summary>
    [Table("TAppInfo")]
    [Description("TAppInfo")]
    public class TAppInfoTest : CUD_User
    {
        /// <summary>
        /// Name
        /// </summary>
        [StringLength(200)]
        [Description("Name")]
        public string Name { get; set; } = "";
    }
}
