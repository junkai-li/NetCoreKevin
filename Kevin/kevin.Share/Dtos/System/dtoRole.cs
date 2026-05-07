using System.ComponentModel.DataAnnotations;

namespace kevin.Share.Dtos.System
{
    public class dtoRole
    {  /// <summary>
       /// Id
       /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "角色名称不能为空")]
        public string Name { get; set; } = "";


        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remarks { get; set; } = "";

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
