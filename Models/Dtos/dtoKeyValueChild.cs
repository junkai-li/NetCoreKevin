using System.Collections.Generic;

namespace Models.Dtos
{
    public class dtoKeyValueChild
    {



        /// <summary>
        /// 键
        /// </summary>
        public object Key { get; set; }




        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }




        /// <summary>
        /// 子级集合信息
        /// </summary>
        public List<dtoKeyValueChild> ChildList { get; set; }



    }
}
