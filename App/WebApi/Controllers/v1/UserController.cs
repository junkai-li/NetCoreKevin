using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using Service.Dtos.v1.User;
using Service.Services.v1._;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Filters;
using WebApi.Controllers.Bases;

namespace WebApi.Controllers.v1
{


    /// <summary>
    /// 用户数据操作控制器
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [Authorize] 
    [SkipAuthority]
    public class UserController : ApiControllerBase
    {
        private IUserService _userService { get; set; }


        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// 获取微信小程序OpenId
        /// </summary>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        /// <param name="code">微信临时code</param>
        /// <returns>openid,userid</returns>
        /// <remarks>传入租户ID和微信临时 code 获取 openid，如果 openid 在系统有中对应用户，则一并返回用户的ID值，否则用户ID值为空</remarks>
        [HttpGet("GetWeiXinMiniAppOpenId")]
        public string GetWeiXinMiniAppOpenId(Guid weixinkeyid, string code)
        {
            return _userService.GetWeiXinMiniAppOpenId(weixinkeyid, code);
        }

        /// <summary>
        /// 获取微信小程序手机号
        /// </summary>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据</param>
        /// <param name="code">微信临时code</param>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        [HttpGet("GetWeiXinMiniAppPhone")]
        public string GetWeiXinMiniAppPhone(string iv, string encryptedData, string code, Guid weixinkeyid)
        {


            return _userService.GetWeiXinMiniAppPhone(iv, encryptedData, code, weixinkeyid);
        }

        /// <summary>
        /// 通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [HttpGet("GetUser")]
        [CacheDataFilter(TTL = 60, UseToken = true)]
        public dtoUser GetUser(Guid userId)
        {
            return _userService.GetUser(userId);
        }

        /// <summary>
        /// 通过短信验证码修改账户手机号
        /// </summary>
        /// <param name="keyValue">key 为新手机号，value 为短信验证码</param>
        /// <returns></returns>
        [HttpPost("EditUserPhoneBySms")]
        public bool EditUserPhoneBySms([FromBody] dtoKeyValue keyValue)
        {
            return _userService.EditUserPhoneBySms(keyValue);
        }

        /// <summary>
        /// 通过短信验证码修改账户密码</summary>
        /// <param name="keyValue">key为新密码，value为短信验证码</param>
        /// <returns></returns>
        [HttpPost("EditUserPassWordBySms")]
        public bool EditUserPassWordBySms([FromBody] dtoKeyValue keyValue)
        {

            return _userService.EditUserPassWordBySms(keyValue);

        }

        /// <summary>
        /// 获取小程序用户列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns>
        [HttpPost("GetUserList")]
        [ActionDescription("获取小程序用户列表信息")]
        public dtoPageData<dtoUser> GetUserList(dtoPageData<dtoUser> dtoPage)
        {

            return _userService.GetUserList(dtoPage);
        }

        /// <summary>
        /// 获取系统用户列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns>
        [HttpPost("GetSysUserList")]
        [ActionDescription("获取系统用户列表信息")]
        public dtoPageData<dtoUser> GetSysUserList(dtoPageData<dtoUser> dtoPage)
        {
            return _userService.GetSysUserList(dtoPage);
        }

        /// <summary>
        /// 后台管理通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns>
        [HttpGet("GetSysUserWhereId")]
        [SkipAuthority]
        public dtoUser GetSysUserWhereId([Required] Guid Id)
        {
            return _userService.GetSysUserWhereId(Id);
        }

        /// <summary>
        /// 新增编辑用户信息 
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        [HttpPost("EditUser")]
        [ActionDescription("新增编辑用户信息")]
        public bool EditUser(dtoUser user)
        {
            return _userService.EditUser(user);
        }

        /// <summary>
        /// 删除用户信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns>
        [HttpDelete("DeleteUser")]
        [ActionDescription("删除用户信息")]
        public bool DeleteUser(Guid Id)
        {
            return _userService.DeleteUser(Id);
        }

        /// <summary>
        /// 获取用户角色列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns>
        [HttpPost("GetUserRoleList")]
        [ActionDescription("获取用户角色列表信息")]
        public dtoPageData<dtoRole> GetUserRoleList(dtoPageData<dtoRole> dtoPage)
        {
            return _userService.GetUserRoleList(dtoPage);
        }

        /// <summary>
        /// 通过id获取角色信息 
        /// </summary>
        /// <param name="Id">ID</param>
        /// <returns></returns>
        [HttpGet("GetRoleWhereId")]
        [SkipAuthority]
        public dtoRole GetRoleWhereId([Required] Guid Id)
        {
            return _userService.GetRoleWhereId(Id);
        }

        /// <summary>
        /// 新增编辑用户角色信息 
        /// </summary>
        /// <param name="role">user</param>
        /// <returns></returns>
        [HttpPost("EditUserRole")]
        [ActionDescription("新增编辑用户角色")]
        public bool EditUserRole(dtoRole role)
        {
            return _userService.EditUserRole(role);
        }

        /// <summary>
        /// 删除用户角色信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns>
        [HttpDelete("DeleteUserRole")]
        [ActionDescription("删除用户角色")]
        public bool DeleteUserRole(Guid Id)
        {
            return _userService.DeleteUserRole(Id);
        }

        /// <summary>
        /// 获取可用户角色的键值对列表信息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetUserRoleKey")]
        [SkipAuthority]
        public List<dtoKeyValue> GetUserRoleKey()
        {
            return _userService.GetUserRoleKey();
        }

        /// <summary>
        /// 获取可用系统用户
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetUserSystemKey")]
        [SkipAuthority]
        public List<dtoKeyValue> GetUserSystemKey()
        {
            return _userService.GetUserSystemKey();
        }

        /// <summary>
        /// GetTokenUserId
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetTokenUserId")] 
        public Guid GetTokenUserId()
        {
            return CurrentUser.UserId;
        }

    }
}