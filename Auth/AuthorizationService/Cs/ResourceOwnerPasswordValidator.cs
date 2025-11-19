
using Common;
using Common.Json;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using kevin.Cache.Service;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Kevin;
using kevin.Domain.Share.Interfaces;
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
                case "UMUserClient":
                    uMClientUserDto uMUser = null;
                    //查询数据库
                    dataJson = GlobalServices.ServiceProvider.GetService<ICacheService>().GetString("CacheClientUserList"+context.UserName);

                    if (!string.IsNullOrEmpty(dataJson))
                    {
                        uMUser = JsonHelper.JSONToObject<uMClientUserDto>(dataJson);
                    }
                    else
                    {
                        using (var db = new KevinDbContext())
                        {
                            uMUser = db.Set<TUser>().Where(x => x.IsDelete == false && x.Id.ToString() == context.UserName).Select(x => new uMClientUserDto
                            {
                                Id = x.Id.ToString(),
                                Phone = x.Phone,
                                CreatedTime = x.CreateTime,
                                Name = x.Name,
                                TenantId = x.TenantId
                            }).FirstOrDefault();
                        }
                    }
                    if (uMUser != null && uMUser != default)
                    {
                        context.Result = new GrantValidationResult(
                         subject: context.UserName,
                         claims: GetUserClaims(uMUser),
                         authenticationMethod: "clientCustom"
                         );
                        CacheUserListAsync(uMUser);
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
        private Claim[] GetUserClaims(uMClientUserDto user)
        {
            return new Claim[]
            {
            new Claim(JwtKeinClaimTypes.UserId, user.Id),
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
        private Task CacheUserListAsync(uMClientUserDto user)
        {
            return Task.Run(() =>
            {
                GlobalServices.ServiceProvider.GetService<ICacheService>().SetString("CacheClientUserList"+user.Id, JsonHelper.ObjectToJSON(user));
                GlobalServices.ServiceProvider.GetService<IUserService>().UpdateRecentLoginTime(Guid.Parse(user.Id));
            });
        }
    }
}
