using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.CodeGenerator.Dto
{
    public class EntityItemDto
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        public string EntityName { get; set; } = "";
        public string Description { get; set; } = "";

        /// <summary>
        /// 仓储接口生成路径
        /// </summary>
        public string IRpBulidPath { get; set; } = "";
        /// <summary>
        /// 仓储生成路径
        /// </summary>
        public string RpBulidPath { get; set; } = "";

        /// <summary>
        /// 服务接口生成路径
        /// </summary>
        public string IServiceBulidPath { get; set; } = "";

        /// <summary>
        /// 服务生成路径
        /// </summary>
        public string ServiceBulidPath { get; set; } = "";
    }
}
