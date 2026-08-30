using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Services.Implementations;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LMS.Api.Tests.Services.Users;

public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _userManagerMock = CreateUserManagerMock();
        _userService = new UserService(_userManagerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        User? result = await _userService.GetByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Same(user, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        Guid userId = Guid.NewGuid();

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User?)null);

        User? result = await _userService.GetByIdAsync(userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenUserCreationSucceeds_AddsRole()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            UserName = "test@example.com"
        };

        const string password = "Password123!";
        const string role = "Student";

        _userManagerMock
            .Setup(manager => manager.CreateAsync(user, password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.AddToRoleAsync(user, role))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result =
            await _userService.CreateAsync(user, password, role);

        Assert.True(result.Succeeded);

        _userManagerMock.Verify(
            manager => manager.CreateAsync(user, password),
            Times.Once);

        _userManagerMock.Verify(
            manager => manager.AddToRoleAsync(user, role),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenUserCreationFails_DoesNotAddRole()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com"
        };

        IdentityResult failedResult = IdentityResult.Failed(
            new IdentityError
            {
                Code = "CreateFailed",
                Description = "User could not be created."
            }
        );

        _userManagerMock
            .Setup(manager => manager.CreateAsync(user, "Password123!"))
            .ReturnsAsync(failedResult);

        IdentityResult result =
            await _userService.CreateAsync(
                user,
                "Password123!",
                "Student"
            );

        Assert.False(result.Succeeded);

        _userManagerMock.Verify(
            manager => manager.AddToRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenRoleAssignmentFails_DeletesCreatedUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com"
        };

        IdentityResult roleFailure = IdentityResult.Failed(
            new IdentityError
            {
                Code = "RoleFailed",
                Description = "Role could not be assigned."
            }
        );

        _userManagerMock
            .Setup(manager => manager.CreateAsync(user, "Password123!"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.AddToRoleAsync(user, "Student"))
            .ReturnsAsync(roleFailure);

        _userManagerMock
            .Setup(manager => manager.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result =
            await _userService.CreateAsync(
                user,
                "Password123!",
                "Student"
            );

        Assert.False(result.Succeeded);

        _userManagerMock.Verify(
            manager => manager.DeleteAsync(user),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserExists_UpdatesUser()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "old@example.com",
            UserName = "old@example.com",
            Status = UserStatus.Active
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result = await _userService.UpdateAsync(
            userId,
            "New Name",
            "new@example.com",
            UserStatus.Inactive,
            null
        );

        Assert.True(result.Succeeded);
        Assert.Equal("New Name", user.Name);
        Assert.Equal("new@example.com", user.Email);
        Assert.Equal("new@example.com", user.UserName);
        Assert.Equal(UserStatus.Inactive, user.Status);

        _userManagerMock.Verify(
            manager => manager.UpdateAsync(user),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenOptionalValuesAreNull_KeepsExistingValues()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Existing Name",
            Email = "existing@example.com",
            UserName = "existing@example.com",
            Status = UserStatus.Active
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result = await _userService.UpdateAsync(
            userId,
            null,
            null,
            UserStatus.Active,
            null
        );

        Assert.True(result.Succeeded);
        Assert.Equal("Existing Name", user.Name);
        Assert.Equal("existing@example.com", user.Email);
        Assert.Equal("existing@example.com", user.UserName);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserDoesNotExist_ReturnsFailedResult()
    {
        Guid userId = Guid.NewGuid();

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User?)null);

        IdentityResult result = await _userService.UpdateAsync(
            userId,
            "New Name",
            "new@example.com",
            UserStatus.Active,
            null
        );

        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "UserNotFound"
        );

        _userManagerMock.Verify(
            manager => manager.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleIsValid_UpdatesRole()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            UserName = "test@example.com",
            Status = UserStatus.Active
        };

        IList<string> currentRoles = ["Student"];

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);

        _userManagerMock
            .Setup(manager => manager.RemoveFromRolesAsync(user, currentRoles))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.AddToRoleAsync(user, "Teacher"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(manager => manager.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result = await _userService.UpdateAsync(
            userId,
            null,
            null,
            UserStatus.Active,
            "Teacher"
        );

        Assert.True(result.Succeeded);

        _userManagerMock.Verify(
            manager => manager.RemoveFromRolesAsync(user, currentRoles),
            Times.Once);

        _userManagerMock.Verify(
            manager => manager.AddToRoleAsync(user, "Teacher"),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleIsInvalid_ReturnsFailedResult()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            Status = UserStatus.Active
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        IdentityResult result = await _userService.UpdateAsync(
            userId,
            null,
            null,
            UserStatus.Active,
            "Administrator"
        );

        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "InvalidRole"
        );

        _userManagerMock.Verify(
            manager => manager.AddToRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);

        _userManagerMock.Verify(
            manager => manager.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserAlreadyHasRole_DoesNotChangeRole()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Status = UserStatus.Active
        };

        IList<string> currentRoles = ["Student"];

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);

        _userManagerMock
            .Setup(manager => manager.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result = await _userService.UpdateAsync(
            userId,
            null,
            null,
            UserStatus.Active,
            "Student"
        );

        Assert.True(result.Succeeded);

        _userManagerMock.Verify(
            manager => manager.RemoveFromRolesAsync(
                It.IsAny<User>(),
                It.IsAny<IEnumerable<string>>()),
            Times.Never);

        _userManagerMock.Verify(
            manager => manager.AddToRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenUserExists_UpdatesStatus()
    {
        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Status = UserStatus.Active
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result =
            await _userService.UpdateStatusAsync(
                userId,
                UserStatus.Suspended
            );

        Assert.True(result.Succeeded);
        Assert.Equal(UserStatus.Suspended, user.Status);

        _userManagerMock.Verify(
            manager => manager.UpdateAsync(user),
            Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenUserDoesNotExist_ReturnsFailedResult()
    {
        Guid userId = Guid.NewGuid();

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User?)null);

        IdentityResult result =
            await _userService.UpdateStatusAsync(
                userId,
                UserStatus.Inactive
            );

        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "UserNotFound"
        );

        _userManagerMock.Verify(
            manager => manager.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }

    private static Mock<UserManager<User>> CreateUserManagerMock()
    {
        var userStoreMock = new Mock<IUserStore<User>>();

        return new Mock<UserManager<User>>(
            userStoreMock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );
    }
}
