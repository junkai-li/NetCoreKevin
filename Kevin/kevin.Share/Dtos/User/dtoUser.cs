using kevin.Share.Dtos;

namespace kevin.Domain.Share.Dtos.User
{
    public class dtoUser
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public List<dtoKeyValue> HeadImgs { get; set; }

    }
}
