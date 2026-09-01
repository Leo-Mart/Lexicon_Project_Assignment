using LMS.Api.DTOs.Course;
using CourseEntity = LMS.Api.Models.Course;

namespace LMS.Api.Repositories.Interfaces.Course
{
    public interface ICourseRepository
    {
        Task<IEnumerable<CourseEntity>> GetCoursesAsync();
        Task<CourseEntity?> GetCourseByIdAsync(Guid courseId);
        Task<CourseEntity> CreateCourseAsync(CourseEntity course);
        Task<CourseEntity?> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateDto);
        Task<CourseEntity?> DeleteCourseByIdAsync(Guid courseId);
    }
}
