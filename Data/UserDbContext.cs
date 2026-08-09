using MySql.Data.MySqlClient;

namespace LibraryWebAPI.Data
{
    public class UserDbContext(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}