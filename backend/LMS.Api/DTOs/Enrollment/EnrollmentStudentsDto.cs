using LMS.Api.DTOs.Users;

namespace LMS.Api.DTOs.Enrollment;

public class EnrollmentStudentsDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }

    public UserDto Student { get; set; } = null!;
}
