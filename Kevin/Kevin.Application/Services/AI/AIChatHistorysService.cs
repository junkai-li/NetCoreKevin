using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace kevin.Application.Services.AI
{

    public class AIChatHistorysService : BaseService, IAIChatHistorysService
    {
        public IAIChatHistorysRp aIChatHistorysRp { get; set; }
        public AIChatHistorysService(IHttpContextAccessor _httpContextAccessor, IAIChatHistorysRp _aIChatHistorysRp) : base(_httpContextAccessor)
        {
            this.aIChatHistorysRp = _aIChatHistorysRp;
        }

        /// <summary>
        /// 获取我的ai聊天列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIChatHistorysDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIChatHistorysDto>();
            int skip = dtoPage.GetSkip();
            var data = aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (dtoPage.whereId > 0)
            {
                data = data.Where(t => t.AIChatsId == dtoPage.whereId);
            }
            else
            {
                throw new UserFriendlyException("必须传入聊天Id");
            }
            result.total = await data.CountAsync();
            result.data = (await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPage.pageSize).ToListAsync()).MapToList<TAIChatHistorys, AIChatHistorysDto>();
            return result;
        }


        /// <summary>
        /// 新建聊天
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AIChatHistorysDto> Add(AIChatHistorysDto par)
        {
            var add = par.MapTo<TAIChatHistorys>();
            add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.CreateUserId = CurrentUser.UserId;
            add.TenantId = CurrentUser.TenantId;
            add.IsSend = true;
            aIChatHistorysRp.Add(add);
            await aIChatHistorysRp.SaveChangesAsync();
            //模拟AI回复消息
            var addAi = new TAIChatHistorys();
            addAi.Id = SnowflakeIdService.GetNextId();
            addAi.IsDelete = false;
            addAi.CreateTime = DateTime.Now;
            addAi.CreateUserId = CurrentUser.UserId;
            addAi.TenantId = CurrentUser.TenantId;
            addAi.IsSend = false;
            addAi.AIChatsId = par.AIChatsId;
            addAi.Content = $"关于{par.Content}，这是一个很好的问题。我可以为您提供相关信息...";
            aIChatHistorysRp.Add(addAi);
            await aIChatHistorysRp.SaveChangesAsync();
            return addAi.MapTo<AIChatHistorysDto>();
        }

        /// <summary>
        /// 删除聊天记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIChatHistorysRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
