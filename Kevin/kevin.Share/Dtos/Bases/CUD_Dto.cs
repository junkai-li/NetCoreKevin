namespace kevin.Domain.Share.Dtos.Bases
{

    /// <summary>
    /// 创建，编辑，删除
    /// </summary>
    public class CUD_Dto:CD_Dto
    { 
        /// <summary>
        /// 更新时间
        /// </summary>  
        public DateTime? UpdateTime { get; set; }

    }
}
