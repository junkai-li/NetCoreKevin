namespace kevin.Domain.Kevin
{

    /// <summary>
    /// 日志表
    /// </summary>
    public class TLog:CD
    {


        /// <summary>
        /// 标记
        /// </summary>
        [StringLength(50)]
        public string Sign { get; set; }


        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(50)]
        public string Type { get; set; }



        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

    }
}
