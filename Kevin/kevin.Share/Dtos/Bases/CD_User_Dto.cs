namespace kevin.Domain.Share.Dtos.Bases
{

    /// <summary>
    /// 创建，编辑，删除，并关联了用户
    /// </summary>
    public class CD_User_Dto : CD_Dto
    {

        /// <summary>
        /// 创建人ID
        /// </summary> 
        public long CreateUserId { get; set; } 
        public virtual string? CreateUser { get; set; } 

        /// <summary>
        /// 删除人ID
        /// </summary> 
        public long? DeleteUserId { get; set; } 
        public virtual string? DeleteUser { get; set; }
    }
}
