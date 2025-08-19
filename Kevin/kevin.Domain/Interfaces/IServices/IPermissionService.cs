using kevin.Domain.Kevin;
using kevin.Domain.Share.Dtos.System;
using kevin.Permission.Permission;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IPermissionService
    {
        /// <summary>
        /// 初始化权限
        /// </summary> 
        /// <returns></returns>
         bool Reload();

        /// <summary>
        /// 获取单个 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        TPermission GetDetails(string Id);


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
        bool Edit(TPermission entity);

        /// <summary>
        /// 获取所有权限列表
        /// </summary> 
        /// <returns></returns>
        List<TPermission> GetAllPermissions();

        /// <summary>
        /// 获取所有权限列表Ids
        /// </summary> 
        /// <returns></returns>
        List<string> GetAllPermissionIds();

        /// <summary>
        /// 获取角色对应权限
        /// </summary> 
        /// <returns></returns>
        List<AreaPermissionDto> GetAllAreaPermissions(Guid roleId);

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
