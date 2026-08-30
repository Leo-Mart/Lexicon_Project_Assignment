using LMS.Api.Enums.Model;
using LMS.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS.Api.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid userId);

    Task<IdentityResult> CreateAsync(
        User user,
        string password,
        string role
    );

    Task<IdentityResult> UpdateAsync(
        Guid userId,
        string? name,
        string? email,
        UserStatus status
    );

    Task<IdentityResult> UpdateStatusAsync(Guid userId, UserStatus status);
}
