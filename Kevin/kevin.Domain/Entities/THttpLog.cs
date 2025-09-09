using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities
{
    /// <summary>
    /// 请求日志表
    /// </summary> 
    [Table("THttpLog")]
    [Description("请求日志表")]
    public class THttpLog : CD
    {
        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        [Description("删除人ID")]
        public Guid? DeleteUserId { get; set; }

        /// <summary>
        /// 登录人
        /// </summary>
        [Description("登录人")]
        public String? user_name { get; set; }

        /// <summary>
        /// ip
        /// </summary>
        [Description("ip")]
        [MaxLength(125)]
        public String? ip { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        [Description("设备")]
        [MaxLength(500)]
        public String? device { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [Description("url")] 
        public String? http_url { get; set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        [Description("请求内容")] 
        public String? http_body { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [Description("请求方法")]
        [MaxLength(500)]
        public String? http_method { get; set; }

        /// <summary>
        /// 请求url不带参数
        /// </summary>
        [Description("请求url不带参数")]
        [MaxLength(500)]
        public String? http_action { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Description("操作类型")]
        [MaxLength(50)]
        public String? operate_type { get; set; }

        /// <summary>
        /// 操作备注
        /// </summary>
        [Description("操作备注")]
        [MaxLength(500)]
        public String? operate_remark { get; set; }
    }
}
