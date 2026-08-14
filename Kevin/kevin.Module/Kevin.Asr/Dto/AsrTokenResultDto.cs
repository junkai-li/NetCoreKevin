using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Asr.Dto
{
    public class AsrTokenResultDto
    {
        public string AppKey { get; set; } = "";
        public string Token { get; set; } = "";
        public long ExpireTime { get; set; }
    }
}
