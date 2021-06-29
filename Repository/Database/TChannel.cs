using Repository.Database.Bases;
using System.Collections.Generic;

namespace Repository.Database
{


    /// <summary>
    /// 频道信息表
    /// </summary>
    public class TChannel : CD_User
    {

        /// <summary>
        /// 频道名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }




        /// <summary>
        /// 所包含的类别记录数据
        /// </summary>
        public virtual List<TCategory> TCategory { get; set; }
    }
}
