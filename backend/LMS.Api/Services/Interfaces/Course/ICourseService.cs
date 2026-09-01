using LMS.Api.DTOs.Course;

namespace LMS.Api.Services.Interfaces.Course
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
