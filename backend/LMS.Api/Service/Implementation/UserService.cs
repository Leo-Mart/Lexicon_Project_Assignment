using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userManager.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<IdentityResult> CreateAsync(
        User user,
        string password,
        string role)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        IdentityResult createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return createResult;
        }

        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return roleResult;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(
        Guid userId,
        string? name,
        string? email,
        UserStatus status)
    {
        User? user = await FindUserByIdAsync(userId);

        if (user is null)
        {
            return UserNotFoundResult();
        }

        if (name is not null)
        {
            user.Name = name;
        }

        if (email is not null)
        {
            user.Email = email;
            user.UserName = email;
        }

        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> UpdateStatusAsync(
        Guid userId,
        UserStatus status)
    {
        User? user = await FindUserByIdAsync(userId);

        if (user is null)
        {
            return UserNotFoundResult();
        }

        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;

        return await _userManager.UpdateAsync(user);
    }

    private async Task<User?> FindUserByIdAsync(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }
    private static IdentityResult UserNotFoundResult()
    {
        return IdentityResult.Failed(
            new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found."
            }
        );
    }
}
