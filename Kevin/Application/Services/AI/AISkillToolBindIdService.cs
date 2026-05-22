using DocumentFormat.OpenXml.Office2010.Excel;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AISkillToolBindId服务接口
    /// </summary>
    public class AISkillToolBindIdService : BaseService, IAISkillToolBindIdService
    {
        public IAISkillToolBindIdRp AISkillToolBindIdRp { get; set; }
        public AISkillToolBindIdService(IHttpContextAccessor _httpContextAccessor, IAISkillToolBindIdRp _AISkillToolBindIdRp) : base(_httpContextAccessor)
        {
            this.AISkillToolBindIdRp = _AISkillToolBindIdRp;
        }

        public async Task<dtoPageData<TAISkillToolBindId>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAISkillToolBindId>();
            var data = AISkillToolBindIdRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TAISkillToolBindId data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AISkillToolBindIdRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAISkillToolBindId>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AISkillToolBindIdRp.Add(add);
            }
            else
            {
                var upData = AISkillToolBindIdRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    upData = data.MapTo(upData);
                    upData.UpdateTime = DateTime.Now;
                    upData.UpdateUserId = CurrentUser.UserId;
                    upData.TenantId = CurrentUser.TenantId;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            await AISkillToolBindIdRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AISkillToolBindIdRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {

                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AISkillToolBindIdRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        public async Task<List<TAISkillToolBindId>> GetListById(string Id)
        {
            return await AISkillToolBindIdRp.Query().Where(t => t.IsDelete == false && t.BindId == Id).ToListAsync();
        }

        public async Task<bool> BatchAddIds(string Id, List<long> ids)
        {
            var data = await AISkillToolBindIdRp.Query().Where(t => t.IsDelete == false && t.BindId == Id).ToListAsync();
            var addItems = new List<TAISkillToolBindId>();
            foreach (var item in data)
            {
                if (!ids.Contains(item.AISkillToolManagementId))
                {
                    item.IsDelete = true;
                    item.DeleteTime = DateTime.Now;
                    item.DeleteUserId = CurrentUser.UserId;
                }
                else
                {
                    ids.Remove(item.AISkillToolManagementId);
                }
            }
            foreach (var item in ids)
            {
                addItems.Add(new TAISkillToolBindId
                {
                    Id = SnowflakeIdService.GetNextId(),
                    AISkillToolManagementId = item,
                    BindId = Id,
                    IsDelete = false,
                    CreateTime = DateTime.Now,
                    CreateUserId = CurrentUser.UserId,
                    TenantId = CurrentUser.TenantId
                });
            }
            if (addItems.Count > 0)
            {
                AISkillToolBindIdRp.AddRange(addItems);
            }
            await AISkillToolBindIdRp.SaveChangesAsync();
            return true;
        }
    }
}
