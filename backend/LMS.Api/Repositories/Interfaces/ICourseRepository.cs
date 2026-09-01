using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces.Courses
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid courseId);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<Course?> DeleteCourseByIdAsync(Guid courseId);
    }
}
