namespace Kevin.Authentication.Jwt.Dto
{
    public class UserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary> 
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// Password //存储哈希
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 创建人;
        /// </summary> 
        public string CreatedBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary> 
        public virtual DateTime? CreatedTime { get; set; }

        public int TenantId { get; set; }
    }
}
