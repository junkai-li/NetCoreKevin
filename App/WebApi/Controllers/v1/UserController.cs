using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos.User;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using kevin.Share.Dtos.System;
using Kevin.Common.Helper;
using Kevin.Web.Filters.TransactionScope.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Web.Filters;

namespace WebApi.Controllers.v1
{


    /// <summary>
    /// 用户数据操作控制器
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [Authorize]
    [MyArea("系统管理", "System")]
    [ActionDescription("用户管理")]
    [MyModule("用户管理", "User")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private IUserService _userService { get; set; }


        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// 通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [HttpGet("GetUser")]
        [SkipAuthority]
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
        [SkipAuthority]
        public bool EditUserPhoneBySms([FromBody] dtoKeyValue keyValue)
        {
            return _userService.EditUserPhoneBySms(keyValue);
        }

        /// <summary>
        /// 通过短信验证码修改账户密码</summary>
        /// <param name="keyValue">key为新密码，value为短信验证码</param>
        /// <returns></returns>
        [HttpPost("EditUserPassWordBySms")]
        [SkipAuthority]
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
        [HttpLog("用户管理", "获取小程序用户列表信息")]
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
        [HttpLog("用户管理", "获取系统用户列表信息")]
        public dtoPageData<dtoUser> GetSysUserList(dtoPageData<dtoUser> dtoPage)
        {
            return _userService.GetSysUserList(dtoPage);
        }

        /// <summary>
        /// 导出获取系统用户列表信息
        /// </summary> 
        /// <returns></returns>
        [HttpPost("ExportGetSysUserList")]
        [ActionDescription("导出预览值班排期表")]
        [SkipAuthority]
        public IActionResult ExportGetSysUserList([FromBody] dtoPageData<dtoUser> dtoPage)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "用户名", "Name" },
                { "昵称", "NickName" },
                    { "手机号", "Phone" },
                       { "邮箱", "Email" },
                          { "创建时间", "CreateTimeStr" },
                             { "状态", "StatusStr" },
                                { "最近登陆时间", "RecentLoginTimeStr" }
            };
            var data = _userService.GetSysUserList(dtoPage);
            foreach (var item in data.data)
            {
                item.CreateTimeStr = item.CreateTime.Date.ToString("yyyy-MM-dd");
                item.RecentLoginTimeStr = item.RecentLoginTime != default ? item.RecentLoginTime.Value.Date.ToString("yyyy-MM-dd") : "";
                item.StatusStr = item.Status ? "启用" : "禁用";
            }
            return new NPOIHelper().ExportExcelFileStream("值班排期表.xlsx", data.data, keyValuePairs: keyValuePairs);
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
        [Transactional]
        [HttpLog("用户管理", "新增编辑用户信息")]
        public bool EditUser(dtoUser user)
        {
            return _userService.EditUser(user);
        }

        /// <summary>
        /// 更改当前用户密码 
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        [HttpPost("ChangePasswordTokenUser")]
        [Transactional]
        [ActionDescription("更改当前用户密码")]
        public async Task<bool> ChangePasswordTokenUser([FromBody] ChangePasswordTokenUserDto user, CancellationToken cancellationToken)
        {
            return await _userService.ChangePasswordTokenUser(user.OldPwd, user.NewPwd, cancellationToken);
        }

        /// <summary>
        /// 删除用户信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns>
        [HttpDelete("DeleteUser")]
        [Transactional]
        [ActionDescription("删除用户信息")]
        [HttpLog("用户管理", "删除用户信息")]
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
        [SkipAuthority]
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
        [HttpLog("用户管理", "新增编辑用户角色")]
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
        [HttpLog("用户管理", "删除用户角色")]
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
        [SkipAuthority]
        public Guid GetTokenUserId()
        {
            return CurrentUser.UserId;
        }

    }
}