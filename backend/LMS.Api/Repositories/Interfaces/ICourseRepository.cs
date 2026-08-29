using LMS.Api.DTOs.Courses;
using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces
{
    public interface ICourserepository
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid courseId);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course?> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateDto);
        Task<Course?> DeleteCourseByIdAsync(Guid courseId);
    }
}
