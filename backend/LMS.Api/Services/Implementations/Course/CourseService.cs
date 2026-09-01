using AutoMapper;
using LMS.Api.DTOs.Courses;
using LMS.Api.Repositories.Interfaces.Courses;
using LMS.Api.Services.Interfaces.Course;
using CourseEntity = LMS.Api.Models.Course;

namespace LMS.Api.Services.Implementations.Course
{
    // The enclosing namespace is itself called Course, so the bare name Course
    // binds to the namespace rather than to LMS.Api.Models.Course. The
    // CourseEntity alias above is what lets the entity be named in _mapper.Map
    // calls; without it those lines are CS0118, "namespace used like a type".
    public class CourseService(ICourseRepository courseRepo, IMapper mapper) : ICourseService
    {
        private readonly ICourseRepository _courseRepo = courseRepo;
        private readonly IMapper _mapper = mapper;

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
            var courseToSave = _mapper.Map<CourseEntity>(newCourse);

            var savedCourse = await _courseRepo.CreateCourseAsync(courseToSave);
            return _mapper.Map<CourseDto>(savedCourse);
        }

        public async Task<CourseDto?> DeleteCourse(Guid courseId)
        {
            var deletedCourse = await _courseRepo.DeleteCourseByIdAsync(courseId);
            if (deletedCourse == null)
            {
                // log / throw error?
                return null;
            }

            return _mapper.Map<CourseDto>(deletedCourse);
        }

        public async Task<IEnumerable<CourseDto>?> GetAllCourses()
        {
            var courses = await _courseRepo.GetCoursesAsync();
            if (courses == null)
            {
                // log / throw error?
                return null;
            }

            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<CourseDto?> GetCourseById(Guid courseId)
        {
            var course = await _courseRepo.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                // log / throw error?
                return null;
            }

            return _mapper.Map<CourseDto>(course);
        }

        public async Task<CourseDto?> UpdateCourse(Guid courseId, UpdateCourseDto updateCourseDto)
        {
            var courseFromDb = await _courseRepo.GetCourseByIdAsync(courseId);
            if (courseFromDb == null)
            {
                // log / throw error?
                return null;
            }

            // The merge onto the entity belongs here, not in the repository,
            // which no longer sees the DTO at all.
            _mapper.Map(updateCourseDto, courseFromDb);

            var updatedCourseFromDb = await _courseRepo.UpdateCourseAsync(courseFromDb);

            return _mapper.Map<CourseDto>(updatedCourseFromDb);
        }
    }
}
