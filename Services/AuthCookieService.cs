using article.Services;
using Microsoft.AspNetCore.DataProtection;

namespace ArticleApi.Services;

public class AuthCookieService : IAuthCookieService
{
    public const string AccessCookie = "__Host-access";
    public const string RefreshCookie = "__Host-refresh";

    private readonly IDataProtector _protector;

    public AuthCookieService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("ArticleApi.AuthCookies.v1");
    }

    public void SetAuthCookies(
        HttpResponse response,
        string accessToken,
        DateTime accessExpiry,
        string refreshToken,
        DateTime refreshExpiry)
    {
        response.Cookies.Append(
            AccessCookie,
            _protector.Protect(accessToken),
            Options(accessExpiry));

        response.Cookies.Append(
            RefreshCookie,
            _protector.Protect(refreshToken),
            Options(refreshExpiry));
    }

    public void ClearAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete(
            AccessCookie,
            Options(DateTime.UtcNow));

        response.Cookies.Delete(
            RefreshCookie,
            Options(DateTime.UtcNow));
    }

    public string? ReadAccessToken(HttpRequest request)
        => Unprotect(request, AccessCookie);

    public string? ReadRefreshToken(HttpRequest request)
        => Unprotect(request, RefreshCookie);

    private string? Unprotect(HttpRequest request, string name)
    {
        if (!request.Cookies.TryGetValue(name, out var value))
            return null;

        try
        {
            return _protector.Unprotect(value);
        }
        catch
        {
            return null;
        }
    }

    private static CookieOptions Options(DateTime expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
        Expires = expires,
        IsEssential = true
    };
}