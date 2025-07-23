using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin_ESL.Const
{
    public class EventNameConst
    {
        /// <summary>
        /// 通道呼叫状态事件
        /// </summary>
        public const string CHANNEL_CALLSTATE = "CHANNEL_CALLSTATE";

        /// <summary>
        /// 通道应答事件
        /// </summary>
        public const string CHANNEL_ANSWER = "CHANNEL_ANSWER";

        ///// <summary>
        ///// 通道创建事件
        ///// </summary>
        //public const string CHANNEL_CREATE = "CHANNEL_CREATE";

        /// <summary>
        /// 通道销毁事件
        /// </summary>
        public const string CHANNEL_DESTROY = "CHANNEL_DESTROY";

        /// <summary>
        /// 通道执行完成事件
        /// </summary>
        public const string CHANNEL_EXECUTE_COMPLETE = "CHANNEL_EXECUTE_COMPLETE";

        /// <summary>
        /// 通道挂断完成事件
        /// </summary>
        public const string CHANNEL_HANGUP_COMPLETE = "CHANNEL_HANGUP_COMPLETE";

        /// <summary>
        /// 通道挂断事件
        /// </summary>
        public const string CHANNEL_HANGUP = "CHANNEL_HANGUP";

        /// <summary>
        /// 按键事件
        /// </summary>
        public const string DTMF = "DTMF";
        /// <summary>
        /// 接入通道
        /// </summary>
        public const string CHANNEL_STATE = "CHANNEL_STATE";

        /// <summary>
        /// 异常
        /// </summary>
        public const string Err = "Err";


    }
}
