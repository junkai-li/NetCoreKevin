using kevin.Share.Dtos;
using kevin.Share.Dtos.System;

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
        public string? Name { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }
        
        public List<dtoRole>? Roles { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? PassWord { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        ///最近登陆时间
        /// </summary>
        public DateTime? RecentLoginTime { get; set; }

        public List<dtoKeyValue>? HeadImgs { get; set; }

    }
}
