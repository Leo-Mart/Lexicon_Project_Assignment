using System.Collections.Concurrent;
using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.DTOs.Auth;
using LMS.Api.Models;
using LMS.Api.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IAuthCookieService _authCookieService;

    private static readonly ConcurrentDictionary<string, (Guid UserId, DateTime ExpiresAt)> RefreshTokens = new();

    public AuthController(
        IAuthService authService,
        ITokenService tokenService,
        IAuthCookieService authCookieService)
    {
        _authService = authService;
        _tokenService = tokenService;
        _authCookieService = authCookieService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("LoginLimit")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        User? user = await _authService.AuthenticateAsync(request);

        if (user is null)
        {
            return Unauthorized();
        }

        IList<string> roles = await _authService.GetRolesAsync(user);

        string accessToken = _tokenService.GenerateAccessToken(user, roles);
        string refreshToken = _tokenService.GenerateRefreshToken();

        RefreshTokens[refreshToken] = (
            user.Id,
            DateTime.UtcNow.AddDays(JwtConstants.RefreshTokenExpirationDays)
        );

        _authCookieService.SetRefreshTokenCookie(Response, refreshToken);

        return Ok(new { AccessToken = accessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        string? currentRefreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(currentRefreshToken))
        {
            return Unauthorized();
        }

        if (!RefreshTokens.TryGetValue(currentRefreshToken, out var storedToken))
        {
            return Unauthorized();
        }

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            RefreshTokens.TryRemove(currentRefreshToken, out _);
            _authCookieService.DeleteRefreshTokenCookie(Response);

            return Unauthorized();
        }

        User? user = await _authService.FindActiveUserByIdAsync(storedToken.UserId);

        if (user is null)
        {
            RefreshTokens.TryRemove(currentRefreshToken, out _);
            _authCookieService.DeleteRefreshTokenCookie(Response);

            return Unauthorized();
        }

        IList<string> roles = await _authService.GetRolesAsync(user);

        RefreshTokens.TryRemove(currentRefreshToken, out _);

        string newAccessToken = _tokenService.GenerateAccessToken(user, roles);
        string newRefreshToken = _tokenService.GenerateRefreshToken();

        RefreshTokens[newRefreshToken] = (
            user.Id,
            DateTime.UtcNow.AddDays(JwtConstants.RefreshTokenExpirationDays)
        );

        _authCookieService.SetRefreshTokenCookie(Response, newRefreshToken);

        return Ok(new { AccessToken = newAccessToken });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        string? refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            RefreshTokens.TryRemove(refreshToken, out _);
        }

        _authCookieService.DeleteRefreshTokenCookie(Response);

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Name = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Roles = User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToArray()
        });
    }
}
