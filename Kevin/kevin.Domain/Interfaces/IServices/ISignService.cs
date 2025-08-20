using kevin.Domain.Share.Dtos.Sign;

namespace kevin.Domain.Interfaces.IServices
{
    public interface ISignService: IBaseService
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
