namespace article.Services
{
    public interface IAuthCookieService
    {
        void SetAuthCookies(
        HttpResponse response,
        string accessToken,
        DateTime accessExpiry,
        string refreshToken,
        DateTime refreshExpiry);

        void ClearAuthCookies(HttpResponse response);

        string? ReadAccessToken(HttpRequest request);

        string? ReadRefreshToken(HttpRequest request);
    }
}
