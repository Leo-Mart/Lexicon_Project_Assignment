using LMS.Api.DTOs.Courses;
using LMS.Api.Mappings;
using LMS.Api.Repositories.Interfaces.Courses;
using LMS.Api.Services.Interfaces.Course;

namespace LMS.Api.Services.Implementations.Course
{
    public class CourseService(ICourseRepository courseRepo) : ICourseService
    {
        private readonly ICourseRepository _courseRepo = courseRepo;

        public async Task<CourseDto> CreateNewCourse(CreateNewCourseDto newCourse)
        {
            if (newCourse.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            {
                throw new ArgumentException("Start date cannot be in the past.");
            }
            int result = newCourse.EndDate.CompareTo(newCourse.StartDate);
            if (result < 0 || result == 0)
            {
                throw new ArgumentException("End date has to be in the future.");
            }
            var courseToSave = newCourse.ToCourseFromCreateDto();

            var savedCourse = await _courseRepo.CreateCourseAsync(courseToSave);
            return savedCourse.ToDtoFromCourse();
        }

        public async Task<CourseDto?> DeleteCourse(Guid courseId)
        {
            var deletedCourse = await _courseRepo.DeleteCourseByIdAsync(courseId);
            if (deletedCourse == null)
            {
                // log / throw error?
                return null;
            }

            return deletedCourse.ToDtoFromCourse();
        }

        public async Task<IEnumerable<CourseDto>?> GetAllCourses()
        {
            var courses = await _courseRepo.GetCoursesAsync();
            if (courses == null)
            {
                // log / throw error?
                return null;
            }

            return courses.Select(c => c.ToDtoFromCourse());
        }

        public async Task<CourseDto?> GetCourseById(Guid courseId)
        {
            var course = await _courseRepo.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                // log / throw error?
                return null;
            }

            return course.ToDtoFromCourse();
        }

        public async Task<CourseDto?> UpdateCourse(Guid courseId, UpdateCourseDto updateCourseDto)
        {
            var updatedCourseFromDb = await _courseRepo.UpdateCourseAsync(
                courseId,
                updateCourseDto
            );
            if (updatedCourseFromDb == null)
            {
                // log / throw error?
                return null;
            }

            return updatedCourseFromDb.ToDtoFromCourse();
        }
    }
}
