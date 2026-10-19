using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kevin.Web.Basics.AI
{
    /// <summary>
    /// 用户 Token 额度
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MyArea("AI管理", "AI")]
    [MyModule("用户Token额度", "AIUserTokenQuota")]
    public class AIUserTokenQuotaController : ControllerBase
    {
        private readonly IAITokenQuotaService _service;

        public AIUserTokenQuotaController(IAITokenQuotaService service)
        {
            _service = service;
        }

        /// <summary>
        /// 获取当前登录用户当月 Token 额度（个人信息页展示）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMyQuota")]
        [ActionDescription("获取当前用户当月Token额度")]
        [SkipAuthority]
        public async Task<AIUserTokenQuotaDto> GetMyQuota()
        {
            return await _service.GetMyQuotaAsync();
        }
    }
}
