using kevin.CodeGenerator.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.CodeGenerator
{
    public interface ICodeGeneratorService
    {
        /// <summary>
        /// 获取区域名称列表
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetAreaNames();

        /// <summary>
        /// 获取区域名称下面的表列表
        /// </summary>
        /// <returns></returns>
        Task<List<EntityItemDto>> GetAreaNameEntityItems(string areaName);

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="entityItems"></param>
        /// <returns></returns>
        Task<bool> BulidCode(List<EntityItemDto> entityItems);
    }
}
