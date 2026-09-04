using System.IdentityModel.Tokens.Jwt;
using LMS.Api.Models;
using LMS.Api.Services.Implementations.Auth;
using LMS.Api.Services.Interfaces.Auth;
using Microsoft.Extensions.Configuration;

namespace LMS.Api.Tests.Services.Auth;

public class TokenServiceTests
{
    private readonly ITokenService _tokenService;

    public TokenServiceTests()
    {
        Dictionary<string, string?> settings = new()
        {
            ["Jwt:Key"] = "ThisIsATestSecretKeyThatIsLongEnough123456789",
            ["Jwt:Issuer"] = "LMS.Api.Tests",
            ["Jwt:Audience"] = "LMS.Client.Tests"
        };

        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

        _tokenService = new TokenService(configuration);
    }

    [Fact]
    public void GenerateAccessToken_WithValidUser_ReturnsJwtToken()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com"
        };

        IList<string> roles = ["Student"];

        string result = _tokenService.GenerateAccessToken(user, roles);

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        var tokenHandler = new JwtSecurityTokenHandler();

        Assert.True(tokenHandler.CanReadToken(result));
    }

    [Fact]
    public void GenerateAccessToken_ContainsUserInformation()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        IList<string> roles = ["Student"];

        string token = _tokenService.GenerateAccessToken(user, roles);

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Contains(jwtToken.Claims, claim => claim.Value == userId.ToString());

        Assert.Contains(jwtToken.Claims, claim => claim.Value == user.Name);

        Assert.Contains(jwtToken.Claims, claim => claim.Value == user.Email);
    }

    [Fact]
    public void GenerateAccessToken_ContainsRoles()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test Teacher",
            Email = "teacher@example.com"
        };

        IList<string> roles =
        [
            "Teacher",
            "Administrator"
        ];

        string token = _tokenService.GenerateAccessToken(user, roles);

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Contains(jwtToken.Claims, claim => claim.Value == "Teacher");

        Assert.Contains(jwtToken.Claims, claim => claim.Value == "Administrator");
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectIssuerAndAudience()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com"
        };

        string token =
            _tokenService.GenerateAccessToken(
                user,
                ["Student"]
            );

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Equal("LMS.Api.Tests", jwtToken.Issuer);

        Assert.Contains("LMS.Client.Tests", jwtToken.Audiences);
    }

    [Fact]
    public void GenerateAccessToken_WhenEmailIsMissing_ThrowsException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = null
        };

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => _tokenService.GenerateAccessToken(
                    user,
                    ["Student"]
                )
            );

        Assert.Equal("User email is missing.", exception.Message);
    }

    [Fact]
    public void GenerateAccessToken_WhenJwtKeyIsMissing_ThrowsException()
    {
        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Jwt:Issuer"] = "LMS.Api.Tests",
                        ["Jwt:Audience"] = "LMS.Client.Tests"
                    }
                )
                .Build();

        ITokenService tokenService = new TokenService(configuration);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com"
        };

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => tokenService.GenerateAccessToken(
                    user,
                    ["Student"]
                )
            );

        Assert.Equal("JWT key is missing.", exception.Message);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsValidBase64Token()
    {
        string refreshToken = _tokenService.GenerateRefreshToken();

        byte[] tokenBytes = Convert.FromBase64String(refreshToken);

        Assert.Equal(64, tokenBytes.Length);
    }

    [Fact]
    public void GenerateRefreshToken_GeneratesUniqueTokens()
    {
        string firstToken = _tokenService.GenerateRefreshToken();

        string secondToken = _tokenService.GenerateRefreshToken();

        Assert.NotEqual(firstToken, secondToken);
    }
}
