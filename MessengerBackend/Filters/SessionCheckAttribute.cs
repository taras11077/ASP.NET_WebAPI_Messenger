using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessengerBackend.Filters;

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();

        if (controller == "User" && (action == "Login" || action == "Register"))
        {
            base.OnActionExecuting(context);
            return;
        }

        if (context.HttpContext.Session.GetString("user") == null)
            context.Result = new RedirectToActionResult("Login", "User", null);
        else
            base.OnActionExecuting(context);
    }
}