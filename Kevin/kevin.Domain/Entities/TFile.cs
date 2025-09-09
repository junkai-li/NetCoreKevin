﻿namespace kevin.Domain.Kevin
{


    /// <summary>
    /// 文件表
    /// </summary>
    [Table("TFile")]
    [Description("文件表")]
    public partial class TFile : CD_User
    {


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
