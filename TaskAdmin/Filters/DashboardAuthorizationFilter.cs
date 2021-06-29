using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TaskAdmin.Filters
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (!string.IsNullOrEmpty(httpContext.Session.GetString("userId")))
            {
                //成功登录
                return true;
            }
            else
            {
                //阻断跳转原先的请求信息到登录页


                //302跳转到登录页面 
                httpContext.Response.Redirect("/User/");

                return true;
            }

        }
    }
}
