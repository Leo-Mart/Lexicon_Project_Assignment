using AutoMapper;
using LMS.Api.Constants;
using LMS.Api.DTOs.Users;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userManager.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<IdentityResult> CreateUserAsync(
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

    public async Task<IdentityResult> UpdateUserStatusAsync(
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

    public async Task<IdentityResult> UpdateUserAsync(Guid userId, UserUpdateDto request)
    {
        User? user = await FindUserByIdAsync(userId);

        if (user is null)
        {
            return UserNotFoundResult();
        }

        // Merges onto the tracked entity. Which members a null skips is
        // declared by the PreConditions in UserProfile, not repeated here.
        // Role is not a User property, so the map cannot carry it - Identity
        // keeps it in AspNetUserRoles and it stays a UserManager call below.
        _mapper.Map(request, user);

        if (request.Role is not null)
        {
            IdentityResult roleResult = await UpdateRoleAsync(user, request.Role);

            if (!roleResult.Succeeded)
            {
                return roleResult;
            }
        }

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
    private async Task<IdentityResult> UpdateRoleAsync(User user, string role)
    {
        if (role != RoleConstants.Student &&
            role != RoleConstants.Teacher)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "InvalidRole",
                    Description = "Role must be Student or Teacher."
                }
            );
        }

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains(role))
        {
            return IdentityResult.Success;
        }

        if (currentRoles.Count > 0)
        {
            IdentityResult removeResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    currentRoles
                );

            if (!removeResult.Succeeded)
            {
                return removeResult;
            }
        }

        return await _userManager.AddToRoleAsync(user, role);
    }
}
