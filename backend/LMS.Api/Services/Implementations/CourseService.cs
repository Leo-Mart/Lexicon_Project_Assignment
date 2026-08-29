using LMS.Api.DTOs.Courses;
using LMS.Api.Mappings;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations
{
    public class CourseService(ICourserepository courseRepo) : ICourseService
    {
        private readonly ICourserepository _courseRepo = courseRepo;

        public Task<CourseDto?> CreateNewCourse(CreateNewCourseDto newCourse)
        {
            throw new NotImplementedException();
        }

        public Task<CourseDto?> DeleteCourse(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CourseDto>?> GetAllCourses()
        {
            var courses = await _courseRepo.GetCoursesAsync();
            if (courses == null)
            {
                // log an error
                return null;
            }

            return courses.Select(c => c.ToDtoFromCourse());
        }

        public async Task<CourseDto?> GetCourseById(Guid courseId)
        {
            var course = await _courseRepo.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                // log an error
                return null;
            }

            return course.ToDtoFromCourse();
        }

        public Task<CourseDto?> UpdateCourse(UpdateCourseDto updateCourse)
        {
            throw new NotImplementedException();
        }
    }
}
