
using Repository.Database.Bases;

namespace Repository.Database
{


    /// <summary>
    /// 友情链接表
    /// </summary>
    public class TLink : CD_User
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
