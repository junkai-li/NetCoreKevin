namespace Repository.Database
{
    /// <summary>
    /// 用户详细信息表
    /// </summary>
    public class TUserInfo : CD
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        public virtual TUser User { get; set; }


        /// <summary>
        /// 地址区域ID
        /// </summary>
        public int RegionAreaId { get; set; }
        public virtual TRegionArea RegionArea { get; set; }



        /// <summary>
        /// 地址详细信息
        /// </summary>
        [StringLength(200)]
        public string Address { get; set; }


        /// <summary>
        /// 个性签名
        /// </summary>
        [StringLength(200)]
        public string Signature { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        public bool? Sex { get; set; }


        /// <summary>
        /// 公司名称
        /// </summary>
        [StringLength(200)]
        public string Company { get; set; }



        /// <summary>
        /// 职务
        /// </summary>
        [StringLength(200)]
        public string Position { get; set; }



        /// <summary>
        /// 微信号
        /// </summary>
        [StringLength(200)]
        public string WeChat { get; set; }


        /// <summary>
        /// QQ
        /// </summary>
        [StringLength(100)]
        public string QQ { get; set; }
    }
}
