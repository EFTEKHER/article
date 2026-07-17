using article.Models;
using article.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace article.Repositories
{
    public class RefreshTokenRepository:IRefreshTokenRepository
    {

        private readonly string _cs;

        public RefreshTokenRepository(IConfiguration configuration)
        {
            _cs = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
        }

        public async Task SaveAsync(int userId, string refreshToken, DateTime expiresAt,string? Ip)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_SaveRefreshToken", con)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RefreshToken", refreshToken);
            cmd.Parameters.AddWithValue("@ExpiresAt", expiresAt);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    public async Task<RefreshToken> GetAsync(string tokenHash)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_GetRefreshToken", con)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TokenHash", tokenHash);
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RefreshToken
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    TokenHash = reader.GetString(reader.GetOrdinal("TokenHash")),
                    ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    CreatedByIp = reader.IsDBNull(reader.GetOrdinal("CreatedByIp")) ? null : reader.GetString(reader.GetOrdinal("CreatedByIp")),
                    RevokedAt = reader.IsDBNull(reader.GetOrdinal("RevokedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("RevokedAt")),
                    RevokedByIp = reader.IsDBNull(reader.GetOrdinal("RevokedByIp")) ? null : reader.GetString(reader.GetOrdinal("RevokedByIp")),
                    ReplacedByHash = reader.IsDBNull(reader.GetOrdinal("ReplacedByHash")) ? null : reader.GetString(reader.GetOrdinal("ReplacedByHash"))
                };
            }
            else
            {
                return null;
            }
        }

        public async Task RevokeAsync(string tokenHash, string? revokedByIp, string? replacedByHash)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_RevokeRefreshToken", con)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TokenHash", tokenHash);
            cmd.Parameters.AddWithValue("@RevokedByIp", (object?)revokedByIp ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReplacedByHash", (object?)replacedByHash ?? DBNull.Value);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RevokeAllForUserAsync(int userId, string? revokedByIp)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_RevokeAllRefreshTokensForUser", con)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RevokedByIp", (object?)revokedByIp ?? DBNull.Value);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

    }
}
