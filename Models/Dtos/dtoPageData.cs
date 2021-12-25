using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dtos
{
    /// <summary>
    /// 分页dto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class dtoPageData<T>
    {
        /// <summary>
        /// 条件Id
        /// </summary>
        public Guid whereId { get; set; }
        /// <summary>
        /// 条件Key
        /// </summary>
        public string searchKey { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public string sign { get; set; }
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
        /// 具体数据内容
        /// </summary>
        public List<T> data { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
