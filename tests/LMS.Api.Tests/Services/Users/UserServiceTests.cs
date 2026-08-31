using AutoMapper;
using LMS.Api.DTOs.Users;
using LMS.Api.Enums.Model;
using LMS.Api.Mappings;
using LMS.Api.Models;
using LMS.Api.Services.Implementations;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LMS.Api.Tests.Services.Users;

public class UserServiceTests
{
    // No UserProfile test lives here yet. When one is added, be aware
    // AssertConfigurationIsValid() will likely fail on the create map as it
    // stands: User inherits PasswordHash, SecurityStamp, NormalizedEmail and
    // the rest from IdentityUser<Guid>, and AutoMapper counts every one as an
    // unmapped destination member. Making it pass means an explicit ignore
    // list or ForAllOtherMembers. A narrower test that maps a UserCreateDto
    // and asserts UserName == Email, Status == Active and Id == Guid.Empty
    // covers the same ground without that.
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _userManagerMock = CreateUserManagerMock();

        // A real mapper, not a mock: UpdateUserAsync delegates its merge rules
        // to UserProfile, so a stubbed IMapper would leave these assertions
        // testing nothing.
        IMapper mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<UserProfile>(),
            NullLoggerFactory.Instance
        ).CreateMapper();

        _userService = new UserService(_userManagerMock.Object, mapper);
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

        User? result = await _userService.GetUserByIdAsync(userId);

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

        User? result = await _userService.GetUserByIdAsync(userId);

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
            await _userService.CreateUserAsync(user, password, role);

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
            await _userService.CreateUserAsync(
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
            await _userService.CreateUserAsync(
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

        IdentityResult result = await _userService.UpdateUserAsync(
            userId,
            new UserUpdateDto
            {
                Name = "New Name",
                Email = "new@example.com",
                Status = UserStatus.Inactive,
                Role = null
            }
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

        IdentityResult result = await _userService.UpdateUserAsync(
            userId,
            new UserUpdateDto
            {
                Name = null,
                Email = null,
                Status = UserStatus.Active,
                Role = null
            }
        );

        Assert.True(result.Succeeded);
        Assert.Equal("Existing Name", user.Name);
        Assert.Equal("existing@example.com", user.Email);
        Assert.Equal("existing@example.com", user.UserName);
    }

    [Fact]
    public async Task UpdateAsync_WhenStatusIsNull_KeepsExistingStatus()
    {
        Guid userId = Guid.NewGuid();

        // Suspended rather than Active so the assertion still fails if the
        // PreCondition is dropped: an unguarded map turns a null UserStatus?
        // into (UserStatus)0, which is not a declared member.
        var user = new User
        {
            Id = userId,
            Name = "Existing Name",
            Email = "existing@example.com",
            UserName = "existing@example.com",
            Status = UserStatus.Suspended
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        IdentityResult result = await _userService.UpdateUserAsync(
            userId,
            new UserUpdateDto
            {
                Name = "New Name",
                Email = null,
                Status = null,
                Role = null
            }
        );

        Assert.True(result.Succeeded);
        Assert.Equal(UserStatus.Suspended, user.Status);
        Assert.Equal("New Name", user.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserDoesNotExist_ReturnsFailedResult()
    {
        Guid userId = Guid.NewGuid();

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User?)null);

        IdentityResult result = await _userService.UpdateUserAsync(
            userId,
            new UserUpdateDto
            {
                Name = "New Name",
                Email = "new@example.com",
                Status = UserStatus.Active,
                Role = null
            }
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

        IdentityResult result = await _userService.UpdateUserAsync(
            userId,
            new UserUpdateDto
            {
                Name = null,
                Email = null,
                Status = UserStatus.Active,
                Role = "Teacher"
            }
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

        IdentityResult result = await _userService.UpdateUserAsync(
            userId,
            new UserUpdateDto
            {
                Name = null,
                Email = null,
                Status = UserStatus.Active,
                Role = "Administrator"
            }
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

        IdentityResult result = await _userService.UpdateUserAsync(
            userId,
            new UserUpdateDto
            {
                Name = null,
                Email = null,
                Status = UserStatus.Active,
                Role = "Student"
            }
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
            await _userService.UpdateUserStatusAsync(
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
            await _userService.UpdateUserStatusAsync(
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
