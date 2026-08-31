using LMS.Api.Constants;
using LMS.Api.Services.Interfaces.Auth;
namespace LMS.Api.Services.Implementations.Auth;

public class AuthCookieService : IAuthCookieService
{
    private const string RefreshTokenCookieName = "refreshToken";

    public void SetRefreshTokenCookie(HttpResponse response, string refreshToken)
    {
        response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(
                    JwtConstants.RefreshTokenExpirationDays
                ),
                Path = "/api/auth"
            }
        );
    }

    public void DeleteRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth"
            }
        );
    }
}
