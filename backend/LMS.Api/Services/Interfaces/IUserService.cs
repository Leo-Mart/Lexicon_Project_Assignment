using LMS.Api.Enums.Model;
using LMS.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS.Api.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetUserByIdAsync(Guid userId);

    Task<IdentityResult> CreateUserAsync(
        User user,
        string password,
        string role
    );

    Task<IdentityResult> UpdateUserAsync(
        Guid userId,
        string? name,
        string? email,
        UserStatus status,
        string? role
    );

    Task<IdentityResult> UpdateUserStatusAsync(Guid userId, UserStatus status);
}
