using LMS.Api.DTOs.Common;
using LMS.Api.Models;
namespace LMS.Api.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<PagedResponse<Course>> GetCoursesAsync(QueryParametersDto query, CancellationToken cancellationToken = default);
    Task<Course?> GetCourseByIdAsync(Guid courseId);
    Task<IEnumerable<Module>> GetModulesForCourseAsync(Guid courseId);
    Task<Course> CreateCourseAsync(Course course);
    Task<Course> UpdateCourseAsync(Course course);
    Task<Course?> DeleteCourseByIdAsync(Guid courseId);
}
