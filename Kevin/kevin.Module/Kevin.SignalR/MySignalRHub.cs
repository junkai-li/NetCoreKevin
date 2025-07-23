using Microsoft.AspNetCore.SignalR;

namespace Kevin.SignalR
{
    public class MySignalRHub : Hub
    { 
        /// <summary>
        /// 链接
        /// </summary>
        /// <returns></returns> 
        public override async Task OnConnectedAsync()
        {
            var token = Context.GetHttpContext().Request.Query["token"].ToString();
            if (string.IsNullOrEmpty(token))
            { 
                throw new Exception($"token不存在");
            }   
            Console.WriteLine(Context.ConnectionId + token + "链接");
            await base.OnConnectedAsync();
        }
        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var token = Context.GetHttpContext().Request.Query["token"].ToString(); 
                Console.WriteLine(Context.ConnectionId + token + "断开");
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }

        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        { 
            base.Dispose(); ;
        }
        /// <summary>
        /// 发送公告消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task SendPublicMsg(string msg)
        {
            return this.Clients.All.SendAsync("AllMsg", msg);
        }
        /// <summary>
        /// 私发信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task SendUserMsg(string connId, string msg)
        {
            string newmsg = $"{connId}{DateTime.Now}:{msg}";
            return this.Clients.Client(connId).SendAsync("ReceptionUserMsg", newmsg);
        } 
    }
}
