using CourseEntity = LMS.Api.Models.Course;

namespace LMS.Api.Repositories.Interfaces.Course
{
    public interface ICourseRepository
    {
        Task<IEnumerable<CourseEntity>> GetCoursesAsync();
        Task<CourseEntity?> GetCourseByIdAsync(Guid courseId);
        Task<CourseEntity> CreateCourseAsync(CourseEntity course);
        Task<CourseEntity> UpdateCourseAsync(CourseEntity course);
        Task<CourseEntity?> DeleteCourseByIdAsync(Guid courseId);
    }
}
