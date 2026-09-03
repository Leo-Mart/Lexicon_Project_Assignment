using AutoMapper;
using LMS.Api.DTOs.Course;
using LMS.Api.Exceptions;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;
using LMS.Api.Validators;

namespace LMS.Api.Services.Implementations;

public class CourseService(ICourseRepository courseRepo, IMapper mapper) : ICourseService
{
    private readonly ICourseRepository _courseRepo = courseRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<CourseDto> CreateNewCourse(CreateNewCourseDto newCourse)
    {
        ValidateCourseDates(
       newCourse.StartDate,
       newCourse.EndDate,
       validateNotBefore: true
   );

        var courseToSave = _mapper.Map<Course>(newCourse);

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

    private static void ValidateCourseDates(
    DateOnly startDate,
    DateOnly endDate,
    bool validateNotBefore = false
)
    {
        DateRangeValidator.ValidateRange(startDate, endDate, "Course");

        if (validateNotBefore)
        {
            DateRangeValidator.ValidateNotBefore(
                startDate,
                DateOnly.FromDateTime(DateTime.UtcNow),
                "Course"
            );
        }
    }
}
