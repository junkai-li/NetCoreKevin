﻿using App.Domain.Interfaces.Repositorie.v1;
using Common.AliYun;
using kevin.Domain.Kevin;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using Microsoft.AspNetCore.Http;
using Service.Dtos.v1.User;
using Service.Services.v1._;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Global.Exceptions;

namespace Service.Services.v1
{
    public class UserService : BaseService, IUserService
    {
        public IUserRp userRp { get; set; }
        public IWeiXinKeyRp weiXinKeyRp { get; set; } 
        public IFileRp fileRp { get; set; }

        public IRoleRp roleRp { get; set; }

        public IUserBindWeixinRp userBindWeixinRp { get; set; }
        public UserService(IHttpContextAccessor _httpContextAccessor, IUserRp _userRp, IWeiXinKeyRp _weiXinKeyRp, IFileRp _IFileRp, IRoleRp _IRoleRp, IUserBindWeixinRp _IUserBindWeixinRp) : base(_httpContextAccessor)
        {
            userRp = _userRp;
            weiXinKeyRp = _weiXinKeyRp;
            fileRp = _IFileRp;
            roleRp = _IRoleRp;
            userBindWeixinRp = _IUserBindWeixinRp;
        }


        /// <summary>
        /// 通过短信验证码修改账户手机号
        /// </summary>
        /// <param name="keyValue">key 为新手机号，value 为短信验证码</param>
        /// <returns></returns>
        public bool EditUserPhoneBySms(dtoKeyValue keyValue)
        {
            if (Web.Auth.AuthorizeAction.SmsVerifyPhone(keyValue))
            {
                var userId = CurrentUser.UserId;
                string phone = keyValue.Key.ToString();
                var checkPhone = userRp.Query().Where(t => t.Id != userId && t.Phone == phone).Count();
                var user = userRp.Query().Where(t => t.Id == userId).FirstOrDefault();
                var isMergeUser = false;

                if (isMergeUser)
                {
                    //获取目标手机号绑定的账户ID
                    var phoneUserId = userRp.Query().Where(t => t.Phone == phone).Select(t => t.Id).FirstOrDefault();
                    user.Phone = phone;
                    userRp.SaveChanges();

                    //如果目标手机号绑定用户，则进行数据合并动作
                    if (phoneUserId != default)
                    {
                        //将手机号对应的用户移除，合并数据到新的账号
                        //Common.DBHelper.MergeUser(phoneUserId, user.Id);
                    }
                    return true;
                }
                else
                {
                    if (checkPhone == 0)
                    {
                        user.Phone = phone;
                        userRp.SaveChanges();
                        return true;
                    }
                    else
                    {
                        throw new UserFriendlyException("User.EditUserPhoneBySms.'The target mobile number has been bound by another account");
                    }
                }
            }
            else
            {
                throw new UserFriendlyException("User.EditUserPhoneBySms.'Error in SMS verification code'");
            }
        }

        /// <summary>
        /// 通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public dtoUser GetUser(Guid userId)
        {
            if (userId == default)
            {
                userId = CurrentUser.UserId;
            }

            return userRp.Query().Where(t => t.Id == userId && t.IsDelete == false).Select(t => new dtoUser
            {
                Name = t.Name,
                NickName = t.NickName,
                Phone = t.Phone,
                Email = t.Email,
                Role = t.Role.Name,
                CreateTime = t.CreateTime
            }).FirstOrDefault();
        }

        /// <summary>
        /// 获取微信小程序OpenId
        /// </summary>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        /// <param name="code">微信临时code</param>
        /// <returns>openid,userid</returns>
        /// <remarks>传入租户ID和微信临时 code 获取 openid，如果 openid 在系统有中对应用户，则一并返回用户的ID值，否则用户ID值为空</remarks>
        public string GetWeiXinMiniAppOpenId(Guid weixinkeyid, string code)
        {
            var weixinkey = weiXinKeyRp.Query().Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);

            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;

            return openid;
        }

        /// <summary>
        /// 通过短信验证码修改账户密码</summary>
        /// <param name="keyValue">key为新密码，value为短信验证码</param>
        /// <returns></returns> 
        public bool EditUserPassWordBySms(dtoKeyValue keyValue)
        {

            var userId = CurrentUser.UserId;

            string phone = userRp.Query().Where(t => t.Id == userId).Select(t => t.Phone).FirstOrDefault();

            string smsCode = keyValue.Value.ToString();

            var checkSms = Web.Auth.AuthorizeAction.SmsVerifyPhone(new dtoKeyValue { Key = phone, Value = smsCode });

            if (checkSms)
            {
                string password = keyValue.Key.ToString();

                if (!string.IsNullOrEmpty(password))
                {
                    var user = userRp.Query().Where(t => t.Id == userId).FirstOrDefault();

                    user.PassWord = password;

                    userRp.SaveChanges();

                    return true;
                }
                else
                {
                    throw new UserFriendlyException("User.EditUserPassWordBySms.'New password is not allowed to be empty'");
                }
            }
            else
            {
                throw new UserFriendlyException("User.EditUserPassWordBySms.'Error in SMS verification code''");
            }

        }


        /// <summary>
        /// 获取小程序用户列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public dtoPageData<dtoUser> GetUserList(dtoPageData<dtoUser> dtoPage)
        {

            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = userRp.Query().Where(t => t.IsDelete == false && t.RoleId == Guid.Parse("B414EBC7-D646-4B57-B07C-4A39F08361E1"));
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
                HeadImgs = fileRp.Query().Where(f => f.Table == "TUser" && f.IsDelete == false && f.Sign == "head" && f.TableId == t.Id).OrderByDescending(x => x.CreateTime).Select(f => new dtoKeyValue
                {
                    Key = f.Id,
                    Value = OssHelper.url + f.Path
                }).Take(1).ToList(),
                CreateTime = t.CreateTime
            }).ToList();

            return dtoPage;
        }

        /// <summary>
        /// 获取系统用户列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public dtoPageData<dtoUser> GetSysUserList(dtoPageData<dtoUser> dtoPage)
        {
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = userRp.Query().Where(t => t.IsDelete == false && t.RoleId != Guid.Parse("B414EBC7-D646-4B57-B07C-4A39F08361E1"));
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
                HeadImgs = fileRp.Query().Where(f => f.Table == "TUser" && f.IsDelete == false && f.Sign == "head" && f.TableId == t.Id).OrderByDescending(x => x.CreateTime).Select(f => new dtoKeyValue
                {
                    Key = f.Id,
                    Value = OssHelper.url + f.Path
                }).Take(1).ToList(),
                CreateTime = t.CreateTime
            }).ToList();

            return dtoPage;
        }

        /// <summary>
        /// 后台管理通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns> 
        public dtoUser GetSysUserWhereId(Guid Id)
        {
            var user = userRp.Query().Where(t => t.Id == Id && t.IsDelete == false).Select(t => new dtoUser
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







        /// <summary>
        /// 新增编辑用户信息 
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns> 
        public bool EditUser(dtoUser user)
        {
            try
            {
                var data = userRp.Query().Where(x => x.Id == user.Id && x.IsDelete == false).FirstOrDefault();
                var tokenuser = userRp.Query().Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                var UserPh = userRp.Query().Where(t => t.Phone == user.Phone && t.IsDelete == false && t.Role.Name != "user").FirstOrDefault();
                if (data != null)
                {
                    //验证手机号唯一不允许添加
                    if (UserPh != null && data.Id != UserPh.Id)
                    {
                        throw new UserFriendlyException("手机号码已存在");
                        return false;
                    }
                    var UserName = userRp.Query().Where(t => t.Name == user.Name && t.IsDelete == false && t.Role.Name != "user").FirstOrDefault();
                    //验证姓名唯一不允许添加
                    if (UserName != null && data.Id != UserName.Id)
                    {
                        throw new UserFriendlyException("人员姓名已存在");
                    }
                    TUser olddata = new();
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
                        throw new UserFriendlyException("手机号码已存在");
                    }
                    var UserName = userRp.Query().Where(t => t.Name == user.Name && t.IsDelete == false && t.Role.Name != "user").FirstOrDefault();
                    //验证姓名唯一不允许添加
                    if (UserName != null)
                    {
                        throw new UserFriendlyException("人员姓名已存在");
                    }

                    data = new TUser();
                    data.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;
                    data.Name = user.Name;
                    data.Phone = user.Phone;
                    data.RoleId = user.RoleId;
                    data.IsDelete = false;
                    data.CreateTime = DateTime.Now;
                    data.PassWord = user.PassWord;
                    userRp.Add(data);
                }
                userRp.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }



        /// <summary>
        /// 删除用户信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns> 
        public bool DeleteUser(Guid Id)
        {
            try
            {
                var data = userRp.Query().Where(x => x.Id == Id && x.IsDelete == false).FirstOrDefault();
                var tokenuser = userRp.Query().Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                //编辑
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                userRp.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }





        /// <summary>
        /// 获取用户角色列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public dtoPageData<dtoRole> GetUserRoleList(dtoPageData<dtoRole> dtoPage)
        {
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = roleRp.Query().Where(t => t.IsDelete == false);
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




        /// <summary>
        /// 通过id获取角色信息 
        /// </summary>
        /// <param name="Id">ID</param>
        /// <returns></returns> 
        public dtoRole GetRoleWhereId(Guid Id)
        {
            var user = roleRp.Query().Where(t => t.Id == Id && t.IsDelete == false).Select(t => new dtoRole
            {
                Id = t.Id,
                Name = t.Name,
                Remarks = t.Remarks,
                CreateTime = t.CreateTime
            }).FirstOrDefault();

            return user;
        }



        /// <summary>
        /// 新增编辑用户角色信息 
        /// </summary>
        /// <param name="role">user</param>
        /// <returns></returns> 
        public bool EditUserRole(dtoRole role)
        {
            try
            {
                var data = roleRp.Query().Where(x => x.Id == role.Id && x.IsDelete == false).FirstOrDefault();
                var tokenuser = userRp.Query().Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                if (data != null)
                {
                    TRole olddata = new();
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
                    roleRp.Add(data);


                }
                roleRp.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }



        /// <summary>
        /// 删除用户角色信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns> 
        public bool DeleteUserRole(Guid Id)
        {
            try
            {
                var users = userRp.Query().Where(x => x.RoleId == Id && x.IsDelete == false).ToList();
                var tokenuser = userRp.Query().Where(x => x.IsDelete == false && x.Id == CurrentUser.UserId).FirstOrDefault();
                if (users.Count > 0)
                {
                    throw new UserFriendlyException("当前角色含有 未失效用户删除失败");
                }
                var data = roleRp.Query().Where(x => x.Id == Id && x.IsDelete == false).FirstOrDefault();
                //删除
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;

                roleRp.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }



        /// <summary>
        /// 获取可用户角色的键值对列表信息
        /// </summary> 
        /// <returns></returns> 
        public List<dtoKeyValue> GetUserRoleKey()
        {
            var data = roleRp.Query().Where(t => t.IsDelete == false).Select(x => new dtoKeyValue
            {
                Key = x.Id,
                Value = x.Name,
            }).ToList();
            return data;
        }

        /// <summary>
        /// 获取可用系统用户
        /// </summary> 
        /// <returns></returns> 
        public List<dtoKeyValue> GetUserSystemKey()
        {

            var data = userRp.Query().Where(t => t.IsDelete == false && t.Role.Name != "user").Select(x => new dtoKeyValue
            {
                Key = x.Id,
                Value = x.Name,
            }).ToList();
            return data;
        }

        /// <summary>
        /// 获取微信小程序手机号
        /// </summary>
        /// <param name="iv">加密算法的初始向量</param>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据</param>
        /// <param name="code">微信临时code</param>
        /// <param name="weixinkeyid">微信配置密钥ID</param> 
        public string GetWeiXinMiniAppPhone(string iv, string encryptedData, string code, Guid weixinkeyid)
        {


            var weixinkey = weiXinKeyRp.Query().Where(t => t.Id == weixinkeyid).FirstOrDefault();
            var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);
            var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

            string openid = wxinfo.openid;
            string sessionkey = wxinfo.sessionkey;

            var strJson = Web.Libraries.WeiXin.MiniApp.WeiXinHelper.DecryptionData(encryptedData, sessionkey, iv);

            var user = userBindWeixinRp.Query().Where(t => t.WeiXinOpenId == openid & t.WeiXinKeyId == weixinkeyid).Select(t => t.User).FirstOrDefault();
            user.Phone = Common.Json.JsonHelper.GetValueByKey(strJson, "phoneNumber");
            userBindWeixinRp.SaveChanges(); 
            return user.Phone;
        }
    }
}
