using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.System
{
    public class HttpLogDto
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public Guid CreateUserId { get; set; } 

        /// <summary>
        /// 登录人
        /// </summary>
        [Description("登录人")]
        public String? UserName { get; set; }

        /// <summary>
        /// ip
        /// </summary>
        [Description("ip")]
        [MaxLength(125)]
        public String? IP { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        [Description("设备")]
        [MaxLength(500)]
        public String? Device { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [Description("url")]
        public String? HttpUrl { get; set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        [Description("请求内容")]
        public String? HttpBody { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [Description("请求方法")]
        [MaxLength(500)]
        public String? HttpMethod { get; set; }

        /// <summary>
        /// 请求url不带参数
        /// </summary>
        [Description("请求url不带参数")]
        [MaxLength(500)]
        public String? HttpAction { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Description("操作类型")]
        [MaxLength(50)]
        public String? OperateType { get; set; }

        /// <summary>
        /// 操作备注
        /// </summary>
        [Description("操作备注")]
        [MaxLength(500)]
        public String? OperateRemark { get; set; }
    }
}
