// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Duende.IdentityServer.Models; 
using Duende.IdentityServer;
using System.Collections.Generic;  
namespace AuthorizationService
{
    /// <summary>
    /// 配置类
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                    new IdentityResources.OpenId(), 
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResources.Phone(),
                    new IdentityResources.Email(),
                   };
       /// <summary>
       /// api配置
       /// </summary>
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("WebApi","My Yfc.User.Api"), 
            };
        /// <summary>
        /// 客户端配置
        /// </summary>
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                  new Client
                {
                    ClientId = "UserClient",
                    ClientName = "UserClient", 
                    AllowedGrantTypes = Duende.IdentityServer.Models.GrantTypes.ResourceOwnerPassword,
                    AlwaysIncludeUserClaimsInIdToken=true,
                      AllowOfflineAccess = true,//如果要获取refresh_tokens ,必须把AllowOfflineAccess设置为true
                    ClientSecrets = { new Secret("UserClientSecrets".Sha256()) },
                    AccessTokenLifetime=14000,// 20se /设置AccessToken过期时间
                      RefreshTokenExpiration = TokenExpiration.Sliding,//刷新令牌时，将刷新RefreshToken的生命周期。RefreshToken的总生命周期不会超过AbsoluteRefreshTokenLifetime。
                    AbsoluteRefreshTokenLifetime = 259200,//RefreshToken的最长生命周期, 
                    AllowedScopes = { "WebApi",
                            "offline_access", //如果要获取refresh_tokens ,必须在scopes中加上OfflineAccess
                            "openid",//如果要获取id_token,必须在scopes中加上OpenId和Profile，id_token需要通过refresh_tokens获取AccessToken的时候才能拿到（还未找到原因）
                           "profile"//如果要获取id_token,必须在scopes中加上OpenId和Profile
                          }
                },
                new Client
                {
                    ClientId = "UMUserClient",
                    ClientName = "UMUserClient",
                    AllowedGrantTypes = Duende.IdentityServer.Models.GrantTypes.ResourceOwnerPassword,
                    AlwaysIncludeUserClaimsInIdToken=true,
                      AllowOfflineAccess = true,//如果要获取refresh_tokens ,必须把AllowOfflineAccess设置为true
                    ClientSecrets = { new Secret("UMUserClientSecrets".Sha256()) },
                    AccessTokenLifetime=204000,// 20se /设置AccessToken过期时间
                      RefreshTokenExpiration = TokenExpiration.Sliding,//刷新令牌时，将刷新RefreshToken的生命周期。RefreshToken的总生命周期不会超过AbsoluteRefreshTokenLifetime。
                    AbsoluteRefreshTokenLifetime = 259200,//RefreshToken的最长生命周期, 
                    AllowedScopes = { "WebApi",
                            "offline_access", //如果要获取refresh_tokens ,必须在scopes中加上OfflineAccess
                            "openid",//如果要获取id_token,必须在scopes中加上OpenId和Profile，id_token需要通过refresh_tokens获取AccessToken的时候才能拿到（还未找到原因）
                           "profile"//如果要获取id_token,必须在scopes中加上OpenId和Profile
                          }
                },
                //// m2m client credentials flow client
                //new Client
                //{
                //    ClientId = "m2m.client",
                //    ClientName = "Client Credentials Client",

                //    AllowedGrantTypes = GrantTypes.ClientCredentials,
                //    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                //    AllowedScopes = { "scope1" }
                //},

                //// interactive client using code flow + pkce
                //new Client
                //{
                //    ClientId = "interactive",
                //    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                //    AllowedGrantTypes = GrantTypes.Code,

                //    RedirectUris = { "https://localhost:44300/signin-oidc" },
                //    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                //    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                //    AllowOfflineAccess = true,
                //    AllowedScopes = { "openid", "profile", "scope2" }
                //},
            };
    }
}