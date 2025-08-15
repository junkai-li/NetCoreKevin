using Service.Dtos.v1.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.v1._
{
    public interface ISignService
    {
        /// <summary>
        /// 获取标记总数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableId"></param>
        /// <param name="sign"></param>
        /// <returns></returns> 
        int GetSignCount(string table, Guid tableId, string sign);

        /// <summary>
        /// 新增标记
        /// </summary>
        /// <param name="addSign"></param>
        /// <returns></returns> 
        bool AddSign(dtoSign addSign);

        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="deleteSign"></param>
        /// <returns></returns> 

        bool DeleteSign(dtoSign deleteSign);
    }
}
