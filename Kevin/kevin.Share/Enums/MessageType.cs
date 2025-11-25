using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Enums
{
    public enum MessageType
    {
   
        [Description("业务公告")]
        Announcement = 1, 
        [Description("私人私信")]
        PrivateUser = 2, 
        [Description("公共系统消息")]
        System = 3,
        [Description("私人系统消息")]
        PrivateUserSystem = 4,
        [Description("所有消息")]//只用于筛选
        All =5
    }
}
