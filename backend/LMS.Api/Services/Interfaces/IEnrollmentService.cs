using LMS.Api.DTOs.Course;

namespace LMS.Api.Services.Interfaces;

public interface IEnrollmentService
{
    Task<CourseDto?> GetStudentCourseAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task<bool> AssignOrChangeCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);

    Task RemoveCourseAsync(Guid studentId, CancellationToken cancellationToken = default);
}
