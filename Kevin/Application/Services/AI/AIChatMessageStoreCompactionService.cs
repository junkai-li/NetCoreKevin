using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos;
using NPOI.SS.Formula.Functions;
using System.ComponentModel;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AIChatMessageStoreCompaction服务接口
    /// </summary>
    public class AIChatMessageStoreCompactionService : BaseService, IAIChatMessageStoreCompactionService
    {

        private string _threadId { get; set; } 
        public void InitData(object data)
        {
            _threadId= (string)data;
        }
        public IAIChatMessageStoreCompactionRp AIChatMessageStoreCompactionRp { get; set; }
        public AIChatMessageStoreCompactionService(IHttpContextAccessor _httpContextAccessor, IAIChatMessageStoreCompactionRp _AIChatMessageStoreCompactionRp) : base(_httpContextAccessor)
        {
            this.AIChatMessageStoreCompactionRp = _AIChatMessageStoreCompactionRp;
        }

        public async Task<dtoPageData<TAIChatMessageStoreCompaction>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAIChatMessageStoreCompaction>();
            var data = AIChatMessageStoreCompactionRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TAIChatMessageStoreCompaction data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AIChatMessageStoreCompactionRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAIChatMessageStoreCompaction>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AIChatMessageStoreCompactionRp.Add(add);
            }
            else
            {
                var upData = AIChatMessageStoreCompactionRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
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
            await AIChatMessageStoreCompactionRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AIChatMessageStoreCompactionRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {

                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AIChatMessageStoreCompactionRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        public async Task<List<TAIChatMessageStoreCompaction>> GetList(string threadId)
        {
            return await AIChatMessageStoreCompactionRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.ThreadId == threadId).ToListAsync();
        }

        /// <summary>
        /// 根据线程id获取提示词
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public async Task<String> GetThreadPrompt(string threadId)
        {
            var prompt = "";
            var data = await AIChatMessageStoreCompactionRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.ThreadId == threadId).OrderBy(t => t.CreateTime).ToListAsync();
            if (data != default && data.Count > 0)
            {
                prompt = " 历史对话（压缩提取摘要版本）：";
                for (int i = 1; i <= data.Count; i++)
                {
                    prompt += "\n " + i + $".时间{data[i-1].CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}：内容如下：" + data[i - 1].CompactionResultMessageText;
                }
            } 
            return prompt;
        }

        /// <summary>
        /// 获取聊天对话历史记录，当用户询问聊天记录时调用，返回用户历史对话
        /// </summary> 
        [Description("获取聊天对话历史记录，当用户询问聊天记录时调用，返回用户历史对话(压缩摘要版本)")]
        public async Task<String> GetAIToolThreadPrompt()
        {
            var prompt = "";
            var data = await AIChatMessageStoreCompactionRp.Query(isDataPer: false, isTenant: false).Where(t => t.IsDelete == false && t.ThreadId == _threadId).OrderBy(t => t.CreateTime).ToListAsync();
            if (data != default && data.Count > 0)
            {
                prompt = " 历史对话（压缩摘要版本）：";
                for (int i = 1; i <= data.Count; i++)
                {
                    prompt += "\n " + i + $".时间{data[i - 1].CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}：内容如下：" + data[i - 1].CompactionResultMessageText;
                }
            }
            return prompt;
        }
    }
}
