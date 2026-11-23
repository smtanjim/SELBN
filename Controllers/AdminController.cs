using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SELBN.Data;

namespace SELBN.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    [HttpGet("/admin")]
    public IActionResult Index() => View();
}