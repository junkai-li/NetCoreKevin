using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using Microsoft.Extensions.DependencyInjection;
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

        public IAIShareInfoService aIShareInfoService { get; set; } 

        /// <summary>
        /// 独立作用域工厂：Add 被注册为智能体可调用的工具，在流式回答中途随时可能写入。
        /// 与请求作用域的 DbContext 共用时，一次日志写入失败会让本请求后续所有 SaveChanges
        /// （含本轮回复落库）跟着失败；这里改用独立作用域的仓储写入。
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        public AIJsonLogService(IHttpContextAccessor _httpContextAccessor, IAIJsonLogRp _AIJsonLogRp, IAIShareInfoService aIShareInfoService, IServiceScopeFactory scopeFactory) : base(_httpContextAccessor)
        {
            this.AIJsonLogRp = _AIJsonLogRp;
            this.aIShareInfoService = aIShareInfoService;
            _scopeFactory = scopeFactory;
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
            add.AIChatsId = aIShareInfoService.GetData().AIChatsId;
            add.AIAppsId = aIShareInfoService.GetData().AIAppsId;
            add.TenantId = aIShareInfoService.GetData().TenantId;
            // 独立作用域落库（见 _scopeFactory 字段注释）：失败只影响本次工具调用
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var writeRp = scope.ServiceProvider.GetRequiredService<IAIJsonLogRp>();
                writeRp.Add(add);
                await writeRp.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIJsonLog] 保存 Json 日志失败（不影响对话）：{ex.Message}");
                return "保存失败：数据库写入异常，请稍后重试";
            }
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
                add.AIChatsId = aIShareInfoService.GetData().AIChatsId;
                add.AIAppsId= aIShareInfoService.GetData().AIAppsId;
                add.TenantId = aIShareInfoService.GetData().TenantId;
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
