using Repository.Bases;

namespace Repository.Database
{

    /// <summary>
    /// 计数表
    /// </summary>
    public class TCount:CUD
    {


        /// <summary>
        /// 标记
        /// </summary>
        public string Tag { get; set; }



        /// <summary>
        /// 计数
        /// </summary>
        public int Count { get; set; }

    }
}
