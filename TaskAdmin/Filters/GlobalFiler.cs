using Microsoft.AspNetCore.Mvc.Filters;
using System; 

namespace TaskAdmin.Filters
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
