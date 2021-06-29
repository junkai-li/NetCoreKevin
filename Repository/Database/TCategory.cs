using Repository.Database.Bases;
using System;
using System.Collections.Generic;

namespace Repository.Database
{

    /// <summary>
    /// 栏目信息表
    /// </summary>
    public class TCategory : CD_User
    {


        /// <summary>
        /// 频道ID
        /// </summary>
        public Guid ChannelId { get; set; }
        public virtual TChannel Channel { get; set; }


        /// <summary>
        /// 栏目名目
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 父级栏目ID
        /// </summary>
        public Guid? ParentId { get; set; }
        public virtual TCategory Parent { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }




        public virtual List<TArticle> TArticle { get; set; }
    }
}
