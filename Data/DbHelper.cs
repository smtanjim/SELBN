using MySqlConnector;

namespace SELBN.Data;

public class DbHelper
{
    private readonly string _connectionString;

    public DbHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string 'DefaultConnection' is missing.");
    }

    public MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<int> ExecuteNonQueryAsync(string sql, params MySqlParameter[] parameters)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<object?> ExecuteScalarAsync(string sql, params MySqlParameter[] parameters)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        return await cmd.ExecuteScalarAsync();
    }

    public async Task<T?> QuerySingleAsync<T>(string sql, Func<MySqlDataReader, T> mapper, params MySqlParameter[] parameters)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? mapper(reader) : default;
    }
}