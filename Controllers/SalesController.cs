using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SELBN.Data;

namespace SELBN.Controllers;

[Authorize(Roles = AppRoles.SalesExecutive)]
public class SalesController : Controller
{
    [HttpGet("/sales")]
    public IActionResult Index() => View();
}