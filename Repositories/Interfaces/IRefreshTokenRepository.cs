using article.Models;

namespace article.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {

        Task SaveAsync(int userId, string tokenHash, DateTime expiresAt, string? ip);

        Task<RefreshToken?>GetAsync(string tokenHash);
        Task RevokeAsync(string TokenHash, string? ip, string? replacedByHash);


        Task RevokeAllForUserAsync(int userId, string? ip);
    }
}
