using LMS.Api.Models;

namespace LMS.Api.Services.Interfaces.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user, IList<string> roles);

    string GenerateRefreshToken();
}
