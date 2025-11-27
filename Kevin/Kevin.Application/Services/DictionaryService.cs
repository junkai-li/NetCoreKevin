using kevin.Domain.Share.Dtos;
using kevin.Share.Dtos;
using Web.Global.Exceptions;

namespace kevin.Application.Services
{
    public class DictionaryService : BaseService, IDictionaryService
    {
        public IDictionaryRp dictionaryRp { get; set; }
        public DictionaryService(IHttpContextAccessor _httpContextAccessor, IDictionaryRp _dictionaryRp) : base(_httpContextAccessor)
        {
            dictionaryRp = _dictionaryRp;
        }

        /// <summary>
        /// 获取类型列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<string?>> GetTypeList()
        {
            return await dictionaryRp.Query().Where(t => t.IsDelete == false).Select(t => t.Type).Distinct().ToListAsync();
        }

        /// <summary>
        /// 获取指定类型列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<DictionaryDto>> GetTypeWhereList(string type)
        {
            return (await dictionaryRp.Query().Where(t => t.IsDelete == false && t.Type == type).ToListAsync()).OrderByDescending(t => t.Sort).MapToList<TDictionary, DictionaryDto>();
        }

        /// <summary>
        /// 获取变动日志分页数据
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<DictionaryDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            int skip = dtoPage.GetSkip();
            var result = new dtoPageData<DictionaryDto>();
            var data = dictionaryRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Key ?? "").Contains(dtoPage.searchKey) || (t.Value ?? "").Contains(dtoPage.searchKey) || (t.Remarks ?? "").Contains(dtoPage.searchKey));
            }
            if (!string.IsNullOrEmpty(dtoPage.Parameter))
            {
                data = data.Where(t => t.Type == dtoPage.Parameter);
            }
            result.total = await data.CountAsync();
            var list = (await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.Sort).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync());
            result.data = list.MapToList<TDictionary, DictionaryDto>();
            foreach (var item in result.data)
            {
                var listitem = list.Where(t => t.Id == item.Id).FirstOrDefault();
                if (listitem != default)
                {
                    item.CreateUserName = listitem.CreateUser != null ? listitem.CreateUser.Name : "";
                    item.UpdateUserName = listitem.UpdateUser != null ? listitem.UpdateUser.Name : "";
                }
            }
            return result;
        }

        /// <summary>
        /// 新增或编辑
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> AddEdit(DictionaryDto dic)
        {
            var isAdd = dic.Id == default;
            if (!isAdd)
            {
                var msg = dictionaryRp.Query().Where(t => t.IsDelete == false && t.Id == dic.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = dic.MapTo<TDictionary>();
                add.Id = dic.Id == default ? SnowflakeIdService.GetNextId() : dic.Id;
                add.IsDelete = false;
                add.IsSystem = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                dictionaryRp.Add(add);
            }
            else
            {
                var msg = dictionaryRp.Query().Where(t => t.IsDelete == false && t.Id == dic.Id).FirstOrDefault();
                if (msg != default)
                {
                    msg.UpdateTime = DateTime.Now;
                    msg.UpdateUserId = CurrentUser.UserId;
                    msg.TenantId = CurrentUser.TenantId;
                    msg.Key = dic.Key;
                    msg.Value = dic.Value;
                    msg.Remarks = dic.Remarks;
                    msg.Type = dic.Type;
                    msg.Sort = dic.Sort;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await dictionaryRp.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await dictionaryRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                if (like.IsSystem)
                {
                    throw new UserFriendlyException("系统内置配置不能删除");
                }
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                dictionaryRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

    }
}
