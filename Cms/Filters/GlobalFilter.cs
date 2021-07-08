using Microsoft.AspNetCore.Mvc.Filters;
using System; 

namespace Cms.Filters
{
    public class GlobalFilter : Attribute, IActionFilter
    {

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {

        }


        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
