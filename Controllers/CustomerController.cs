using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SELBN.Data;

namespace SELBN.Controllers;

[Authorize(Roles = AppRoles.Customer)]
public class CustomerController : Controller
{
    [HttpGet("/customer")]
    public IActionResult Index() => View();
}