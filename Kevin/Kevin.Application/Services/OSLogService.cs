using kevin.Share.Dtos;

namespace kevin.Application.Services
{
    public class OSLogService : BaseService, IOSLogService
    {
        public IOSLogRp osLogRp { get; set; }
        public OSLogService(IHttpContextAccessor _httpContextAccessor, IOSLogRp _osLogRp) : base(_httpContextAccessor)
        {
            osLogRp = _osLogRp;
        }
        /// <summary>
        /// 获取变动日志分页数据
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<OSLogDto>> GetPageData(dtoPageData<OSLogDto> dtoPage)
        {
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = osLogRp.Query().Where(t => t.IsDelete == false);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Content ?? "").Contains(dtoPage.searchKey) || (t.Sign ?? "").Contains(dtoPage.searchKey) || (t.Table ?? "").Contains(dtoPage.searchKey) || (t.TableId.ToString() ?? "").Contains(dtoPage.searchKey));
            }
            dtoPage.total = await data.CountAsync();
            var list = (await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPage.pageSize).Include(t => t.ActionUser).ToListAsync());
            dtoPage.data = list.MapToList<TOSLog, OSLogDto>();
            foreach (var item in dtoPage.data)
            {
                var listitem = list.Where(t => t.Id == item.Id).FirstOrDefault();
                if (listitem != default)
                {
                    item.ActionUserName = listitem.ActionUser != null ? listitem.ActionUser.Name : "";
                }
            }
            return dtoPage;
        }
    }
}
