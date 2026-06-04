using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIMsgService
    {
        /// <summary>
        /// 发送钉钉消息给自己
        /// </summary>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        [Description("发送钉钉消息给自己， 用于把消息发送给自己的钉钉账户。以 ❌ 开头的错误信息。")]
        public  string SendDDToMyMsg([Description("消息内容")] string msgContent);

        /// <summary>
        /// 发送钉钉消息给其他用户
        /// </summary>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        [Description("发送钉钉消息给其他用户， 用于把消息发送给指定的钉钉账户。以 ❌ 开头的错误信息。")]
        public string SendDDToUserMsg([Description("消息内容")] string msgContent, [Description("发送用户名称")] string userName);
    }
}
