using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SELBN.Data;

namespace SELBN.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Landing()
    {
        return View();
    }

    [Authorize]
    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        return Redirect(AppRoles.GetDashboardUrl(role));
    }

    [HttpGet("/error/{code:int}")]
    public IActionResult Error(int code)
    {
        ViewBag.Code = code;
        ViewBag.Message = code switch
        {
            403 => "Access Denied: Your role does not have permission to view this page.",
            404 => "The requested page could not be found.",
            _ => "An unexpected error occurred."
        };
        return View();
    }
}