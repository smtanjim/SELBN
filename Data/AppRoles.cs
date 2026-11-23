namespace SELBN.Data;

public static class AppRoles
{
    public const string Customer = "customer";
    public const string Supplier = "supplier";
    public const string SalesExecutive = "sales_executive";
    public const string AccountsOfficer = "accounts_officer";
    public const string Admin = "admin";

    public static readonly Dictionary<string, string> AllRoles = new()
    {
        [Customer] = "Customer",
        [Supplier] = "Supplier",
        [SalesExecutive] = "Sales Executive",
        [AccountsOfficer] = "Accounts Officer",
        [Admin] = "Administrator"
    };

    public static bool IsValid(string? role) =>
        !string.IsNullOrWhiteSpace(role) && AllRoles.ContainsKey(role);

    public static string GetLabel(string? role) =>
        role != null && AllRoles.TryGetValue(role, out var label) ? label : "Guest";

    public static string GetDashboardUrl(string? role) => role switch
    {
        Customer => "/customer",
        Supplier => "/supplier",
        SalesExecutive => "/sales",
        AccountsOfficer => "/accounts",
        Admin => "/admin",
        _ => "/"
    };
}