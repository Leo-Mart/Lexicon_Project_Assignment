using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Module;

namespace LMS.Api.Services.Interfaces;

public interface ICourseService
{
    Task<PagedResponse<CourseDto>> GetAllCourses(QueryParametersDto query, CancellationToken cancellationToken = default);
    Task<CourseDto?> GetCourseById(Guid courseId);
    Task<IEnumerable<ModuleDto>?> GetModulesForCourse(Guid courseId);
    Task<CourseDto> CreateNewCourse(CreateNewCourseDto newCourse);
    Task<CourseDto?> UpdateCourse(Guid courseId, UpdateCourseDto updateCourse);
    Task<CourseDto?> DeleteCourse(Guid courseId);
}
