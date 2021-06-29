using Repository.Bases;

namespace Repository.Database
{

    /// <summary>
    /// 字典信息表
    /// </summary>
    public class TDictionary : CD
    {


        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }



        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }



        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
