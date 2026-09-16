using AutoMapper;
using LMS.Api.DTOs.Enrollment;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between enrollments and their DTOs.
/// </summary>
public class EnrollmentsProfile : Profile
{
    public EnrollmentsProfile()
    {
        CreateMap<Enrollment, EnrollmentStudentsDto>();
        CreateMap<EnrollmentStudentsDto, Enrollment>();
    }
}
