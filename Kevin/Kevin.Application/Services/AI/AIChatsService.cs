using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.RepositorieRps.Repositories.AI;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace kevin.Application.Services.AI
{
    public class AIChatsService : BaseService, IAIChatsService
    {
        public IAIChatsRp aIChatsRp { get; set; }
        public AIChatsService(IHttpContextAccessor _httpContextAccessor, IAIChatsRp _aIChatsRp) : base(_httpContextAccessor)
        {
            this.aIChatsRp = _aIChatsRp;
        }

        /// <summary>
        /// 获取我的ai聊天列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIChatsDto>> GetMyPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIChatsDto>();
            int skip = dtoPage.GetSkip();
            var data = aIChatsRp.Query().Where(t => t.IsDelete == false && t.UserId == CurrentUser.UserId && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey));
            }
            result.total = await data.CountAsync();
            result.data = (await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIChats, AIChatsDto>();
            return result;
        }


        /// <summary>
        /// 新建聊天
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Add(AIChatsDto par)
        {
            var add = par.MapTo<TAIChats>();
            add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.CreateUserId = CurrentUser.UserId;
            add.TenantId = CurrentUser.TenantId;
            add.UserId = CurrentUser.UserId;
            aIChatsRp.Add(add);
            await aIChatsRp.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 删除ai聊天
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIChatsRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIChatsRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }


}
