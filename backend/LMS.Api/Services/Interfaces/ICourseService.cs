using LMS.Api.Models;

namespace LMS.Api.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>?> GetAllCourses();
    }
}
