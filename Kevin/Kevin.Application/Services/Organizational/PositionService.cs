using kevin.Application;
using kevin.Application.Services;
using kevin.Domain.Entities;
using kevin.Domain.Entities.Organizational;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IRepositories.Organizational;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Domain.Share.Enums;
using kevin.RepositorieRps.Repositories;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using Web.Global.Exceptions;

namespace kevin.Domain.Interfaces.IServices.Organizational
{
    public class PositionService : BaseService, IPositionService
    {
        public IPositionRp positionRp { get; set; }

        public IUserBindPositionRp userBindPositionRp { get; set; }
        public PositionService(IHttpContextAccessor _httpContextAccessor, IPositionRp _positionRp, IUserBindPositionRp _userBindPositionRp) : base(_httpContextAccessor)
        {
            positionRp = _positionRp;
            this.userBindPositionRp = _userBindPositionRp;
        }

        public async Task<dtoPageData<PositionDto>> GetPageData(dtoPagePar<string> par)
        {
            var dataPage = new dtoPageData<PositionDto>();
            int skip = par.GetSkip();
            var data = positionRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(par.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(par.searchKey) || (t.Code ?? "").Contains(par.searchKey));
            }
            dataPage.total = await data.CountAsync();
            var dbdata = await data.Skip(skip).Take(par.pageSize).OrderByDescending(x => x.Sort).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            dataPage.data = dbdata.MapToList<TPosition, PositionDto>();
            dataPage.data.ForEach(t =>
            {
                t.CreateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.CreateUser?.Name;
                t.UpdateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.UpdateUser?.Name;
            });
            return dataPage;
        }

        public async Task<List<PositionDto>> GetALLList()
        {
            return (await positionRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId).OrderByDescending(x => x.Sort)
                 .ToListAsync()).MapToList<TPosition, PositionDto>();
        }


        public Task<PositionDto> GetPositionTree()
        {
            var dataPage = new PositionDto();
            var allList = positionRp.Query().Where(t => t.IsDelete == false && t.Status == OrganizationalStatus.Active && t.TenantId == CurrentUser.TenantId).ToList().MapToList<TPosition, PositionDto>();
            var data = allList.FirstOrDefault(t => t.ParentId == default);
            if (data != default)
            {
                dataPage = GetChildListData(data.Id, allList, data);
            }
            return Task.FromResult(dataPage);
        }

        /// <summary>
        /// 获取某个岗位下面的所有岗位ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<long> GetChildIdList(long id)
        {
            var position = new PositionDto();
            var allList = positionRp.Query().Where(t => t.IsDelete == false && t.Status == OrganizationalStatus.Active && t.TenantId == CurrentUser.TenantId).ToList().MapToList<TPosition, PositionDto>();
            var data = allList.FirstOrDefault(t => t.Id == id);
            if (data != default)
            {
                position = GetChildListData(data.Id, allList, data);
            }
            return GetChildListIdData(position, new List<long>());
        }

        /// <summary>
        /// 获取某个岗位下面的所有用户ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<long> GetChildUserIds(long id)
        {
            var dataPids = GetChildIdList(id);
            return userBindPositionRp.Query().Where(t => t.IsDelete == false && dataPids.Contains(t.PositionId)).Select(t => t.UserId).ToList(); 
        }

        /// <summary>
        /// 获取子集数据
        /// </summary>
        /// <returns></returns>
        private PositionDto GetChildListData(long id, List<PositionDto> allList, PositionDto reslut)
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

        private List<long> GetChildListIdData(PositionDto positionDto, List<long> ids)
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

        public async Task<bool> AddEdit(PositionDto data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = positionRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TPosition>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                positionRp.Add(add);
            }
            else
            {
                var position = positionRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (position != default)
                {
                    position.UpdateTime = DateTime.Now;
                    position.UpdateUserId = CurrentUser.UserId;
                    position.Name = data.Name;
                    position.Code = data.Code;
                    position.Description = data.Description;
                    position.ParentId = data.ParentId;
                    position.Status = data.Status;
                    position.Sort = data.Sort;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await positionRp.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await positionRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                positionRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }


        public async Task<bool> AddEditUserBindPosition(long userId, List<long> positionIds)
        {
            var deleteRoleData = userBindPositionRp.Query().Where(t => t.UserId == userId && t.IsDelete == false).ToList();
            foreach (var item in deleteRoleData)
            {
                item.IsDelete = true;
                item.DeleteTime = DateTime.Now;
                item.UserId = CurrentUser.UserId;
            }
            await userBindPositionRp.SaveChangesAsync();
            if (positionIds.Count > 0)
            {
                foreach (var id in positionIds)
                {
                    var userBindRole = new TUserBindPosition();
                    userBindRole.Id = SnowflakeIdService.GetNextId();
                    userBindRole.UserId = userId;
                    userBindRole.PositionId = id;
                    userBindRole.IsDelete = false;
                    userBindRole.CreateUserId = CurrentUser.UserId;
                    userBindRole.CreateTime = DateTime.Now;
                    userBindPositionRp.Add(userBindRole);
                }
                await userBindPositionRp.SaveChangesAsync();
            }
            return true;
        }
    }
}
