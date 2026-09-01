using LMS.Api.DTOs.Auth;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Services.Implementations.Auth;
using LMS.Api.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LMS.Api.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = CreateUserManagerMock();
        _authService = new AuthService(_userManagerMock.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var loginDto = new LoginDto
        {
            Email = "missing@example.com",
            Password = "Password123!"
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        User? result = await _authService.AuthenticateAsync(loginDto);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(UserStatus.Inactive)]
    [InlineData(UserStatus.Suspended)]
    public async Task AuthenticateAsync_WhenUserIsNotActive_ReturnsNull(
        UserStatus status)
    {
        var loginDto = new LoginDto
        {
            Email = "student@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Email = loginDto.Email,
            Status = status
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager =>
                manager.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        User? result = await _authService.AuthenticateAsync(loginDto);

        Assert.Null(result);

        // The status check runs after the password check so every failure costs the same.
        _userManagerMock.Verify(
            manager => manager.CheckPasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenPasswordIsInvalid_ReturnsNull()
    {
        var loginDto = new LoginDto
        {
            Email = "student@example.com",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Email = loginDto.Email,
            Status = UserStatus.Active
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager =>
                manager.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        User? result = await _authService.AuthenticateAsync(loginDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenCredentialsAreValid_ReturnsUser()
    {
        var loginDto = new LoginDto
        {
            Email = "student@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = loginDto.Email,
            Status = UserStatus.Active
        };

        _userManagerMock
            .Setup(manager => manager.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager =>
                manager.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        User? result = await _authService.AuthenticateAsync(loginDto);

        Assert.NotNull(result);
        Assert.Same(user, result);
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsUserRoles()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Status = UserStatus.Active
        };

        IList<string> roles = new List<string>
        {
            "Student"
        };

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(roles);

        IList<string> result =
            await _authService.GetRolesAsync(user);

        Assert.Single(result);
        Assert.Contains("Student", result);
    }

    [Fact]
    public async Task FindActiveUserByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        Guid userId = Guid.NewGuid();

        _userManagerMock
            .Setup(manager =>
                manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User?)null);

        User? result =
            await _authService.FindActiveUserByIdAsync(userId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(UserStatus.Inactive)]
    [InlineData(UserStatus.Suspended)]
    public async Task FindActiveUserByIdAsync_WhenUserIsNotActive_ReturnsNull(
        UserStatus status)
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Status = status
        };

        _userManagerMock
            .Setup(manager =>
                manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        User? result =
            await _authService.FindActiveUserByIdAsync(userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task FindActiveUserByIdAsync_WhenUserIsActive_ReturnsUser()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Status = UserStatus.Active
        };

        _userManagerMock
            .Setup(manager =>
                manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        User? result =
            await _authService.FindActiveUserByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Same(user, result);
    }

    private static Mock<UserManager<User>> CreateUserManagerMock()
    {
        var userStoreMock = new Mock<IUserStore<User>>();

        return new Mock<UserManager<User>>(
            userStoreMock.Object,
            null!,
            new PasswordHasher<User>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );
    }
}
