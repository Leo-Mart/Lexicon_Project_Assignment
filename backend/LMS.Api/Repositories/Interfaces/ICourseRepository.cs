using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces
{
    public interface ICourserepository
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course> GetCourseByIdAsync(Guid courseId);
        Task<Course> CreateCourseAsync(Course course);
    }
}
