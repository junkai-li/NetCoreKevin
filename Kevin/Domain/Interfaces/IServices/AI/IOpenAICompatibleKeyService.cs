using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// OpenAI 协议兼容接口的用户级 API Key 服务：
    /// 每个用户一条 Key；Key 落库为 AES 加密后的密文，明文仅在生成/主动查看时一次性返回。
    /// </summary>
    public interface IOpenAICompatibleKeyService : IBaseService
    {
        /// <summary>
        /// 获取当前用户的 Key 信息（不含明文，仅返回脱敏展示与元数据）
        /// </summary>
        Task<OpenAICompatibleKeyInfoDto> GetMyKey();

        /// <summary>
        /// 生成新的 Key；若已存在则覆盖（先物理删除旧记录再插入）。返回明文 Key 字符串。
        /// <para>这是唯一一次明文输出的机会；前端必须立即展示并强制用户保存。</para>
        /// </summary>
        /// <param name="name">用户备注名（可选）</param>
        Task<string> GenerateMyKey(string? name);

        /// <summary>
        /// 查看当前 Key 明文（手动解密返回）。需要调用方具备权限校验上下文。
        /// </summary>
        Task<string> RevealMyKey();

        /// <summary>
        /// 修改备注名
        /// </summary>
        Task<bool> UpdateMyKeyName(string? name);

        /// <summary>
        /// 启用 / 禁用当前 Key
        /// </summary>
        Task<bool> SetMyKeyEnabled(bool enabled);

        /// <summary>
        /// 物理删除当前 Key（用户主动销毁）
        /// </summary>
        Task<bool> DeleteMyKey();

        /// <summary>
        /// 获取对接文档数据：Base URL、可调用的智能体列表、cURL/Python/Node.js 示例
        /// </summary>
        /// <param name="baseUrl">外部传入的 BaseUrl（由 Controller 拼 Request.Scheme+Host），便于多环境切换</param>
        Task<OpenAICompatibleIntegrationDocDto> GetMyIntegrationDoc(string baseUrl);
    }
}