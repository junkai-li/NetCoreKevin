using Repository.Database.Bases;
using System;

namespace Repository.Database
{


    /// <summary>
    /// 文件表
    /// </summary>
    public class TFile : CD_User
    {


        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 保存路径
        /// </summary>
        public string Path { get; set; }


        /// <summary>
        /// 外链表名
        /// </summary>
        public string Table { get; set; }


        /// <summary>
        /// 外链表ID
        /// </summary>
        public Guid TableId { get; set; }


        /// <summary>
        /// 标记
        /// </summary>
        public string Sign { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
