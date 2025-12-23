using kevin.Domain.Entities.Organizational;

namespace kevin.Domain.Entities
{
    /// <summary>
    /// 用户绑定岗位表
    /// </summary>
    [Table("TUserBindPosition")]
    [Description("用户绑定岗位表")]
    public class TUserBindPosition : CUD_User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public long UserId { get; set; }
        public virtual TUser? User { get; set; }
        /// <summary>
        /// 岗位信息
        /// </summary>
        [Description("岗位Id")]
        public long PositionId { get; set; }
        public virtual TPosition? Position { get; set; }
    }
}
