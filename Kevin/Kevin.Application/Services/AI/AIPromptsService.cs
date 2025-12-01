using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IRepositories.Organizational;
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
    public class AIPromptsService : BaseService, IAIPromptsService
    {
        public IAIPromptsRp aIPromptsRp { get; set; } 
        public AIPromptsService(IHttpContextAccessor _httpContextAccessor, IAIPromptsRp _aIPromptsRp) : base(_httpContextAccessor)
        {
            this.aIPromptsRp = _aIPromptsRp; 
        }

        /// <summary>
        /// 获取ai提示词列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIPromptsDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIPromptsDto>();
            int skip = dtoPage.GetSkip();
            var data = aIPromptsRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey));
            }
            result.total = await data.CountAsync();
            result.data = (await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIPrompts, AIPromptsDto>();
            return result;
        }

        /// <summary>
        /// 编辑或添加ai提示词
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> AddEdit(AIPromptsDto par)
        {
            var isAdd = par.Id == default;
            if (!isAdd)
            {
                var msg = aIPromptsRp.Query().Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = par.MapTo<TAIPrompts>();
                add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                aIPromptsRp.Add(add);
            }
            else
            {
                var msg = aIPromptsRp.Query().Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg != default)
                {
                    msg = par.MapTo<TAIPrompts>();
                    msg.UpdateTime = DateTime.Now;
                    msg.UpdateUserId = CurrentUser.UserId;
                    msg.TenantId = CurrentUser.TenantId;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await aIPromptsRp.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 删除ai提示词
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIPromptsRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIPromptsRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
