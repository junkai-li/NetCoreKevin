using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Libraries.WeiXin.App.Models
{

    /// <summary>
    /// 获取用户信息接口返回数据类型
    /// </summary>
    public class GetUserInfo
    {
        public string openid { get; set; }

        public string nickname { get; set; }

        public int sex { get; set; }

        public string province { get; set; }

        public string city { get; set; }

        public string country { get; set; }

        public string headimgurl { get; set; }

        public string[] privilege { get; set; }

        public string unionid { get; set; }
    }
}
