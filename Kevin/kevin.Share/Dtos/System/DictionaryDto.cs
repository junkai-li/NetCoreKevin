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
    public class DictionaryDto
    {
        /// <summary>
        /// 主键标识ID
        /// </summary>
        [Description("主键标识ID")]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public Guid CreateUserId { get; set; } 
        public virtual string? CreateUserName { get; set; }


        /// <summary>
        /// 编辑人ID
        /// </summary>
        [Description("编辑人ID")]
        public Guid? UpdateUserId { get; set; } 
        public virtual string? UpdateUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        [StringLength(200)]
        [Description("键")]
        public string? Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [StringLength(200)]
        [Description("值")]
        public string? Value { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(100)]
        [Description("类型")]
        public string? Type { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(500)]
        [Description("备注信息")]
        public string? Remarks { get; set; }

        /// <summary>
        /// 是否系统内置
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }
    }
}
