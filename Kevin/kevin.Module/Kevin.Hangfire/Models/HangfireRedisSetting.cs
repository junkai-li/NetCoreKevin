using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Hangfire.Models
{
    public class HangfireRedisSetting
    {
        public int Db { get; set; } = 0;
        public string RedisConnectionString { get; set; } = "";

        public string Prefix { get; set; } = "{hangfire}:";
    }
}
