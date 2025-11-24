using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.System
{
    public class OSLogDto
    {
        /// <summary>
        /// 主键标识ID
        /// </summary>
        [Description("主键标识ID")]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

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
        public Guid TableId { get; set; }



        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(100)]
        [Description("标记")]
        public string? Sign { get; set; }



        /// <summary>
        /// 变动内容
        /// </summary> 
        [Description("变动内容")]
        public string? Content { get; set; }



        /// <summary>
        /// 操作人信息
        /// </summary>
        [Description("操作人信息")]
        public Guid? ActionUserId { get; set; } 
        public string? ActionUserName { get; set; } 



        /// <summary>
        /// 备注
        /// </summary> 
        [Description("备注")]
        public string? Remarks { get; set; }



        /// <summary>
        /// Ip地址
        /// </summary>
        [StringLength(100)]
        [Description("Ip地址")]
        public string? IpAddress { get; set; }



        /// <summary>
        ///  设备标记
        /// </summary> 
        [Description("设备标记")]
        public string? DeviceMark { get; set; }
    }
}
