using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Enrollment;
using LMS.Api.Models;

namespace LMS.Api.Services.Interfaces;

public interface IEnrollmentService
{
    Task<CourseDto?> GetStudentCourseAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task<bool> AssignOrChangeCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);

    // Each Enrollment's Student is loaded, so callers get names, not just ids.
    Task<List<Enrollment>> GetEnrollmentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<List<EnrollmentStudentsDto>> GetStudentEnrollmentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    
    Task RemoveCourseAsync(Guid studentId, CancellationToken cancellationToken = default);
}
