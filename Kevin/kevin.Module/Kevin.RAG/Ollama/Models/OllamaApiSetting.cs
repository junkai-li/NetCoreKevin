using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Ollama.Models
{
    public class OllamaApiSetting
    {
        public string Url { get; set; } = "http://****/v1";

        public string? ApiKey { get; set; }
        public string DefaultModel { get; set; } = "modelscope.cn/Qwen/Qwen3-Embedding-0.6B-GGUF:latest";
    }
}
