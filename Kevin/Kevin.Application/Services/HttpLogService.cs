using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IRepositories;
using kevin.RepositorieRps.Repositories;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Application.Services
{
    public class HttpLogService : BaseService, IHttpLogService
    {
        public IHttpLogRp httpLogRp { get; set; }
        public HttpLogService(IHttpContextAccessor _httpContextAccessor, IHttpLogRp _httpLogRp) : base(_httpContextAccessor)
        {
            httpLogRp = _httpLogRp;
        }


        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<HttpLogDto>> GetPageData(dtoPageData<HttpLogDto> dtoPage)
        {
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = httpLogRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.OperateRemark ?? "").Contains(dtoPage.searchKey) || (t.OperateType ?? "").Contains(dtoPage.searchKey) || (t.UserName ?? "").Contains(dtoPage.searchKey));
            }
            dtoPage.total = await data.CountAsync();
            dtoPage.data = (await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<THttpLog, HttpLogDto>(); 
            return dtoPage;
        }


        public async Task<bool> Add(string operateType, string operateRemark)
        {
            return await httpLogRp.Add(operateType, operateRemark);
        }
    }
}
