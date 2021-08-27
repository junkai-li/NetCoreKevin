using Web.Actions; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters; 
using Web.Actions;
using Web.Permisson.Attributes;

namespace Web.Filters
{
    public class PrivilegeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        { 

            var controller = context.Controller as ControllerBase;
            if (controller == null)
            {
                base.OnActionExecuting(context);
                return;
            } 
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;
            var isSkip = ad.MethodInfo.IsDefined(typeof(SkipAuthorityAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(SkipAuthorityAttribute), false);
            if (isSkip == true)
            {
                base.OnActionExecuting(context);
                return;
            }

            var area = context.RouteData.Values["area"];
            var PermissionId = $"{area}.{ad.ControllerName}.{ad.ActionName}";


            //如果是QuickDebug模式，或者Action或Controller上有AllRightsAttribute标记都不需要判断权限
            //如果用户登录信息为空，也不需要判断权限，BaseController中会对没有登录的用户做其他处理

            var isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);

            var isAllRights = ad.MethodInfo.IsDefined(typeof(AuthorizeAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AuthorizeAttribute), false);
            var isDebug = ad.MethodInfo.IsDefined(typeof(DebugOnlyAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(DebugOnlyAttribute), false); 
            if (isDebug)
            { 
                    if (controller is ControllerBase c)
                    {
                        context.Result = c.Content("Is Debug Only");
                    }
                    else if (controller is ControllerBase c2)
                    {
                        context.Result = c2.BadRequest("Is Debug Only");
                    } 
                return;
            }

            if (isPublic == true)
            {
                base.OnActionExecuting(context);
                return;
            }
            if (isAllRights)
            {
                bool canAccess = PermissionsAction.IsAccess(PermissionId);
                if (canAccess == false)
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new JsonResult(new { errMsg = "Access Forbidden" });
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
