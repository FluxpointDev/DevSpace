using DevSpaceWeb.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevSpaceWeb.Extensions;

public class ControllerExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(new Response(500, context.Exception.Message));
            context.ExceptionHandled = true;
        }
    }
}