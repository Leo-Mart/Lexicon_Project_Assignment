using LMS.Api.Services.Implementations.Auth;
using LMS.Api.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Http;

namespace LMS.Api.Tests.Services.Auth;

public class AuthCookieServiceTests
{
    private readonly IAuthCookieService _authCookieService;

    public AuthCookieServiceTests()
    {
        _authCookieService = new AuthCookieService();
    }

    [Fact]
    public void SetRefreshTokenCookie_SetsRefreshTokenCookie()
    {
        var context = new DefaultHttpContext();
        HttpResponse response = context.Response;

        const string refreshToken = "test-refresh-token";

        _authCookieService.SetRefreshTokenCookie(
            response,
            refreshToken
        );

        string setCookieHeader =
            response.Headers["Set-Cookie"].ToString();

        Assert.Contains(
            $"refreshToken={refreshToken}",
            setCookieHeader
        );
    }

    [Fact]
    public void SetRefreshTokenCookie_SetsSecureCookieOptions()
    {
        var context = new DefaultHttpContext();
        HttpResponse response = context.Response;

        _authCookieService.SetRefreshTokenCookie(
            response,
            "test-refresh-token"
        );

        string setCookieHeader =
            response.Headers["Set-Cookie"].ToString();

        Assert.Contains("httponly", setCookieHeader.ToLowerInvariant());
        Assert.Contains("secure", setCookieHeader.ToLowerInvariant());
        Assert.Contains("samesite=strict", setCookieHeader.ToLowerInvariant());
        Assert.Contains("path=/api/auth", setCookieHeader.ToLowerInvariant());
        Assert.Contains("expires=", setCookieHeader.ToLowerInvariant());
    }

    [Fact]
    public void DeleteRefreshTokenCookie_DeletesRefreshTokenCookie()
    {
        var context = new DefaultHttpContext();
        HttpResponse response = context.Response;

        _authCookieService.DeleteRefreshTokenCookie(response);

        string setCookieHeader =
            response.Headers["Set-Cookie"].ToString();

        Assert.Contains("refreshToken=", setCookieHeader);
        Assert.Contains("path=/api/auth", setCookieHeader.ToLowerInvariant());
        Assert.Contains("secure", setCookieHeader.ToLowerInvariant());
        Assert.Contains("samesite=strict", setCookieHeader.ToLowerInvariant());
    }
}
