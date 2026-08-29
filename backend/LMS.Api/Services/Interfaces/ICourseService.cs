using LMS.Api.DTOs.Courses;

namespace LMS.Api.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>?> GetAllCourses();
        Task<CourseDto?> GetCourseById(Guid courseId);
        Task<CourseDto> CreateNewCourse(CreateNewCourseDto newCourse);
        Task<CourseDto?> UpdateCourse(Guid courseId, UpdateCourseDto updateCourse);
        Task<CourseDto?> DeleteCourse(Guid courseId);
    }
}
