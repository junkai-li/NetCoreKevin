using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.SnowflakeId.Service
{
    public interface ISnowflakeIdService
    {
        /// <summary>
        /// 获取id
        /// </summary>
        /// <returns></returns>
        public long GetNextId(); 
    }
}
