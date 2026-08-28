using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations
{
    public class CourseService(ICourserepository courseRepo) : ICourseService
    {
        private readonly ICourserepository _courseRepo = courseRepo;

        public async Task<IEnumerable<Course>?> GetAllCourses()
        {
            var courses = await _courseRepo.GetCoursesAsync();
            if (courses == null)
            {
                // log an error
                return null;
            }

            return courses;
        }
    }
}
