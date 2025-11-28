using Aop.Api.Domain;
using kevin.Domain.Configuration;
using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IServices.Organizational;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Domain.Share.Dtos.User;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using Kevin.Common.Helper;
using Repository.Database;
using System;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace kevin.Application
{
    public class UserService : BaseService, IUserService
    {
        public IUserRp userRp { get; set; }
        public IWeiXinKeyRp weiXinKeyRp { get; set; }
        public IFileRp fileRp { get; set; }

        public IRoleRp roleRp { get; set; }

        public IUserBindWeixinRp userBindWeixinRp { get; set; }

        public IUserBindRoleRp userBindRoleRp { get; set; }

        public IPositionService positionService { get; set; }

        public IDepartmentService departmentService { get; set; }

        public IUserInfoRp userInfoRp { get; set; }

        public IUserBindPositionRp userBindPositionRp { get; set; }
        public UserService(IHttpContextAccessor _httpContextAccessor, IUserRp _userRp, IWeiXinKeyRp _weiXinKeyRp, IFileRp _IFileRp, IRoleRp _IRoleRp,
            IUserBindWeixinRp _IUserBindWeixinRp, IUserBindRoleRp userBindRoleRp, IPositionService _positionService,
            IUserBindPositionRp _userBindPositionRp, IDepartmentService _departmentService, IUserInfoRp _userInfoRp) : base(_httpContextAccessor)
        {
            userRp = _userRp;
            weiXinKeyRp = _weiXinKeyRp;
            fileRp = _IFileRp;
            roleRp = _IRoleRp;
            userBindWeixinRp = _IUserBindWeixinRp;
            this.userBindRoleRp = userBindRoleRp;
            this.positionService = _positionService;
            this.userBindPositionRp = _userBindPositionRp;
            this.departmentService = _departmentService;
            this.userInfoRp = _userInfoRp;
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
                string phone = keyValue.Key?.ToString() ?? "";
                var checkPhone = userRp.Query().Where(t => t.Id != userId && t.Phone == phone).Count();
                var user = userRp.Query().Where(t => t.Id == userId).FirstOrDefault();
                var isMergeUser = false;
                if (user != default)
                {
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
                throw new UserFriendlyException("登录用户不存在");

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
        public dtoUser GetUser(long userId)
        {
            if (userId == default)
            {
                userId = CurrentUser.UserId;
            }
            var roleData = userBindRoleRp.Query().Where(t => t.UserId == userId && t.IsDelete == false).Include(u => u.Role).ToList();
            return userRp.Query().Where(t => t.Id == userId && t.IsDelete == false).Select(t => new dtoUser
            {
                Name = t.Name,
                Id = t.Id,
                NickName = t.NickName,
                Phone = t.Phone,
                Email = t.Email,
                CreateTime = t.CreateTime,
                Roles = roleData.Select(r => new dtoRole { Id = r.Role.Id, Name = r.Role.Name ?? "", Remarks = r.Role.Remarks ?? "", CreateTime = r.Role.CreateTime }).ToList(),
            }).FirstOrDefault() ?? new();
        }

        /// <summary>
        /// 登录用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <param name="tenantId">租户id</param>
        /// <returns></returns>
        public dtoUser LoginUser(string name, string pwd, Int32 tenantId, string passwordHash)
        {
            var user = userRp.Query().Where(t => (t.Name == name || t.Phone == name) && t.PasswordHash == new HashHelper().SHA256Hash(pwd) && t.IsDelete == false && t.TenantId == tenantId).Select(t => new dtoUser
            {
                Name = t.Name,
                NickName = t.NickName,
                Phone = t.Phone,
                Email = t.Email,
                CreateTime = t.CreateTime,
                Id = t.Id
            }).FirstOrDefault();
            if (!string.IsNullOrEmpty(passwordHash))
            {
                user = userRp.Query().Where(t => (t.Name == name || t.Phone == name) && t.PasswordHash == passwordHash && t.IsDelete == false && t.TenantId == tenantId).Select(t => new dtoUser
                {
                    Name = t.Name,
                    NickName = t.NickName,
                    Phone = t.Phone,
                    Email = t.Email,
                    CreateTime = t.CreateTime,
                    Id = t.Id
                }).FirstOrDefault();
            }
            if (user == default)
            {
                throw new UserFriendlyException("账户或密码错误");
            }
            if (!user.Status)
            {
                throw new UserFriendlyException("用户已失效");
            }
            var roleData = userBindRoleRp.Query().Where(t => t.UserId == user.Id && t.IsDelete == false).Include(u => u.Role).ToList();
            user.Roles = roleData.Where(r => r.Role != default).Select(r => new dtoRole { Id = r.RoleId, Name = r.Role?.Name ?? "", Remarks = r.Role?.Remarks ?? "", CreateTime = r.Role.CreateTime }).ToList();
            return user;
        }

        /// <summary>
        /// 获取微信小程序OpenId
        /// </summary>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        /// <param name="code">微信临时code</param>
        /// <returns>openid,userid</returns>
        /// <remarks>传入租户ID和微信临时 code 获取 openid，如果 openid 在系统有中对应用户，则一并返回用户的ID值，否则用户ID值为空</remarks>
        public string GetWeiXinMiniAppOpenId(long weixinkeyid, string code)
        {
            var weixinkey = weiXinKeyRp.Query().Where(t => t.Id == weixinkeyid).FirstOrDefault();
            if (weixinkey != default)
            {
                var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);

                var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);
                string openid = wxinfo.openid;
                return openid;
            }


            return "";
        }

        /// <summary>
        /// 通过短信验证码修改账户密码</summary>
        /// <param name="keyValue">key为新密码，value为短信验证码</param>
        /// <returns></returns> 
        public bool EditUserPassWordBySms(dtoKeyValue keyValue)
        {
            var userId = CurrentUser.UserId;
            var user = userRp.Query().Where(t => t.Id == userId).FirstOrDefault();
            if (user != default && keyValue.Value != default && keyValue.Key != default)
            {
                string phone = user.Phone ?? "";

                string smsCode = keyValue.Value.ToString() ?? "";

                var checkSms = Web.Auth.AuthorizeAction.SmsVerifyPhone(new dtoKeyValue { Key = phone, Value = smsCode });

                if (checkSms)
                {
                    string password = keyValue.Key.ToString() ?? "";

                    if (!string.IsNullOrEmpty(password))
                    {
                        if (user != default)
                        {
                            user.ChangePassword(password);
                            userRp.SaveChanges();
                        }
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
            throw new UserFriendlyException("用户不存在");

        }
        /// <summary>
        /// 获取小程序用户列表信息
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public dtoPageData<dtoUser> GetUserList(dtoPageData<dtoUser> dtoPage)
        {
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = userRp.Query().Where(t => t.IsDelete == false && t.IsSystem == false);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey) || (t.Phone ?? "").Contains(dtoPage.searchKey));
            }
            dtoPage.total = data.Count();
            dtoPage.data = data.Skip(skip).Take(dtoPage.pageSize).Select(t => new dtoUser
            {
                Id = t.Id,
                Name = t.Name,
                Phone = t.Phone,
                HeadImgs = fileRp.Query().Where(f => f.Table == "TUser" && f.IsDelete == false && f.Sign == "head" && f.TableId == t.Id.ToString()).OrderByDescending(x => x.CreateTime).Select(f => new dtoKeyValue
                {
                    Key = f.Id,
                    Value = f.Url
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
        public async Task<dtoPageData<dtoUser>> GetSysUserList(dtoPagePar<dtoUserPar> par)
        {
            var dtoPage = new dtoPageData<dtoUser>();
            int skip = (dtoPage.pageNum - 1) * dtoPage.pageSize;
            var data = userRp.Query().Where(t => t.IsDelete == false && t.IsSystem == true);
            if (!string.IsNullOrEmpty(par.searchKey))
            {
                data = data.Where(t => ((t.Name ?? "") ?? "").Contains(dtoPage.searchKey) || (t.Phone ?? "").Contains(dtoPage.searchKey) || (t.NickName ?? "").Contains(dtoPage.searchKey) || (t.Email ?? "").Contains(dtoPage.searchKey));
            }
            if (par.Parameter?.PositionId > 0)
            {
                //获取当前岗位下所有USERID
                var ids = positionService.GetChildUserIds(par.Parameter.PositionId);
                data = data.Where(t => ids.Contains(t.Id));
            }
            if (par.Parameter?.DepartmentId > 0)
            {
                //获取当前岗位下所有USERID
                var ids = departmentService.GetChildUserIds(par.Parameter.DepartmentId);
                data = data.Where(t => ids.Contains(t.Id));
            }
            dtoPage.total = data.Count();
            dtoPage.data = data.Skip(skip).Take(dtoPage.pageSize).Select(t => new dtoUser
            {
                Id = t.Id,
                Name = t.Name,
                Phone = t.Phone,
                Email = t.Email,
                NickName = t.NickName,
                Status = t.Status,
                RecentLoginTime = t.RecentLoginTime,
                HeadImgs = fileRp.Query().Where(f => f.Table == "TUser" && f.IsDelete == false && f.Sign == "head" && f.TableId == t.Id.ToString()).OrderByDescending(x => x.CreateTime).Select(f => new dtoKeyValue
                {
                    Key = f.Id,
                    Value = f.Url
                }).Take(1).ToList(),
                CreateTime = t.CreateTime
            }).ToList();
            var roleData = userBindRoleRp.Query().Where(t => dtoPage.data.Select(d => d.Id).ToList().Contains(t.UserId) && t.IsDelete == false).Include(u => u.Role).ToList();
            foreach (var item in dtoPage.data)
            {
                item.Roles = roleData.Where(t => t.UserId == item.Id).Select(t => new dtoRole { Id = t.RoleId, Name = t.Role?.Name ?? "" }).ToList();
            }
            var positionData = userBindPositionRp.Query().Where(t => dtoPage.data.Select(d => d.Id).ToList().Contains(t.UserId) && t.IsDelete == false).Include(u => u.Position).ToList();
            foreach (var item in dtoPage.data)
            {
                item.Positions = positionData.Where(t => t.UserId == item.Id).Select(t => new PositionDto { Id = t.PositionId, Name = t.Position?.Name ?? "" }).ToList();
            }
            var userInfoData = userInfoRp.Query().Where(t => dtoPage.data.Select(d => d.Id).ToList().Contains(t.UserId) && t.IsDelete == false).Include(t => t.User).ToList().MapToList<TUserInfo, dtoUserInfo>();
            var departmentData = new List<DepartmentDto>();
            if (userInfoData.Select(t => t.DepartmentId).ToList().Count > 0)
            {
                departmentData = await departmentService.GetALLList(userInfoData.Select(t => t.DepartmentId).ToList());
            }

            foreach (var item in dtoPage.data)
            {
                item.dtoUserInfo = userInfoData.FirstOrDefault(t => t.UserId == item.Id);
                if (item.dtoUserInfo != default)
                {
                    item.dtoUserInfo.DepartmentName = departmentData.FirstOrDefault(t => t.Id == item.dtoUserInfo.DepartmentId)?.Name;
                }
            }
            return dtoPage;
        }

        /// <summary>
        /// 后台管理通过 UserId 获取用户信息 
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns> 
        public dtoUser GetSysUserWhereId(long Id)
        {
            var user = userRp.Query().Where(t => t.Id == Id && t.IsDelete == false).Select(t => new dtoUser
            {
                Id = t.Id,
                Name = t.Name,
                Phone = t.Phone,
                PassWord = t.PasswordHash,
                CreateTime = t.CreateTime,
                Status = t.Status,
                RecentLoginTime = t.RecentLoginTime,
            }).FirstOrDefault();
            var roleData = userBindRoleRp.Query().Where(t => t.UserId == user.Id && t.IsDelete == false).Include(u => u.Role).ToList();
            user.Roles = roleData.Where(r => r.Role != default).Select(r => new dtoRole { Id = r.RoleId, Name = r.Role?.Name ?? "", Remarks = r.Role?.Remarks ?? "", CreateTime = r.Role.CreateTime }).ToList();
            var positionData = userBindPositionRp.Query().Where(t => Id == t.UserId && t.IsDelete == false).Include(u => u.Position).ToList();
            user.Positions = positionData.Where(t => t.UserId == user.Id).Select(t => new PositionDto { Id = t.PositionId, Name = t.Position?.Name ?? "" }).ToList();

            var userInfoData = userInfoRp.Query().Where(t => Id == t.UserId && t.IsDelete == false).ToList().MapToList<TUserInfo, dtoUserInfo>().ToList();
            user.dtoUserInfo = userInfoData.FirstOrDefault(t => t.UserId == user.Id);
            return user ?? new dtoUser();
        }

        /// <summary>
        /// 新增编辑用户信息 
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns> 
        public bool EditUser(dtoUser user)
        {
            var data = userRp.Query().Where(x => x.Id == user.Id && x.IsDelete == false && x.IsSystem == true).FirstOrDefault();
            var UserPh = userRp.Query().Where(t => t.Phone == user.Phone && t.IsDelete == false && t.IsSystem == true).FirstOrDefault();
            if (data != null)
            {
                //验证手机号唯一不允许添加
                if (UserPh != null && data.Id != UserPh.Id)
                {
                    throw new UserFriendlyException("手机号码已存在");
                }
                var UserName = userRp.Query().Where(t => t.Name == user.Name && t.IsDelete == false).FirstOrDefault();
                //验证姓名唯一不允许添加
                if (UserName != null && data.Id != UserName.Id)
                {
                    throw new UserFriendlyException("人员姓名已存在");
                }
                //编辑
                data.Name = user.Name;
                data.Phone = user.Phone;
                data.Email = user.Email;
                data.UpdateTime = DateTime.Now;
                data.NickName = user.NickName;
                data.Status = user.Status;
                if (!string.IsNullOrEmpty(user.PassWord))
                {
                    data.ChangePassword(user.PassWord);
                }
            }
            else
            {
                //验证手机号唯一不允许添加
                if (UserPh != null)
                {
                    throw new UserFriendlyException("手机号码已存在");
                }
                var UserName = userRp.Query().Where(t => t.Name == user.Name && t.IsDelete == false && t.IsSystem == true).FirstOrDefault();
                //验证姓名唯一不允许添加
                if (UserName != null)
                {
                    throw new UserFriendlyException("人员姓名已存在");
                }

                data = new TUser();
                data.Id = user.Id == default ? SnowflakeIdService.GetNextId() : user.Id;
                data.Name = user.Name;
                data.Phone = user.Phone;
                data.IsDelete = false;
                data.IsSystem = true;
                data.Email = user.Email;
                data.UpdateTime = DateTime.Now;
                data.Status = true;
                data.CreateTime = DateTime.Now;
                if (!string.IsNullOrEmpty(user.PassWord))
                {
                    data.ChangePassword(user.PassWord);
                }
                user.Id = data.Id;
                userRp.Add(data);
            }
            userRp.SaveChangesWithSaveLog();
            //处理角色信息
            EditUserRoles(user);
            //处理其他信息
            EditUserInfo(user);
            //处理处理职位信息
            positionService.AddEditUserBindPosition(user.Id, user.Positions?.Select(t => t.Id).ToList() ?? new List<long>());
            return true;
        }
        /// <summary>
        /// 处理角色信息
        /// </summary>
        /// <param name="user"></param>
        private void EditUserRoles(dtoUser user)
        {
            var deleteRoleData = userBindRoleRp.Query().Where(t => t.UserId == user.Id && t.IsDelete == false).ToList();
            foreach (var item in deleteRoleData)
            {
                item.IsDelete = true;
                item.DeleteTime = DateTime.Now;
                item.UserId = CurrentUser.UserId;
            }
            userBindRoleRp.SaveChanges();
            if (user.Roles?.Count > 0)
            {
                foreach (var role in user.Roles)
                {
                    var userBindRole = new TUserBindRole();
                    userBindRole.Id = SnowflakeIdService.GetNextId();
                    userBindRole.UserId = user.Id;
                    userBindRole.RoleId = role.Id;
                    userBindRole.IsDelete = false;
                    userBindRole.CreateUserId = CurrentUser.UserId;
                    userBindRole.CreateTime = DateTime.Now;
                    userBindRoleRp.Add(userBindRole);
                }
                userBindRoleRp.SaveChanges();
            }
        }
        /// <summary>
        /// 处理角色信息
        /// </summary>
        /// <param name="user"></param>
        private void EditUserInfo(dtoUser user)
        {
            if (user.dtoUserInfo != default)
            { 
                if (user.dtoUserInfo.Id != default)
                {
                    var data = userInfoRp.Query().Where(t => t.Id == user.dtoUserInfo.Id && t.IsDelete == false).FirstOrDefault();
                    data.Sex = user.dtoUserInfo.Sex;
                    data.EmployeeStatus = user.dtoUserInfo.EmployeeStatus;
                    data.Signature = user.dtoUserInfo.Signature;
                    data.HireDate = user.dtoUserInfo.HireDate;
                    data.DepartmentId = user.dtoUserInfo.DepartmentId;
                    data.SupervisorId = user.dtoUserInfo.SupervisorId;
                    data.EmployeeNo = user.dtoUserInfo.EmployeeNo;
                    data.WeChat = user.dtoUserInfo.WeChat;
                    data.QQ = user.dtoUserInfo.QQ;
                }
                else
                {
                    var addinfo = new TUserInfo();
                    addinfo.Id = SnowflakeIdService.GetNextId();
                    addinfo.CreateTime = DateTime.Now;
                    addinfo.CreateUserId= CurrentUser.UserId;
                    addinfo.UserId = user.Id;
                    addinfo.Sex = user.dtoUserInfo.Sex;
                    addinfo.EmployeeStatus = user.dtoUserInfo.EmployeeStatus;
                    addinfo.Signature = user.dtoUserInfo.Signature;
                    addinfo.HireDate = user.dtoUserInfo.HireDate;
                    addinfo.DepartmentId = user.dtoUserInfo.DepartmentId;
                    addinfo.SupervisorId = user.dtoUserInfo.SupervisorId;
                    addinfo.EmployeeNo = user.dtoUserInfo.EmployeeNo;
                    addinfo.WeChat = user.dtoUserInfo.WeChat;
                    addinfo.QQ = user.dtoUserInfo.QQ;
                    userInfoRp.Add(addinfo);

                }
                userInfoRp.SaveChanges();
            }
        }
        /// <summary>
        /// 删除用户信息 
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns> 
        public bool DeleteUser(long Id)
        {
            if (CurrentUser.UserId == Id)
            {
                throw new UserFriendlyException("你不能删除你自己");
            }
            if (CurrentUser.IsSuperAdmin)
            {
                throw new UserFriendlyException("超级管理员不能删除");
            }
            if (TUserBaseData.TUsers.Where(t => t.Id == Id).FirstOrDefault() != default)
            {
                throw new UserFriendlyException("种子数据不能删除");
            }
            var data = userRp.Query().Where(x => x.Id == Id && x.IsDelete == false && x.TenantId == CurrentUser.TenantId).FirstOrDefault();
            if (data != default)
            {
                //编辑
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                userRp.SaveChanges();
                var deleteRoleData = userBindRoleRp.Query().Where(t => t.UserId == data.Id && t.IsDelete == false).ToList();
                foreach (var item in deleteRoleData)
                {
                    item.IsDelete = true;
                    item.DeleteTime = DateTime.Now;
                    item.UserId = CurrentUser.UserId;
                }
                userBindRoleRp.SaveChanges();
            }
            else
            {
                throw new UserFriendlyException(Id + "人员不存在");
            }
            return true;
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
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey) || (t.Remarks ?? "").Contains(dtoPage.searchKey));
            }
            dtoPage.total = data.Count();
            dtoPage.data = data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).Select(t => new dtoRole
            {
                Id = t.Id,
                Name = t.Name ?? "",
                Remarks = t.Remarks ?? "",
                CreateTime = t.CreateTime
            }).ToList();

            return dtoPage;
        }


        /// <summary>
        /// 通过id获取角色信息 
        /// </summary>
        /// <param name="Id">ID</param>
        /// <returns></returns> 
        public dtoRole GetRoleWhereId(long Id)
        {
            var user = roleRp.Query().Where(t => t.Id == Id && t.IsDelete == false).Select(t => new dtoRole
            {
                Id = t.Id,
                Name = t.Name ?? "",
                Remarks = t.Remarks ?? "",
                CreateTime = t.CreateTime
            }).FirstOrDefault();
            return user ?? new dtoRole();
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
                    data.Id = role.Id == default ? SnowflakeIdService.GetNextId() : role.Id;
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
        public bool DeleteUserRole(long Id)
        {
            try
            {
                var users = userBindRoleRp.Query().Where(x => x.RoleId == Id && x.IsDelete == false).ToList();
                if (users.Count > 0)
                {
                    throw new UserFriendlyException("当前角色含有 未失效用户删除失败");
                }
                var data = roleRp.Query().Where(x => x.Id == Id && x.IsDelete == false).FirstOrDefault();
                if (data != default)
                {
                    //删除
                    data.IsDelete = true;
                    data.DeleteTime = DateTime.Now;
                    roleRp.SaveChanges();
                }
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
                Value = x.Name ?? "",
            }).ToList();
            return data;
        }

        /// <summary>
        /// 获取可用系统用户
        /// </summary> 
        /// <returns></returns> 
        public List<dtoKeyValue> GetUserSystemKey()
        {
            var data = userRp.Query().Where(t => t.IsDelete == false && t.Status == true && t.TenantId == CurrentUser.TenantId).Select(x => new dtoKeyValue
            {
                Key = x.Id,
                Value = x.Name ?? "",
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
        public string GetWeiXinMiniAppPhone(string iv, string encryptedData, string code, long weixinkeyid)
        {
            var weixinkey = weiXinKeyRp.Query().Where(t => t.Id == weixinkeyid).FirstOrDefault();
            if (weixinkey != default)
            {
                var weiXinHelper = new Web.Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret);
                var wxinfo = weiXinHelper.GetOpenIdAndSessionKey(code);

                string openid = wxinfo.openid;
                string sessionkey = wxinfo.sessionkey;

                var strJson = Web.Libraries.WeiXin.MiniApp.WeiXinHelper.DecryptionData(encryptedData, sessionkey, iv);

                var user = userBindWeixinRp.Query().Where(t => t.WeiXinOpenId == openid & t.WeiXinKeyId == weixinkeyid).Select(t => t.User).FirstOrDefault();
                if (user != default)
                {
                    user.Phone = Common.Json.JsonHelper.GetValueByKey(strJson, "phoneNumber");
                    userBindWeixinRp.SaveChanges();
                    return user.Phone;
                }
            }

            return "";
        }

        public Task<bool> ChangePasswordTokenUser(string oldPwd, string newPwd, CancellationToken cancellationToken)
        {
            var user = userRp.Query().Where(t => t.Id == CurrentUser.UserId && t.IsDelete == false).FirstOrDefault();
            if (user == default)
            {
                throw new UserFriendlyException("用户不存在");
            }
            if (!user.ValidatePassword(oldPwd))
            {
                throw new UserFriendlyException("旧密码错误");
            }
            user.ChangePassword(newPwd);
            userRp.SaveChanges();
            return Task.FromResult(true);
        }
        /// <summary>
        /// 更新登录时间
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public Task<bool> UpdateRecentLoginTime(long guid)
        {
            using var db = new KevinDbContext();
            var user = db.Set<TUser>().Where(t => t.Id == guid).FirstOrDefault();
            if (user == default)
            {
                throw new UserFriendlyException("用户不存在");
            }
            user.UpdateTime = DateTime.Now;
            user.RecentLoginTime = DateTime.Now;
            db.SaveChanges();
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetAllUserCount()
        {
            return await userRp.Query().Where(t => t.IsDelete == false).CountAsync();
        }
    }
}
