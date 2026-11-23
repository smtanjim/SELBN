using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SELBN.Data;

namespace SELBN.Controllers;

[Authorize(Roles = AppRoles.AccountsOfficer)]
public class AccountsController : Controller
{
    [HttpGet("/accounts")]
    public IActionResult Index() => View();
}