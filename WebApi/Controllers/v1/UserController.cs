using AdminApi.Models;
using Common.AliYun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Dtos.System;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Web.Filters;
using Web.Permisson.Attributes;
using WebApi.Controllers.Bases;
using WebApi.Models.v1.User;

namespace WebApi.Controllers.v1
{


    /// <summary>
    /// 用户数据操作控制器
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]  
    [Authorize]
    [ApiController]
    [SkipAuthority]
    public class UserController : PubilcControllerBase
    {   
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


            var weixinkey = db.TWeiXinKey.Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);

            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;

            return openid;


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


            var weixinkey = db.TWeiXinKey.Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);


            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;
            string sessionkey = wxinfo.sessionkey;


            var strJson = Web.Libraries.WeiXin.MiniApp.WeiXinHelper.DecryptionData(encryptedData, sessionkey, iv);

            var user = db.TUserBindWeixin.Where(t => t.WeiXinOpenId == openid & t.WeiXinKeyId == weixinkeyid).Select(t => t.User).FirstOrDefault();

            user.Phone = Common.Json.JsonHelper.GetValueByKey(strJson, "phoneNumber");

            db.SaveChanges();

            return user.Phone;
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

            if (userId == default)
            {
                userId =CurrentUser.UserId;
            }

            var user = db.TUser.Where(t => t.Id == userId && t.IsDelete == false).Select(t => new dtoUser
            {
                Name = t.Name,
                NickName = t.NickName,
                Phone = t.Phone,
                Email = t.Email,
                Role = t.Role.Name,
                CreateTime = t.CreateTime
            }).FirstOrDefault();

            return user;
        }



        /// <summary>
        /// 通过短信验证码修改账户手机号
        /// </summary>
        /// <param name="keyValue">key 为新手机号，value 为短信验证码</param>
        /// <returns></returns>
        [HttpPost("EditUserPhoneBySms")]
        public bool EditUserPhoneBySms([FromBody] dtoKeyValue keyValue)
        {

            if (Web.Auth.AuthorizeAction.SmsVerifyPhone(keyValue))
            {
                var userId = Guid.Parse(Web.Libraries.Verify.JwtToken.GetClaims("userId"));

                string phone = keyValue.Key.ToString();


                var checkPhone = db.TUser.Where(t => t.Id != userId && t.Phone == phone).Count();

                var user = db.TUser.Where(t => t.Id == userId).FirstOrDefault();


                var isMergeUser = false;

                if (isMergeUser)
                {
                    //获取目标手机号绑定的账户ID
                    var phoneUserId = db.TUser.Where(t => t.Phone == phone).Select(t => t.Id).FirstOrDefault();

                    user.Phone = phone;

                    db.SaveChanges();

                    //如果目标手机号绑定用户，则进行数据合并动作
                    if (phoneUserId != default)
                    {
                        //将手机号对应的用户移除，合并数据到新的账号
                        Common.DBHelper.MergeUser(phoneUserId, user.Id);
                    }

                    return true;
                }
                else
                {
                    if (checkPhone == 0)
                    {
                        user.Phone = phone;

                        db.SaveChanges();

                        return true;
                    }
                    else
                    {
                        HttpContext.Response.StatusCode = 400;

                        HttpContext.Items.Add("errMsg", "User.EditUserPhoneBySms.'The target mobile number has been bound by another account'");

                        return false;
                    }
                }


            }
            else
            {
                HttpContext.Response.StatusCode = 400;

                HttpContext.Items.Add("errMsg", "User.EditUserPhoneBySms.'Error in SMS verification code'");

                return false;
            }
        }




        /// <summary>
        /// 通过短信验证码修改账户密码</summary>
        /// <param name="keyValue">key为新密码，value为短信验证码</param>
        /// <returns></returns>
        [HttpPost("EditUserPassWordBySms")]
        public bool EditUserPassWordBySms([FromBody] dtoKeyValue keyValue)
        {

            var userId = Guid.Parse(Web.Libraries.Verify.JwtToken.GetClaims("userId"));

            string phone = db.TUser.Where(t => t.Id == userId).Select(t => t.Phone).FirstOrDefault();

            string smsCode = keyValue.Value.ToString();

            var checkSms = Web.Auth.AuthorizeAction.SmsVerifyPhone(new dtoKeyValue { Key = phone, Value = smsCode });

            if (checkSms)
            {
                string password = keyValue.Key.ToString();

                if (!string.IsNullOrEmpty(password))
                {
                    var user = db.TUser.Where(t => t.Id == userId).FirstOrDefault();

                    user.PassWord = password;

                    db.SaveChanges();


                    var tokenList = db.TUserToken.Where(t => t.UserId == userId).ToList();

                    db.TUserToken.RemoveRange(tokenList);

                    db.SaveChanges();

                    return true;
                }
                else
                {
                    HttpContext.Response.StatusCode = 400;

                    HttpContext.Items.Add("errMsg", "User.EditUserPassWordBySms.'New password is not allowed to be empty'");

                    return false;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;

                HttpContext.Items.Add("errMsg", "User.EditUserPassWordBySms.'Error in SMS verification code'");

                return false;
            }

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
            using (var db = new dbContext())
            {
                int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
                var data = db.TUser.Where(t => t.IsDelete == false && t.RoleId == Guid.Parse("B414EBC7-D646-4B57-B07C-4A39F08361E1"));
                if (!string.IsNullOrEmpty(dtoPage.searchKey))
                {

                    data = data.Where(t => t.Name.Contains(dtoPage.searchKey) || t.Phone.Contains(dtoPage.searchKey) || t.Role.Name.Contains(dtoPage.searchKey));
                }
                dtoPage.total = data.Count();
                dtoPage.data = data.Skip(skip).Take(dtoPage.pageSize).Select(t => new dtoUser
                {
                    Id = t.Id,
                    Name = t.Name,
                    Phone = t.Phone,
                    Role = t.Role.Name,
                    HeadImgs = db.TFile.Where(f => f.Table == "TUser" && f.IsDelete == false && f.Sign == "head" && f.TableId == t.Id).OrderByDescending(x => x.CreateTime).Select(f => new dtoKeyValue
                    {
                        Key = f.Id,
                        Value = OssHelper.url + f.Path
                    }).Take(1).ToList(),
                    CreateTime = t.CreateTime
                }).ToList();

                return dtoPage;
            }
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
            using (var db = new dbContext())
            {
                int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
                var data = db.TUser.Where(t => t.IsDelete == false && t.RoleId != Guid.Parse("B414EBC7-D646-4B57-B07C-4A39F08361E1"));
                if (!string.IsNullOrEmpty(dtoPage.searchKey))
                {

                    data = data.Where(t => t.Name.Contains(dtoPage.searchKey) || t.Phone.Contains(dtoPage.searchKey) || t.Role.Name.Contains(dtoPage.searchKey));
                }
                dtoPage.total = data.Count();
                dtoPage.data = data.Skip(skip).Take(dtoPage.pageSize).Select(t => new dtoUser
                {
                    Id = t.Id,
                    Name = t.Name,
                    Phone = t.Phone,
                    Role = t.Role.Name,
                    HeadImgs = db.TFile.Where(f => f.Table == "TUser" && f.IsDelete == false && f.Sign == "head" && f.TableId == t.Id).OrderByDescending(x => x.CreateTime).Select(f => new dtoKeyValue
                    {
                        Key = f.Id,
                        Value = OssHelper.url + f.Path
                    }).Take(1).ToList(),
                    CreateTime = t.CreateTime
                }).ToList();

                return dtoPage;
            }
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
            using (var db = new dbContext())
            {
                var user = db.TUser.Where(t => t.Id == Id && t.IsDelete == false).Select(t => new dtoUser
                {
                    Id = t.Id,
                    Name = t.Name,
                    Phone = t.Phone,
                    Role = t.Role.Name,
                    RoleId = t.RoleId,
                    PassWord = t.PassWord,
                    CreateTime = t.CreateTime
                }).FirstOrDefault();

                return user;
            }
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
            try
            {
                using (var db = new dbContext())
                {
                    var data = db.TUser.Where(x => x.Id == user.Id && x.IsDelete == false).FirstOrDefault(); 
                    var tokenuser = db.TUser.Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                    var UserPh = db.TUser.Where(t => t.Phone == user.Phone && t.IsDelete == false && t.Role.Name != "user").FirstOrDefault();
                    if (data != null)
                    {
                        //验证手机号唯一不允许添加
                        if (UserPh != null && data.Id != UserPh.Id)
                        {
                            HttpContext.Response.StatusCode = 400;

                            HttpContext.Items.Add("errMsg", "手机号码已存在!");

                            return false;
                        }
                        var UserName = db.TUser.Where(t => t.Name == user.Name && t.IsDelete == false && t.Role.Name != "user").FirstOrDefault();
                        //验证姓名唯一不允许添加
                        if (UserName != null && data.Id != UserName.Id)
                        {
                            HttpContext.Response.StatusCode = 400;

                            HttpContext.Items.Add("errMsg", "人员姓名已存在!");

                            return false;
                        }
                        TUser olddata = new TUser();
                        Common.PropertyHelper.Assignment<TUser>(olddata, data);
                        //编辑
                        data.Name = user.Name;
                        data.Phone = user.Phone;
                        data.UpdateTime = DateTime.Now;
                        data.RoleId = user.RoleId;
                        data.PassWord = user.PassWord;
                       
                    }
                    else
                    {

                        //验证手机号唯一不允许添加
                        if (UserPh != null)
                        {
                            HttpContext.Response.StatusCode = 400;

                            HttpContext.Items.Add("errMsg", "手机号码已存在!");

                            return false;
                        }
                        var UserName = db.TUser.Where(t => t.Name == user.Name && t.IsDelete == false && t.Role.Name != "user").FirstOrDefault();
                        //验证姓名唯一不允许添加
                        if (UserName != null)
                        {
                            HttpContext.Response.StatusCode = 400;

                            HttpContext.Items.Add("errMsg", "人员姓名已存在!");

                            return false;
                        }

                        data = new TUser();
                        data.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;
                        data.Name = user.Name;
                        data.Phone = user.Phone;
                        data.RoleId = user.RoleId;
                        data.IsDelete = false;
                        data.CreateTime = DateTime.Now;
                        data.PassWord = user.PassWord;
                        db.TUser.Add(data); 
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
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
            try
            {
                using (var db = new dbContext())
                {
                    var data = db.TUser.Where(x => x.Id == Id && x.IsDelete == false).FirstOrDefault();
                    var tokenuser = db.TUser.Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                    //编辑
                    data.IsDelete = true;
                    data.DeleteTime = DateTime.Now;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
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
            using (var db = new dbContext())
            {
                int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
                var data = db.TRole.Where(t => t.IsDelete == false);
                if (!string.IsNullOrEmpty(dtoPage.searchKey))
                {

                    data = data.Where(t => t.Name.Contains(dtoPage.searchKey) || t.Remarks.Contains(dtoPage.searchKey));
                }
                dtoPage.total = data.Count();
                dtoPage.data = data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).Select(t => new dtoRole
                {
                    Id = t.Id,
                    Name = t.Name,
                    Remarks = t.Remarks,
                    CreateTime = t.CreateTime
                }).ToList();

                return dtoPage;
            }
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
            using (var db = new dbContext())
            {

                var user = db.TRole.Where(t => t.Id == Id && t.IsDelete == false).Select(t => new dtoRole
                {
                    Id = t.Id,
                    Name = t.Name,
                    Remarks = t.Remarks,
                    CreateTime = t.CreateTime
                }).FirstOrDefault();

                return user;
            }
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
            try
            {
                using (var db = new dbContext())
                {
                    var data = db.TRole.Where(x => x.Id == role.Id && x.IsDelete == false).FirstOrDefault(); 
                    var tokenuser = db.TUser.Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                    if (data != null)
                    {
                        TRole olddata = new TRole(); 
                        //编辑
                        data.Name = role.Name;
                        data.Remarks = role.Remarks; 
                    }
                    else
                    {
                        data = new TRole();
                        data.Id = role.Id == Guid.Empty ? Guid.NewGuid() : role.Id;
                        data.Name = role.Name;
                        data.Remarks = role.Remarks;
                        data.IsDelete = false;
                        data.CreateTime = DateTime.Now;
                        db.TRole.Add(data);
                         

                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
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
            try
            {
                using (var db = new dbContext())
                {
                    var users = db.TUser.Where(x => x.RoleId == Id && x.IsDelete == false).ToList();
                    var tokenuser = db.TUser.Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                    if (users.Count > 0)
                    {

                        HttpContext.Response.StatusCode = 400;
                        HttpContext.Items.Add("errMsg", "当前角色含有 未失效用户删除失败");

                        return false;
                    }
                    var data = db.TRole.Where(x => x.Id == Id && x.IsDelete == false).FirstOrDefault();
                    //删除
                    data.IsDelete = true;
                    data.DeleteTime = DateTime.Now;

                    db.SaveChanges();
                     return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }



        /// <summary>
        /// 获取可用户角色的键值对列表信息
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetUserRoleKey")]
        [SkipAuthority]
        public List<dtoKeyValue> GetUserRoleKey()
        {
            using (var db = new dbContext())
            {

                var data = db.TRole.Where(t => t.IsDelete == false).Select(x => new dtoKeyValue
                {
                    Key = x.Id,
                    Value = x.Name,
                }).ToList();
                return data;
            }
        }

        /// <summary>
        /// 获取可用系统用户
        /// </summary> 
        /// <returns></returns>
        [HttpGet("GetUserSystemKey")]
        [SkipAuthority]
        public List<dtoKeyValue> GetUserSystemKey()
        {
            using (var db = new dbContext())
            {
                var data = db.TUser.Where(t => t.IsDelete == false && t.Role.Name != "user").Select(x => new dtoKeyValue
                {
                    Key = x.Id,
                    Value = x.Name,
                }).ToList();
                return data;
            }
        }

    }
}