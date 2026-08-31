using System.ComponentModel.DataAnnotations;
using LMS.Api.Enums.Model;

namespace LMS.Api.DTOs.Users;

public class UserUpdateDto
{
    public string? Name { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public UserStatus? Status { get; set; }

    [RegularExpression("^(Student|Teacher)$", ErrorMessage = "Role must be Student or Teacher.")]
    public string? Role { get; set; }
}
