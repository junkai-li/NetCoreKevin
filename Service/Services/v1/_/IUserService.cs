using Models.Dtos;
using Models.Dtos.System;
using Service.Dtos.v1.User;
using System;
using System.Collections.Generic;
using Web.Base._;

namespace Service.Services.v1._
{
    public interface IUserService: IBaseService
    {
        /// <summary>
        /// 通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        dtoUser GetUser(Guid userId);

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
        string GetWeiXinMiniAppOpenId(Guid weixinkeyid, string code);

        /// <summary>
        /// 获取微信小程序手机号
        /// </summary>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据</param>
        /// <param name="code">微信临时code</param>
        /// <param name="weixinkeyid">微信配置密钥ID</param> 
        string GetWeiXinMiniAppPhone(string iv, string encryptedData, string code, Guid weixinkeyid);

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
        dtoPageData<dtoUser> GetSysUserList(dtoPageData<dtoUser> dtoPage);

        /// <summary>
        /// 后台管理通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns> 
        dtoUser GetSysUserWhereId(Guid Id);

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
        bool DeleteUser(Guid Id);

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
        dtoRole GetRoleWhereId(Guid Id);

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
        bool DeleteUserRole(Guid Id);

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
    }
}
