using Microsoft.AspNetCore.Identity;
using MySqlConnector;

namespace SELBN.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<DbHelper>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

        try
        {
            var countObj = await db.ExecuteScalarAsync("SELECT COUNT(*) FROM users");
            if (Convert.ToInt32(countObj) > 0) return;

            var hasher = new PasswordHasher<string>();
            var defaultHash = hasher.HashPassword("seed", "Pass@123");

            var demoUsers = new[]
            {
                ("Demo Customer", "customer@selbn.com", AppRoles.Customer),
                ("Demo Supplier", "supplier@selbn.com", AppRoles.Supplier),
                ("Demo Sales Executive", "sales@selbn.com", AppRoles.SalesExecutive),
                ("Demo Accounts Officer", "accounts@selbn.com", AppRoles.AccountsOfficer),
                ("System Admin", "admin@selbn.com", AppRoles.Admin)
            };

            const string insertSql = @"
                INSERT INTO users (name, email, password_hash, role, phone, address)
                VALUES (@name, @email, @hash, @role, '01700000000', 'Dhaka, Bangladesh')";

            foreach (var (name, email, role) in demoUsers)
            {
                await db.ExecuteNonQueryAsync(
                    insertSql,
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@email", email),
                    new MySqlParameter("@hash", defaultHash),
                    new MySqlParameter("@role", role));
            }

            logger.LogInformation("Seeded 5 demo role accounts into SELBN database.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Skipped user seeding (ensure MySQL is running and schema.sql is imported).");
        }
    }
}