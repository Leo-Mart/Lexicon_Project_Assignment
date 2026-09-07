using LMS.Api.Enums.Model;

namespace LMS.Api.DTOs.Users;

public class UserWithCourseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public UserStatus Status { get; set; }

    public Guid? CourseId { get; set; }

    public string? CourseName { get; set; }
    public string Role { get; set; } = string.Empty;
}