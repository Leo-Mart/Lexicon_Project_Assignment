using LMS.Api.DTOs.Auth;
using LMS.Api.Models;

namespace LMS.Api.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(LoginDto loginDto);

    Task<IList<string>> GetRolesAsync(User user);

    Task<User?> FindUserByIdAsync(Guid userId);
}
