using Dapper;
using LibraryWebAPI.Models;
using MySqlConnector;

namespace LibraryWebAPI.Services;

public class UserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    private const string BaseSelect = """
        SELECT
            id             AS Id,
            username       AS Username,
            email          AS Email,
            password_hash  AS PasswordHash,
            first_name     AS FirstName,
            last_name      AS LastName,
            phone          AS Phone,
            is_active      AS IsActive,
            role           AS Role
        FROM users
        """;

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<User>(BaseSelect);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var sql = $"{BaseSelect} WHERE id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var sql = $"{BaseSelect} WHERE email = @Email";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = """
            INSERT INTO users (username, email, password_hash, first_name, last_name, phone, is_active, role)
            VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @Phone, @IsActive, @Role);
            SELECT LAST_INSERT_ID();
            """;
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string sql = """
            UPDATE users
            SET
                username   = @Username,
                email      = @Email,
                first_name = @FirstName,
                last_name  = @LastName,
                phone      = @Phone,
                is_active  = @IsActive,
                role       = @Role
            WHERE id = @Id
            """;
        using var conn = CreateConnection();
        return await conn.ExecuteAsync(sql, user) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM users WHERE id = @Id";
        using var conn = CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}