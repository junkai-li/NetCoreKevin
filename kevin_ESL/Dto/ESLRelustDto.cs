using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace kevin_ESL.Dto
{
    public class ESLRelustDto
    {
        [JsonPropertyName("Event-Name")]
        public string EventName { get; set; }

        [JsonPropertyName("Core-UUID")]
        public string CoreUuid { get; set; }



        [JsonPropertyName("Unique-ID")]
        public string UniqueId { get; set; }

        /// <summary>
        ///执行时间 （豪秒）
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// 1.拒绝接通||未接通 2.接通并挂断  
        /// </summary>
        public int kaiyin_Status { get; set; }

        public string kaiyin_Msg { get; set; }

        public bool kaiyin_IsErr { get; set; }

        public string variable_detect_speech_result { get; set; }
    }
}
