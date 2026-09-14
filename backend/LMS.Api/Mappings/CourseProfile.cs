using AutoMapper;
using LMS.Api.DTOs.Course;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between courses and their DTOs.
/// </summary>
public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(
                dto => dto.CourseResources,
                opt => opt.MapFrom(x => x.CourseResources.Select(cr => cr.Resource))
            );
        CreateMap<Course, BasicCourseInfoDto>();
        CreateMap<CreateNewCourseDto, Course>();
        CreateMap<UpdateCourseDto, Course>();
    }
}
