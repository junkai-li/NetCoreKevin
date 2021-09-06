
using Common;
using Common.Json;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Repository;
using Repository.Database;
using System;
using System.Collections.Generic;
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
                case "UserClient":
                    userDto User = null;
                    //var Password = Crypto.GetEncryptMD5(context.Password);
                    var Password = context.Password;
                    //查询数据库 
                    dataJson = RedisHelper.HashGet("CacheUserList", context.UserName);

                    if (!string.IsNullOrEmpty(dataJson))
                    {
                        User = JsonHelper.JSONToObject<userDto>(dataJson);
                    }
                    else
                    {
                        using (var db =new dbContext())
                        {
                            User = db.TUser.Where(x => x.IsDelete == false && (x.Name == context.UserName || x.Phone == context.UserName) && x.PassWord == Password).Select(x=> new userDto {
                                Id=x.Id.ToString(),
                                Phone=x.Phone,
                                CreatedTime=x.CreateTime,
                                Password=x.PassWord,
                                Name=x.Name
                            }).FirstOrDefault();
                        }
                      
                    }
                    if (User != null && User != default && User.Password == Password)
                    {
                        context.Result = new GrantValidationResult(
                         subject: context.UserName,
                         claims: GetUserClaims(User),
                         authenticationMethod: "custom"
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
                case "UMUserClient":
                    uMClientUserDto uMUser = null;
                    //查询数据库
                    dataJson = RedisHelper.HashGet("CacheClientUserList", context.UserName);

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
                                Name = x.Name
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
            new Claim("userid", user.Id),
              new Claim("createdTime",user.CreatedTime!=null?user.CreatedTime.Value.ToString("yyyy-MM-dd"):""),
               new Claim("phone", user.Phone),
               new Claim("name", user.Name??""), 
            };
        }

        /// <summary> 
        //异步保存缓存
        /// </summary>
        /// <returns></returns>
        private async Task CacheUserListAsync(userDto user)
        {
            RedisHelper.HashSet("CacheUserList", user.Name, JsonHelper.ObjectToJSON(user));

        }

        private Claim[] GetUserClaims(uMClientUserDto user)
        {
            return new Claim[]
            {
            new Claim("userid", user.Id),
                   new Claim("name", user.Name??""),
                        new Claim("createdTime",user.CreatedTime!=null?user.CreatedTime.Value.ToString("yyyy-MM-dd"):"")
            };
        }

        /// <summary> 
        //异步保存缓存
        /// </summary>
        /// <returns></returns>
        private async Task CacheUserListAsync(uMClientUserDto user)
        {
            RedisHelper.HashSet("CacheClientUserList", user.Id, JsonHelper.ObjectToJSON(user));

        }
    }
}
