using Repository.Bases;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Database
{


    /// <summary>
    /// 文件分片上传状态记录表
    /// </summary>
    public class TFileGroup :CUD
    {


        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }
        public virtual TFile File { get; set; }



        /// <summary>
        /// 文件唯一值
        /// </summary>
        public string Unique { get; set; }


        /// <summary>
        /// 分片数
        /// </summary>
        public int Slicing { get; set; }



        /// <summary>
        /// 合成状态
        /// </summary>
        public bool Issynthesis { get; set; }



        /// <summary>
        /// 是否已完整传输
        /// </summary>
        public bool Isfull { get; set; }
    }
}
