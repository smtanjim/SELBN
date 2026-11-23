using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SELBN.Data;

namespace SELBN.Controllers;

[Authorize(Roles = AppRoles.Supplier)]
public class SupplierController : Controller
{
    [HttpGet("/supplier")]
    public IActionResult Index() => View();
}