using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Global.Exceptions;

namespace Kevin.Web.Filters
{
    public class RequireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //获取验证失败的模型字段
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => e.Value.Errors.First().ErrorMessage)
                    .ToList(); 
                var str = string.Join("|", errors);  
                throw new UserFriendlyException("未通过数据验证：" + str);
            }

        }
    }
}
