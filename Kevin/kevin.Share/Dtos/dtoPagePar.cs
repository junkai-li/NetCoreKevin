using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos
{
    public class dtoPagePar<T>
    {
        /// <summary>
        /// 条件Id
        /// </summary>
        public Guid whereId { get; set; }
        /// <summary>
        /// 条件Key
        /// </summary>
        public string searchKey { get; set; } = "";
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; } = "";
        /// <summary>
        /// 标识
        /// </summary>
        public string sign { get; set; } = "";
        /// <summary>
        /// 分页大小
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 分页页数
        /// </summary>
        public int pageNum { get; set; }

        /// <summary>
        /// 数据总量
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 自定义参数内容
        /// </summary>
        public T Parameter { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int GetSkip()
        {
            return (this.pageNum - 1) * this.pageSize;
        }
    }
}
