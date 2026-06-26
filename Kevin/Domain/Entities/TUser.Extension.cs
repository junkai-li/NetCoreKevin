namespace kevin.Domain.Entities
{
    public partial class TUser
    {
        /// <summary>
        /// BI用户Id
        /// </summary>
        [Description("BI用户Id")]
        [StringLength(100)]
        public string? KcUserId { get; set; }
    }
}
