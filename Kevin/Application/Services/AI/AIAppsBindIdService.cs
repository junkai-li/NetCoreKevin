using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using Kevin.RepositorieRps.Repositories.AI;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AIAppsBindId服务接口
    /// </summary>
    public class AIAppsBindIdService : BaseService, IAIAppsBindIdService
    {
        public IAIAppsBindIdRp AIAppsBindIdRp { get; set; }
        public AIAppsBindIdService(IHttpContextAccessor _httpContextAccessor, IAIAppsBindIdRp _AIAppsBindIdRp) : base(_httpContextAccessor)
        {
            this.AIAppsBindIdRp = _AIAppsBindIdRp;
        }

        public async Task<List<TAIAppsBindId>> GetListByBindId(string aIAppsId)
        {
            return await AIAppsBindIdRp.Query().Where(t => t.IsDelete == false && t.TAIAppsId == aIAppsId.ToTryInt64()).ToListAsync();
        }

        public async Task<List<TAIAppsBindId>> GetListById(List<string> bindIds)
        {
            return await AIAppsBindIdRp.Query().Where(t => t.IsDelete == false && bindIds.Contains(t.BindId)).ToListAsync();
        }

        public async Task<bool> BatchAddIds(string Id, List<string> ids)
        {
            var data = await AIAppsBindIdRp.Query().Where(t => t.IsDelete == false && t.TAIAppsId == Id.ToTryInt64()).ToListAsync();
            var addItems = new List<TAIAppsBindId>();
            foreach (var item in data)
            {
                if (!ids.Contains(item.BindId))
                {
                    item.IsDelete = true;
                    item.DeleteTime = DateTime.Now;
                    item.DeleteUserId = CurrentUser.UserId;
                }
                else
                {
                    ids.Remove(item.BindId);
                }
            }
            foreach (var item in ids)
            {
                addItems.Add(new TAIAppsBindId
                {
                    Id = SnowflakeIdService.GetNextId(),
                    TAIAppsId = Id.ToTryInt64(),
                    BindId = item,
                    IsDelete = false,
                    CreateTime = DateTime.Now,
                    CreateUserId = CurrentUser.UserId,
                    TenantId = CurrentUser.TenantId
                });
            }
            if (addItems.Count > 0)
            {
                AIAppsBindIdRp.AddRange(addItems);
            }
            await AIAppsBindIdRp.SaveChangesAsync();
            return true;
        }
    }
    }
