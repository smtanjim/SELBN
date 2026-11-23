using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using SELBN.Data;

namespace SELBN.Controllers;

public class AccountController : Controller
{
    private readonly DbHelper _db;
    private readonly PasswordHasher<string> _hasher = new();

    public AccountController(DbHelper db)
    {
        _db = db;
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return Redirect("/dashboard");

        ViewBag.Roles = AppRoles.AllRoles;
        return View();
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register(
        string name,
        string email,
        string password,
        string? role,
        string? phone,
        string? address)
    {
        ViewBag.Roles = AppRoles.AllRoles;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            TempData["FlashError"] = "Name, email, and password are required.";
            return View();
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var selectedRole = AppRoles.IsValid(role) ? role! : AppRoles.Customer;

        var existingCount = Convert.ToInt32(await _db.ExecuteScalarAsync(
            "SELECT COUNT(*) FROM users WHERE email = @email",
            new MySqlParameter("@email", normalizedEmail)));

        if (existingCount > 0)
        {
            TempData["FlashError"] = "An account with this email already exists.";
            return View();
        }

        var passwordHash = _hasher.HashPassword(normalizedEmail, password);

        var newIdObj = await _db.ExecuteScalarAsync(
            @"INSERT INTO users (name, email, password_hash, role, phone, address)
              VALUES (@name, @email, @hash, @role, @phone, @address);
              SELECT LAST_INSERT_ID();",
            new MySqlParameter("@name", name.Trim()),
            new MySqlParameter("@email", normalizedEmail),
            new MySqlParameter("@hash", passwordHash),
            new MySqlParameter("@role", selectedRole),
            new MySqlParameter("@phone", (object?)phone?.Trim() ?? DBNull.Value),
            new MySqlParameter("@address", (object?)address?.Trim() ?? DBNull.Value));

        var userId = Convert.ToInt32(newIdObj);
        await SignInUserAsync(userId, name.Trim(), normalizedEmail, selectedRole);

        TempData["FlashSuccess"] = "Registration successful.";
        return Redirect(AppRoles.GetDashboardUrl(selectedRole));
    }

    [HttpGet("/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return Redirect("/dashboard");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        ViewBag.Email = email;
        ViewBag.ReturnUrl = returnUrl;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            TempData["FlashError"] = "Email and password are required.";
            return View();
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var record = await _db.QuerySingleAsync(
            "SELECT id, name, email, password_hash, role FROM users WHERE email = @email LIMIT 1",
            reader => new
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Email = reader.GetString("email"),
                PasswordHash = reader.GetString("password_hash"),
                Role = reader.GetString("role")
            },
            new MySqlParameter("@email", normalizedEmail));

        if (record == null ||
            _hasher.VerifyHashedPassword(normalizedEmail, record.PasswordHash, password) == PasswordVerificationResult.Failed)
        {
            TempData["FlashError"] = "Invalid email or password.";
            return View();
        }

        await SignInUserAsync(record.Id, record.Name, record.Email, record.Role);
        TempData["FlashSuccess"] = $"Welcome back, {record.Name}!";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return Redirect(AppRoles.GetDashboardUrl(record.Role));
    }

    [HttpGet("/logout")]
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["FlashInfo"] = "You have been logged out.";
        return Redirect("/login");
    }

    private async Task SignInUserAsync(int id, string name, string email, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });
    }
}