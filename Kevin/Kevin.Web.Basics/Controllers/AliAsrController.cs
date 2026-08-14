using Kevin.Asr;
using Kevin.Asr.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Web.Basics.Controllers
{
    /// <summary>
    /// Asr语音识别
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AliAsrController : ControllerBase
    {
        private readonly IAsrService _tokenService;

        public AliAsrController(IAsrService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// 获取阿里云 NLS 实时语音识别临时 Token
        /// </summary>
        [HttpPost("GetToken")]
        public async Task<AsrTokenResultDto> GetToken()
        {
            return await _tokenService.GenerateTokenAsync();

        }
    }
}
