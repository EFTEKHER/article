
using article.Models;

namespace article.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        (string token, string tokenHash) CreateRefreshToken();

        string Hash(string token);
        DateTime AccessTokenExpiry();
        DateTime RefreshTokenExpiry();
    }
}
