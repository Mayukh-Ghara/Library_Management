using MySql.Data.MySqlClient;

namespace LibraryWebAPI.Data
{
    public class UserDbContext
    {
        private readonly string _connectionString;

        public UserDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}