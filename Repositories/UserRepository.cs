using article.Models;

using article.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace article.Repositories
{
    public class UserRepository:IUserRepository
    {

        public readonly string _cs;

        public UserRepository(IConfiguration configuration)
        {
            _cs = configuration.GetConnectionString("DefaultConnection")?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");

        }

        public async Task<int> RegisterAsync(User user)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("sp_RegisterUser", con)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            await con.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public Async
    }
}
