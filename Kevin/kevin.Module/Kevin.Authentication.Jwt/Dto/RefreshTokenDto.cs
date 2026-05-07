using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Authentication.Jwt.Dto
{
    public class RefreshTokenDto
    { 
        
        /// <summary>
        /// 刷新Token过期时间
        /// </summary>
        public DateTime RefreshExpireTime { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserDto User { get; set; }
    }
}
