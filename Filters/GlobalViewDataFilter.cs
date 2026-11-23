using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SELBN.Data;

namespace SELBN.Filters;

public class GlobalViewDataFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is not Controller controller) return;

        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var role = user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            controller.ViewBag.CurrentUser = new
            {
                Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
                Name = user.FindFirstValue(ClaimTypes.Name),
                Email = user.FindFirstValue(ClaimTypes.Email),
                Role = role
            };
            controller.ViewBag.RoleLabel = AppRoles.GetLabel(role);
        }
        else
        {
            controller.ViewBag.CurrentUser = null;
            controller.ViewBag.RoleLabel = "Guest";
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}