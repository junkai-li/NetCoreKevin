using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.User;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IUserService : IBaseService
    {
        /// <summary>
        /// 通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        dtoUser GetUser(long userId);

        /// <summary>
        /// 登录用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <param name="tenantId">租户id</param>
        /// <returns></returns>
        dtoUser LoginUser(string name, string pwd, Int32 tenantId, string passwordHash);

        /// <summary>
        /// 修改当前用户密码
        /// </summary>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <param name="tenantId">租户id</param>
        /// <returns></returns>
        Task<bool> ChangePasswordTokenUser(string oldPwd, string newPwd, CancellationToken cancellationToken);

        /// <summary>
        /// 通过短信验证码修改账户手机号
        /// </summary>
        /// <param name="keyValue">key 为新手机号，value 为短信验证码</param>
        /// <returns></returns>
        bool EditUserPhoneBySms(dtoKeyValue keyValue);

        /// <summary>
        /// 获取微信小程序OpenId
        /// </summary>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        /// <param name="code">微信临时code</param>
        /// <returns>openid,userid</returns>
        /// <remarks>传入租户ID和微信临时 code 获取 openid，如果 openid 在系统有中对应用户，则一并返回用户的ID值，否则用户ID值为空</remarks>
        string GetWeiXinMiniAppOpenId(long weixinkeyid, string code);


        /// <summary>
        /// 通过短信验证码修改账户密码</summary>
        /// <param name="keyValue">key为新密码，value为短信验证码</param>
        /// <returns></returns> 
        bool EditUserPassWordBySms(dtoKeyValue keyValue);

        /// <summary>
        /// 获取小程序用户列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        dtoPageData<dtoUser> GetUserList(dtoPageData<dtoUser> dtoPage);

        /// <summary>
        /// 获取系统用户列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        Task<dtoPageData<dtoUser>> GetSysUserList(dtoPagePar<dtoUserPar> dtoPage);

        /// <summary>
        /// 后台管理通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns> 
        dtoUser GetSysUserWhereId(long Id);

        /// <summary>
        /// 新增编辑用户信息 
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns> 
        bool EditUser(dtoUser user);

        /// <summary>
        /// 删除用户信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns> 
        bool DeleteUser(long Id);

        /// <summary>
        /// 获取用户角色列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        dtoPageData<dtoRole> GetUserRoleList(dtoPageData<dtoRole> dtoPage);

        /// <summary>
        /// 通过id获取角色信息 
        /// </summary>
        /// <param name="Id">ID</param>
        /// <returns></returns> 
        dtoRole GetRoleWhereId(long Id);

        /// <summary>
        /// 新增编辑用户角色信息 
        /// </summary>
        /// <param name="role">user</param>
        /// <returns></returns> 
        bool EditUserRole(dtoRole role);

        /// <summary>
        /// 删除用户角色信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns> 
        bool DeleteUserRole(long Id);

        /// <summary>
        /// 获取可用户角色的键值对列表信息
        /// </summary> 
        /// <returns></returns> 
        List<dtoKeyValue> GetUserRoleKey();

        /// <summary>
        /// 获取可用系统用户
        /// </summary> 
        /// <returns></returns> 

        List<dtoKeyValue> GetUserSystemKey();

        /// <summary>
        /// 最近登录时间
        /// </summary>
        /// <param name="long"></param>
        /// <returns></returns>
        Task<bool> UpdateRecentLoginTime(long id);

        /// <summary>
        /// 获取系统用户数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<int> GetAllUserCount();

        /// <summary>
        /// 获取用户模块数据权限对应的用户ID列表
        /// </summary>
        /// <param name="PermissionsKey">权限Key--去除后缀 只需要区域和模块</param>
        /// <returns></returns>
        public Task<List<long>> GetModuleDataPermissionsUserIds(string PermissionsKey);

        /// <summary>
        /// 获取用户其他信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        dtoUserInfo GetUserInfo(long UserId);
    }
}
