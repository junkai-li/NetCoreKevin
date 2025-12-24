using kevin.Domain.Entities;
using kevin.Domain.Share.Enums;

namespace kevin.Domain.Entities
{
    /// <summary>
    /// 用户详细信息表
    /// </summary>
    [Table("TUserInfo")]
    [Description("用户详细信息表")]
    public partial class TUserInfo : CUD_User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public long UserId { get; set; }
        public virtual TUser? User { get; set; }

        /// <summary>
        /// 员工状态 0:离职 1:在职 2:休假 3:停职 4:退休 5:实习
        /// </summary>
        [Description("员工状态 -1:离职 1:在职 2:休假 3:停职 4:退休 5:实习")]
        public EmployeeStatus EmployeeStatus { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        [Description("入职时间")]
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [StringLength(200)]
        [Description("个性签名")]
        public string? Signature { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        [Description("性别")]
        public bool? Sex { get; set; }


        /// <summary>
        /// 部门ID
        /// </summary> 
        [Description("部门ID")]
        public long? DepartmentId { get; set; } 

        /// <summary>
        /// 上级用户id
        /// </summary>
        [Description("上级用户id")]
        public long? SupervisorId { get; set; } 
        /// <summary>
        /// 工号
        /// </summary>
        [StringLength(200)]
        public string? EmployeeNo { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        [StringLength(200)]
        public string? WeChat { get; set; }


        /// <summary>
        /// QQ
        /// </summary>
        [StringLength(100)]
        public string? QQ { get; set; }
    }
}
