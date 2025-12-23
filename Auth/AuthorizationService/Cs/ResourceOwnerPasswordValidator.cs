using Common.Json;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using kevin.Cache.Service;
using kevin.Domain.Entities;
using Kevin.Common.App.Global;
using Kevin.Models.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationService
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            string dataJson = "";
            switch (context.Request.Client.ClientName)
            {
                //系统用户登录
                case "UserClient":
                    userDto User = null;
                    //查询数据库
                    dataJson = GlobalServices.ServiceProvider.GetService<ICacheService>().GetString("CacheClientUserList" + context.UserName); 
                    if (!string.IsNullOrEmpty(dataJson))
                    {
                        User = JsonHelper.JSONToObject<userDto>(dataJson);
                    }
                    else
                    {
                        using (var db = new KevinDbContext())
                        {
                            User = db.Set<TUser>().Where(x => x.IsDelete == false && x.Id.ToString() == context.UserName && x.PasswordHash == context.Password).Select(x => new userDto
                            {
                                Id = x.Id.ToString(),
                                Phone = x.Phone,
                                CreatedTime = x.CreateTime,
                                Name = x.Name,
                                TenantId = x.TenantId,
                                IsSuperAdmin = x.IsSuperAdmin,

                            }).FirstOrDefault();
                        }
                    }
                    if (User != null && User != default)
                    {
                        context.Result = new GrantValidationResult(
                         subject: context.UserName,
                         claims: GetUserClaims(User),
                         authenticationMethod: "clientCustom"
                         );
                        CacheUserListAsync(User);
                    }
                    else
                    {
                        //验证失败
                        context.Result = new GrantValidationResult(
                            TokenRequestErrors.InvalidGrant,
                            "账号密码错误"
                            );
                    }
                    break;
                default:
                    break;
            }
            return Task.FromResult(0);
        }
        private Claim[] GetUserClaims(userDto user)
        {
            return new Claim[]
            {
            new Claim(JwtKeinClaimTypes.UserId, user.Id),
             new Claim(JwtKeinClaimTypes.issuperadmin, user.IsSuperAdmin.ToString()),
                   new Claim(JwtKeinClaimTypes.Name, user.Name??""),
                        new Claim(JwtKeinClaimTypes.CreatedTime,user.CreatedTime!=null?user.CreatedTime.Value.ToString("yyyy-MM-dd"):""),
                         new Claim(JwtKeinClaimTypes.TenantId, user.TenantId.ToString()),
            };
        }
        /// <summary>
        /// 异步保存缓存
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private Task CacheUserListAsync(userDto user)
        {
            return Task.Run(() =>
            {
                GlobalServices.ServiceProvider.GetService<ICacheService>().SetString("CacheClientUserList" + user.Id, JsonHelper.ObjectToJSON(user));
                using (var db = new KevinDbContext())
                {
                    var data = db.Set<TUser>().Where(x => x.IsDelete == false && x.Id.ToString() == user.Id).FirstOrDefault();
                    data.RecentLoginTime = DateTime.Now;
                    data.UpdateTime = DateTime.Now;
                    db.SaveChanges();
                }
            });
        }
    }
}
