using Aop.Api.Domain;
using kevin.Domain.Kevin;
using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Interfaces;
using kevin.Permission.Permission;
using kevin.Permission.Permisson;
using kevin.Share.Dtos;
using StackExchange.Redis;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IPermissionService : IService
    {
        /// <summary>
        /// 初始化权限
        /// </summary> 
        /// <returns></returns>
        bool Reload(int tenantId = default);

        /// <summary>
        /// 获取单个 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        PermissionDto GetDetails(string Id);

        Task<dtoPageData<PermissionDto>> GetPageData(dtoPageData<PermissionDto> dtoPage);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Del(string id);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool AddEdit(PermissionDto entity);

        /// <summary>
        /// 获取所有权限列表
        /// </summary> 
        /// <returns></returns>
        List<PermissionDto> GetAllPermissions();

        /// <summary>
        /// 获取所有权限列表Ids
        /// </summary> 
        /// <returns></returns>
        List<string> GetAllPermissionIds();

        /// <summary>
        /// 获取角色对应权限
        /// </summary> 
        /// <returns></returns>
        PermissionEditDto GetAllAreaPermissions(Guid roleId);

        /// <summary>
        /// 编辑角色对应权限
        /// </summary> 
        /// <returns></returns>
        bool EditAllAreaPermissions(PermissionEditDto dto);

        /// <summary>
        /// 获取登录用户权限
        /// </summary>
        List<string> GetUserPermissions();
    }
}
