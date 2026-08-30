using LMS.Api.Enums.Model;

namespace LMS.Api.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserStatus Status { get; set; }
}