using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Kevin.Web.Basics.Controllers.AI.OpenAI
{
    /// <summary>
    /// OpenAI 协议兼容接口的"我的 Key"管理：当前用户生成、查看、启用/禁用、对接文档。
    /// <para>
    /// 每个登录用户至多一条 Key；Key 落库为 AES 加密密文，外部命中由网关层解密匹配。
    /// </para>
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("OpenAI对接密钥管理", "OpenAICompatibleKey")]
    public class OpenAICompatibleKeyController : ControllerBase
    {
        private IOpenAICompatibleKeyService _service { get; set; }

        public OpenAICompatibleKeyController(IOpenAICompatibleKeyService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 获取当前用户的 Key 信息（不含明文）
        /// </summary>
        [HttpGet("GetMyKey")]
        [ActionDescription("获取我的OpenAI兼容接口Key")]
        public async Task<OpenAICompatibleKeyInfoDto> GetMyKey()
        {
            return await _service.GetMyKey();
        }

        /// <summary>
        /// 生成或重新生成当前用户的 Key，返回明文（仅此次返回明文）。
        /// </summary>
        [HttpPost("Generate")]
        [ActionDescription("生成OpenAI兼容接口Key")]
        public async Task<string> Generate([FromBody] GenerateOpenAIKeyRequest? body)
        {
            return await _service.GenerateMyKey(body?.Name);
        }

        /// <summary>
        /// 查看当前 Key 明文（用户主动点击"显示"时调用）
        /// </summary>
        [HttpPost("Reveal")]
        [ActionDescription("查看OpenAI兼容接口Key明文")]
        public async Task<string> Reveal()
        {
            return await _service.RevealMyKey();
        }

        /// <summary>
        /// 更新备注名
        /// </summary>
        [HttpPost("UpdateName")]
        [ActionDescription("修改OpenAI兼容接口Key备注")]
        public async Task<bool> UpdateName([FromBody] UpdateOpenAIKeyNameRequest body)
        {
            return await _service.UpdateMyKeyName(body?.Name);
        }

        /// <summary>
        /// 启用 / 禁用当前 Key
        /// </summary>
        [HttpPost("SetEnabled")]
        [ActionDescription("启用或禁用OpenAI兼容接口Key")]
        public async Task<bool> SetEnabled([FromBody] SetOpenAIKeyEnabledRequest body)
        {
            return await _service.SetMyKeyEnabled(body.Enabled);
        }

        /// <summary>
        /// 删除（作废）当前 Key
        /// </summary>
        [HttpDelete("Delete")]
        [ActionDescription("删除OpenAI兼容接口Key")]
        public async Task<bool> Delete()
        {
            return await _service.DeleteMyKey();
        }

        /// <summary>
        /// 获取对接文档：Base URL、可调用的智能体列表、cURL/Python/Node.js 示例
        /// </summary>
        [HttpGet("GetIntegrationDoc")]
        [ActionDescription("获取OpenAI兼容接口对接文档")]
        public async Task<OpenAICompatibleIntegrationDocDto> GetIntegrationDoc()
        {
            // 拼当前请求的公网 URL，便于前端/外部按环境切换
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v1";
            return await _service.GetMyIntegrationDoc(baseUrl);
        }
    }

    public class GenerateOpenAIKeyRequest
    {
        public string? Name { get; set; }
    }

    public class UpdateOpenAIKeyNameRequest
    {
        public string? Name { get; set; }
    }

    public class SetOpenAIKeyEnabledRequest
    {
        public bool Enabled { get; set; }
    }
}