using Common;
using Kevin.AI.Dto;
using Kevin.Common.Extension;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kevin.AI
{
    public class AIClient : IAIClient
    {
        private readonly string AIUrl;


        private readonly string AIKeySecret;

        private readonly string AIDefaultModel;


        public AIClient(IOptionsMonitor<AISetting> config)
        {
            AIUrl = config.CurrentValue.AIUrl;
            AIKeySecret = config.CurrentValue.AIKeySecret;
            AIDefaultModel = config.CurrentValue.AIDefaultModel;
        }

        public AIReslutDto SendMsg(string msg, string model = "", string systemMsg = "")
        {
            if (String.IsNullOrEmpty(model))
            {
                model = AIDefaultModel;
            }
            var datajson = new AISendDto
            {
                model = model,
                messages = new List<MessagesItem>()
            };
            if (!string.IsNullOrEmpty(systemMsg))
            {
                datajson.messages.Add(new MessagesItem { role = "system", content = systemMsg });
            }
            if (!string.IsNullOrEmpty(msg))
            {
                datajson.messages.Add(new MessagesItem { role = "user", content = msg });
            }
            var json = HttpHelper.Post(AIUrl, datajson.ToJson(), "json",headers: new Dictionary<string, string> {  {"Authorization",AIKeySecret },}, isSkipSslVerification: true);
            var result = JsonExtension.ToObject<AIReslutDto>(json);
            return result;
        }
    }
}
