using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Msg;
using kevin.Domain.Share.Dtos.Sign;
using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Enums;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IMessageService : IBaseService
    {
        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="dtoPage"></param>
        /// <returns></returns>
        Task<dtoPageData<MessageDto>> GetPageData(dtoPagePar<MsgGetPageDataParDto> par);

        /// <summary>
        /// 已读
        /// </summary> 
        /// <returns></returns>
        Task<bool> Read(Guid id);

        /// <summary>
        /// 新增编辑
        /// </summary> 
        /// <returns></returns>
        Task<bool> AddEdit(MessageDto message);

        /// <summary>
        /// 删除标记
        /// </summary> 
        /// <returns></returns>  
        Task<bool> Delete(Guid id);

        /// <summary>
        /// 获取我的未读消息数量
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        Task<int> GetMyNoReadCount(MessageType messageType = MessageType.All);
    }
}
