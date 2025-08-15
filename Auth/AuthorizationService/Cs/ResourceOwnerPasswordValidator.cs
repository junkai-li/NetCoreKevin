
using Common;
using Common.Json;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using kevin.Cache.Service;
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
                //case "UserClient":
                //    userDto User = null;
                //    //var Password = Crypto.GetEncryptMD5(context.Password);
                //    var Password = context.Password;
                //    //查询数据库 
                //    dataJson = GlobalServices.ServiceProvider.GetService<ICacheService>().GetString("CacheUserList"+context.UserName);

                //    if (!string.IsNullOrEmpty(dataJson))
                //    {
                //        User = JsonHelper.JSONToObject<userDto>(dataJson);
                //    }
                //    else
                //    {
                //        using (var db = new dbContext())
                //        {
                //            User = db.TUser.Where(x => x.IsDelete == false && (x.Name == context.UserName || x.Phone == context.UserName) && x.PassWord == Password).Select(x => new userDto
                //            {
                //                Id = x.Id.ToString(),
                //                Phone = x.Phone,
                //                CreatedTime = x.CreateTime,
                //                Password = x.PassWord,
                //                Name = x.Name,
                //                TenantId =x.TenantId
                //            }).FirstOrDefault();
                //        }

                //    }
                //    if (User != null && User != default && User.Password == Password)
                //    {
                //        context.Result = new GrantValidationResult(
                //         subject: context.UserName,
                //         claims: GetUserClaims(User),
                //         authenticationMethod: "custom"
                //         );
                //        CacheUserListAsync(User);
                //    }
                //    else
                //    {
                //        //验证失败
                //        context.Result = new GrantValidationResult(
                //            TokenRequestErrors.InvalidGrant,
                //            "账号密码错误"
                //            );
                //    }
                //    break;
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
                        using (var db = new dbContext())
                        {
                            uMUser = db.TUser.Where(x => x.IsDelete == false && x.Id.ToString() == context.UserName).Select(x => new uMClientUserDto
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


        private Claim[] GetUserClaims(userDto user)
        {
            return new Claim[]
            {
            new Claim(JwtKeinClaimTypes.UserId, user.Id),
              new Claim(JwtKeinClaimTypes.CreatedTime,user.CreatedTime!=null?user.CreatedTime.Value.ToString("yyyy-MM-dd"):""),
               new Claim(JwtKeinClaimTypes.Phone, user.Phone),
               new Claim(JwtKeinClaimTypes.Name, user.Name??""),
                new Claim(JwtKeinClaimTypes.TenantId, user.TenantId),
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
                GlobalServices.ServiceProvider.GetService<ICacheService>().SetString("CacheUserList"+user.Name, JsonHelper.ObjectToJSON(user));
            });
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
            });
        }
    }
}
