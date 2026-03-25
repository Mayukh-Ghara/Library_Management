using System.Data;
using System.Data.Common;
using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using MySql.Data.MySqlClient;

namespace LibraryWebAPI.Services
{
    public class UserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<User> CreateUser(User user)
        {
            using var conn = _context.CreateConnection();
            await conn.OpenAsync();

            string query = @"INSERT INTO users 
                                (username, email, password_hash, first_name, last_name, phone, is_active, role)
                             VALUES 
                                (@username, @email, @passwordHash, @firstName, @lastName, @phone, @isActive, @role);
                             SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
            cmd.Parameters.AddWithValue("@lastName", user.LastName);
            cmd.Parameters.AddWithValue("@phone", user.Phone);
            cmd.Parameters.AddWithValue("@isActive", user.IsActive);
            cmd.Parameters.AddWithValue("@role", user.Role);

            var newId = await cmd.ExecuteScalarAsync();
            user.Id = Convert.ToInt32(newId);
            return user;
        }

        // READ - Get All Users
        public async Task<List<User>> GetAllUsers()
        {
            var users = new List<User>();

            using var conn = _context.CreateConnection();
            await conn.OpenAsync();

            string query = "SELECT * FROM users";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                users.Add(MapUser(reader));

            return users;
        }

        // READ - Get User By ID
        public async Task<User?> GetUserById(int id)
        {
            using var conn = _context.CreateConnection();
            await conn.OpenAsync();

            string query = "SELECT * FROM users WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapUser(reader);

            return null;
        }

        // UPDATE
        public async Task<User?> UpdateUser(int id, User updatedUser)
        {
            using var conn = _context.CreateConnection();
            await conn.OpenAsync();

            string query = @"UPDATE users SET
                                username   = @username,
                                email      = @email,
                                first_name = @firstName,
                                last_name  = @lastName,
                                phone      = @phone,
                                is_active  = @isActive,
                                role       = @role
                             WHERE id = @id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@username", updatedUser.Username);
            cmd.Parameters.AddWithValue("@email", updatedUser.Email);
            cmd.Parameters.AddWithValue("@firstName", updatedUser.FirstName);
            cmd.Parameters.AddWithValue("@lastName", updatedUser.LastName);
            cmd.Parameters.AddWithValue("@phone", updatedUser.Phone);
            cmd.Parameters.AddWithValue("@isActive", updatedUser.IsActive);
            cmd.Parameters.AddWithValue("@role", updatedUser.Role);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected == 0) return null;

            updatedUser.Id = id;
            return updatedUser;
        }

        // DELETE
        public async Task<bool> DeleteUser(int id)
        {
            using var conn = _context.CreateConnection();
            await conn.OpenAsync();

            string query = "DELETE FROM users WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // ── Helper: Map reader row → User object ──────────────────────────
        private User MapUser(DbDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32("id"),
                Username = reader.GetString("username"),
                Email = reader.GetString("email"),
                PasswordHash = reader.GetString("password_hash"),
                FirstName = reader.IsDBNull(reader.GetOrdinal("first_name")) ? null : reader.GetString("first_name"),
                LastName = reader.IsDBNull(reader.GetOrdinal("last_name")) ? null : reader.GetString("last_name"),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
                IsActive = reader.GetBoolean("is_active"),
                Role = reader.GetString("role")
            };
        }
    }
}