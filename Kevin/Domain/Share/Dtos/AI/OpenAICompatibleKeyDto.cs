namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 当前用户 OpenAI 兼容接口 Key 信息（不含明文）
    /// </summary>
    public class OpenAICompatibleKeyInfoDto
    {
        /// <summary>是否已生成过 Key</summary>
        public bool IsExists { get; set; }

        /// <summary>记录 Id（仅当 IsExists=true 时有值）</summary>
        public long? Id { get; set; }

        /// <summary>绑定的用户 Id</summary>
        public long? UserId { get; set; }

        /// <summary>脱敏展示的 Key，例如 <c>sk-kv-AbCd…WxYz</c></summary>
        public string? MaskKey { get; set; }

        /// <summary>用户备注名</summary>
        public string? Name { get; set; }

        /// <summary>是否启用</summary>
        public bool IsEnabled { get; set; }

        /// <summary>创建时间</summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>最近一次被网关命中时间</summary>
        public DateTime? LastUsedTime { get; set; }
    }

    /// <summary>
    /// 对接文档数据：Base URL、可调用的 model 列表、cURL/Python/Node.js 示例
    /// </summary>
    public class OpenAICompatibleIntegrationDocDto
    {
        /// <summary>对接 Base URL，例如 https://your-host/api/v1</summary>
        public string BaseUrl { get; set; } = "";

        /// <summary>当前用户可用的智能体列表（id + name），供 model 字段下拉选择</summary>
        public List<OpenAICompatibleIntegrationModelItem> Models { get; set; } = new();

        /// <summary>代码示例（实时替换 key/model）</summary>
        public OpenAICompatibleIntegrationSamples Samples { get; set; } = new();
    }

    public class OpenAICompatibleIntegrationModelItem
    {
        public long Id { get; set; }

        public string Name { get; set; } = "";

        public string? Describe { get; set; }
    }

    public class OpenAICompatibleIntegrationSamples
    {
        public string Curl { get; set; } = "";

        public string Python { get; set; } = "";

        public string NodeJs { get; set; } = "";
    }
}