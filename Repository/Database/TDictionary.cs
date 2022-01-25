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
        [StringLength(100)]
        public string Key { get; set; }



        /// <summary>
        /// 值
        /// </summary>
        [StringLength(100)]
        public string Value { get; set; }



        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
