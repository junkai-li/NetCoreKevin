using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.System
{
    /// <summary>
    /// 附件dto
    /// </summary>
    public class FileDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public long CreateUserId { get; set; }
        public virtual string CreateUser { get; set; } = "";

        /// <summary>
        /// 文件名称
        /// </summary>
        [StringLength(100)]
        [Description("文件名称")]
        public string? Name { get; set; }


        /// <summary>
        /// 保存路径
        /// </summary>
        [StringLength(200)]
        [Description("保存路径")]
        public string? Path { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        [StringLength(500)]
        [Description("Url")]
        public string Url { get; set; }

        /// <summary>
        /// 外链表名
        /// </summary>
        [StringLength(50)]
        [Description("外链表名")]
        public string? Table { get; set; }




        /// <summary>
        /// 外链表ID
        /// </summary>  
        [Description("外链表ID")]
        public string TableId { get; set; } = "";


        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(50)]
        [Description("标记")]
        public string? Sign { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }
    }
}
