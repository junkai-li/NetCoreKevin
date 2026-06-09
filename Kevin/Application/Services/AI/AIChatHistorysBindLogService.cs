using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AIChatHistorysBindLog服务接口
    /// </summary>
    public class AIChatHistorysBindLogService : BaseService, IAIChatHistorysBindLogService
    {
        public IAIChatHistorysBindLogRp AIChatHistorysBindLogRp { get; set; }
        public AIChatHistorysBindLogService(IHttpContextAccessor _httpContextAccessor, IAIChatHistorysBindLogRp _AIChatHistorysBindLogRp) : base(_httpContextAccessor)
        {
            this.AIChatHistorysBindLogRp = _AIChatHistorysBindLogRp;
        }

        public async Task<dtoPageData<TAIChatHistorysBindLog>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAIChatHistorysBindLog>();
            var data = AIChatHistorysBindLogRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TAIChatHistorysBindLog data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AIChatHistorysBindLogRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAIChatHistorysBindLog>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AIChatHistorysBindLogRp.Add(add);
            }
            else
            {
                var upData = AIChatHistorysBindLogRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    upData = data.MapTo(upData);
                    upData.TenantId = CurrentUser.TenantId;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            await AIChatHistorysBindLogRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AIChatHistorysBindLogRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {

                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AIChatHistorysBindLogRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        public async Task<List<AIChatHistorysBindLogDto>> GetByIds(List<long> aIChatHistorysIds, CancellationToken cancellationToken = default)
        {
            if (aIChatHistorysIds == null || aIChatHistorysIds.Count == 0)
            {
                return new List<AIChatHistorysBindLogDto>();
            }
            return await AIChatHistorysBindLogRp.Query(isDataPer: true).Where(t => t.IsDelete == false && aIChatHistorysIds.Contains(t.AIChatHistorysId)).Select(t => new AIChatHistorysBindLogDto
            {
                Id = t.Id,
                AIChatHistorysId = t.AIChatHistorysId,
                LogContent = t.LogContent,
                LogType = (int)t.LogType
            }).ToListAsync(cancellationToken);
        }
    }
}
