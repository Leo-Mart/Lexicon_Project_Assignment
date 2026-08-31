using AutoMapper;
using LMS.Api.DTOs.Courses;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between courses and their DTOs.
/// </summary>
public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>();
        CreateMap<CreateNewCourseDto, Course>();

        // Applied as Map(dto, existingCourse); UpdatedAt is set by the repository.
        CreateMap<UpdateCourseDto, Course>();
    }
}
