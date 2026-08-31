namespace LMS.Api.Services.Interfaces.Auth;

public interface IAuthCookieService
{
    void SetRefreshTokenCookie(HttpResponse response, string refreshToken);

    void DeleteRefreshTokenCookie(HttpResponse response);
}
