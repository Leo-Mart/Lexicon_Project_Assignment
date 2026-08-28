using LMS.Api.DTOs.Auth;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;

namespace LMS.Api.Services.Implementations.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User?> AuthenticateAsync(LoginDto loginDto)
    {
        User? user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
        {
            return null;
        }

        if (user.Status != UserStatus.Active)
        {
            return null;
        }

        bool passwordIsValid =
            await _userManager.CheckPasswordAsync(
                user,
                loginDto.Password
            );

        if (!passwordIsValid)
        {
            return null;
        }

        return user;
    }

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<User?> FindActiveUserByIdAsync(Guid userId)
    {
        User? user =
            await _userManager.FindByIdAsync(
                userId.ToString()
            );

        if (user is null)
        {
            return null;
        }

        if (user.Status != UserStatus.Active)
        {
            return null;
        }

        return user;
    }
}
