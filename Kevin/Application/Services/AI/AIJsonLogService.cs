using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AIJsonLog服务接口
    /// </summary>
    public class AIJsonLogService : BaseService, IAIJsonLogService
    {
        public IAIJsonLogRp AIJsonLogRp { get; set; }

        private object? _data { get; set; }
        private long AIChatsId = 0;
        private long AppId = 0;

        private int TenantId = 0;
        public void InitData(object data)
        {
            _data = data;
            if (_data != default)
            {
                var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(_data));
                jsonDoc.RootElement.GetProperty("AIChatsId").TryGetInt64(out AIChatsId);
                jsonDoc.RootElement.GetProperty("AppId").TryGetInt64(out AppId);
                jsonDoc.RootElement.GetProperty("TenantId").TryGetInt32(out TenantId);
            }
        }
        public AIJsonLogService(IHttpContextAccessor _httpContextAccessor, IAIJsonLogRp _AIJsonLogRp) : base(_httpContextAccessor)
        {
            this.AIJsonLogRp = _AIJsonLogRp;
        }

        public async Task<dtoPageData<TAIJsonLog>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAIJsonLog>();
            var data = AIJsonLogRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        [Description("用于保存 Json 数据。")] 
        public async Task<string> Add([Description("传入完整的json结构数据比如 {\"name\":\"kevin\"}")][Required] string Json)
        {
            var add = new TAIJsonLog();
            add.Id = SnowflakeIdService.GetNextId();
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.Json = Json;
            add.AIChatsId = AIChatsId;
            add.AIAppsId = AppId;
            add.TenantId = TenantId;
            AIJsonLogRp.Add(add);
            await AIJsonLogRp.SaveChangesAsync();
            return "保存成功";
        }
        public async Task<bool> AddEdit(TAIJsonLog data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AIJsonLogRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAIJsonLog>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.AIChatsId = AIChatsId;
                add.AIAppsId= AppId;
                AIJsonLogRp.Add(add);
            }
            else
            {
                var upData = AIJsonLogRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    upData = data.MapTo(upData);
                    upData.UpdateTime = DateTime.Now;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            await AIJsonLogRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AIJsonLogRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {

                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AIJsonLogRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
