using kevin.Application;
using kevin.Domain.Entities.Organizational;
using kevin.Domain.Interfaces.IRepositories.Organizational;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Domain.Share.Enums;
using kevin.RepositorieRps.Repositories;
using kevin.Share.Dtos;
using System;
using Web.Global.Exceptions;

namespace kevin.Domain.Interfaces.IServices.Organizational
{
    public class DepartmentService : BaseService, IDepartmentService
    {
        public IDepartmentRp departmentRp { get; set; }

        public IUserInfoRp userInfoRp { get; set; }
        public DepartmentService(IHttpContextAccessor _httpContextAccessor, IDepartmentRp _departmentRp, IUserInfoRp _userInfoRp) : base(_httpContextAccessor)
        {
            departmentRp = _departmentRp;
            this.userInfoRp = _userInfoRp;
        }

        public async Task<dtoPageData<DepartmentDto>> GetPageData(dtoPagePar<string> par)
        {
            var dataPage = new dtoPageData<DepartmentDto>();
            int skip = par.GetSkip();
            var data = departmentRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(par.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(par.searchKey) || (t.Code ?? "").Contains(par.searchKey));
            }
            dataPage.total = await data.CountAsync();
            var dbdata = await data.Skip(skip).Take(par.pageSize).OrderByDescending(x => x.Sort).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            dataPage.data = dbdata.MapToList<TDepartment, DepartmentDto>();
            dataPage.data.ForEach(t =>
            {
                t.CreateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.CreateUser?.Name;
                t.UpdateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.UpdateUser?.Name;
            });
            return dataPage;
        }

        public async Task<List<DepartmentDto>> GetALLList()
        {
            return (await departmentRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId).OrderByDescending(x => x.Sort)
                 .ToListAsync()).MapToList<TDepartment, DepartmentDto>();
        }

        public async Task<List<DepartmentDto>> GetALLList(List<long> Ids)
        {
            return (await departmentRp.Query().Where(t => t.IsDelete == false && Ids.Contains(t.Id) && t.TenantId == CurrentUser.TenantId).OrderByDescending(x => x.Sort)
                 .ToListAsync()).MapToList<TDepartment, DepartmentDto>();
        }


        public Task<DepartmentDto> GetDepartmentTree()
        {
            var dataPage = new DepartmentDto();
            var allList = departmentRp.Query().Where(t => t.IsDelete == false && t.Status == OrganizationalStatus.Active && t.TenantId == CurrentUser.TenantId).ToList().MapToList<TDepartment, DepartmentDto>();
            var data = allList.FirstOrDefault(t => t.ParentId == default);
            if (data != default)
            {
                dataPage = GetChildListData(data.Id, allList, data);
            }
            return Task.FromResult(dataPage);
        }

        /// <summary>
        /// 获取某个部门下面的所有部门ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<long> GetChildIdList(long id)
        {
            var position = new DepartmentDto();
            var allList = departmentRp.Query().Where(t => t.IsDelete == false && t.Status == OrganizationalStatus.Active && t.TenantId == CurrentUser.TenantId).ToList().MapToList<TDepartment, DepartmentDto>();
            var data = allList.FirstOrDefault(t => t.Id == id);
            if (data != default)
            {
                position = GetChildListData(data.Id, allList, data);
            }
            return GetChildListIdData(position, new List<long>());
        }

        /// <summary>
        /// 获取某个部门下面的所有用户ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<long> GetDepartmentChildUserIds(long id)
        {
            var dataPids = GetChildIdList(id);
            return userInfoRp.Query(true, false).Where(t => t.IsDelete == false && dataPids.Contains(t.DepartmentId.Value)).Select(t => t.UserId).ToList();
        }

        /// <summary>
        /// 获取某个部门的用户ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<long>> GetDepartmentUserIds(long id)
        {
            return await userInfoRp.Query(true, false).Where(t => t.IsDelete == false && t.DepartmentId.Value == id).Select(t => t.UserId).ToListAsync();
        }

        /// <summary>
        /// 获取子集数据
        /// </summary>
        /// <returns></returns>
        private DepartmentDto GetChildListData(long id, List<DepartmentDto> allList, DepartmentDto reslut)
        {
            //获取上级是我的数据
            var data = allList.Where(t => id == t.ParentId).ToList();
            if (data.Count > 0)
            {
                reslut.Children = data.OrderByDescending(t => t.Sort).ToList();
                foreach (var item in reslut.Children)
                {
                    GetChildListData(item.Id, allList, item);
                }

            }
            return reslut;
        }

        private List<long> GetChildListIdData(DepartmentDto positionDto, List<long> ids)
        {
            ids.Add(positionDto.Id);
            if (positionDto.Children.Count > 0)
            {
                foreach (var item in positionDto.Children)
                {
                    ids.AddRange(GetChildListIdData(item, ids));
                }
            }
            return ids;
        }
        public async Task<bool> AddEdit(DepartmentDto data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = departmentRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TDepartment>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                departmentRp.Add(add);
            }
            else
            {
                var department = departmentRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (department != default)
                {
                    department.UpdateTime = DateTime.Now;
                    department.UpdateUserId = CurrentUser.UserId;
                    department.Name = data.Name;
                    department.Code = data.Code;
                    department.Description = data.Description;
                    department.ParentId = data.ParentId;
                    department.Status = data.Status;
                    department.Description = data.Description;
                    department.ManagerUserId = data.ManagerUserId;
                    department.Sort = data.Sort;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await departmentRp.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await departmentRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                //查询是否还有绑定岗位的用户
                var userIds = GetDepartmentChildUserIds(id);
                if (userIds.Count > 0)
                {
                    throw new UserFriendlyException($"此部门下有{userIds.Count}位用户，请先解绑{userIds.Count}位用户");
                }
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                departmentRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
