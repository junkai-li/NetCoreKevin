using Kevin.Asr.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Asr
{
    public interface IAsrService
    {
        Task<AsrTokenResultDto> GenerateTokenAsync();
    }
}
