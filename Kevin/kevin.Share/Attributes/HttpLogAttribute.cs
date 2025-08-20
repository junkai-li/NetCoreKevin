using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpLogAttribute : Attribute
    {
        [MaxLength(20)]
        public string OperateType { get; set; } = "未知";
        [MaxLength(100)]
        public string OperateRemark { get; set; } = "未知";

        public bool IsLog { get; set; }
        public HttpLogAttribute(string operateType, string operateRemark, bool isLog = true)
        {
            this.OperateType = operateType;
            this.OperateRemark = operateRemark;
            this.IsLog = isLog;
        }
        public HttpLogAttribute(bool isLog)
        {
            this.IsLog = isLog;
        }
    }
}
