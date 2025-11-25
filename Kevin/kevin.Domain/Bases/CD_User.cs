using kevin.Domain.Bases;
using kevin.Domain.Kevin;

namespace kevin.Domain.Bases
{

    /// <summary>
    /// 创建，编辑，删除，并关联了用户
    /// </summary>
    public class CD_User : CD
    {

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public Guid CreateUserId { get; set; }

        [ForeignKey("CreateUserId")]
        public virtual TUser? CreateUser { get; set; }



        /// <summary>
        /// 删除人ID
        /// </summary>
        [Description("删除人ID")]
        public Guid? DeleteUserId { get; set; }

        [ForeignKey("DeleteUserId")]
        public virtual TUser? DeleteUser { get; set; }
    }
}
