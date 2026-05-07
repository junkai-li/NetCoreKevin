namespace Kevin.Hangfire.Models
{
    public class HangfireRedisSetting
    {
        public int Db { get; set; } = 0;
        public string RedisConnectionString { get; set; } = "";

        public string Prefix { get; set; } = "{hangfire}:";
    }
}
