namespace kevin.Domain.Kevin
{
    /// <summary>
    /// 用户详细信息表
    /// </summary>
    [Table("TUserInfo")]
    [Description("用户详细信息表")]
    public partial class TUserInfo : CD
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public Guid UserId { get; set; }
        public virtual TUser? User { get; set; }


        /// <summary>
        /// 地址区域ID
        /// </summary>
        [Description("地址区域ID")]
        public int RegionAreaId { get; set; }
        public virtual TRegionArea? RegionArea { get; set; }



        /// <summary>
        /// 地址详细信息
        /// </summary>
        [StringLength(200)]
        [Description("地址详细信息")]
        public string? Address { get; set; }


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
        /// 公司名称
        /// </summary>
        [StringLength(200)]
        [Description("公司名称")]
        public string? Company { get; set; }



        /// <summary>
        /// 职务
        /// </summary>
        [StringLength(200)]
        [Description("职务")]
        public string? Position { get; set; }



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
