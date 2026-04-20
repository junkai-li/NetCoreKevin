namespace Kevin.Hangfire.Models
{
    public class HangfireSetting
    {
        /// <summary>
        /// 是否启用Hangfire Dashboard
        /// </summary>
        public bool IsUseHangfireDashboard { get; set; } = true;
        /// <summary>
        /// 地址
        /// </summary>
        public string DashboardUrl { get; set; } = "pchangfire";
        /// <summary>
        /// 标题
        /// </summary>
        public string DashboardTitle { get; set; } = "Kevin Hangfire";

        /// <summary>
        /// 需要SSL连接才能访问HangFire
        /// </summary>
        public bool RequireSsl { get; set; } = false;
        /// <summary>
        /// 是否将所有非SSL请求重定向到SSL URL
        /// </summary>
        public bool SslRedirect { get; set; } = false;
        /// <summary>
        /// 是否区分大小写
        /// </summary>
        public bool LoginCaseSensitive { get; set; } = true;
        /// <summary>
        /// 用户设置
        /// </summary>
        public List<HangfireUserSetting> userSetting { get; set; } = new List<HangfireUserSetting>() { };
    };
}
