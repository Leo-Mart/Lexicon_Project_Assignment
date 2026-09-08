using AutoMapper;
using LMS.Api.DTOs.Resources;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between submissions and their DTOs.
/// </summary>
public class SubmissionsProfile : Profile
{
    public SubmissionsProfile()
    {
        // IsLate is derived from Activity.Deadline, not stored on the submission.
        CreateMap<Submission, SubmissionDto>()
            .ForMember(dest => dest.IsLate, opt => opt.MapFrom(src =>
                src.Activity != null && src.Activity.Deadline != null && src.SubmittedAt > src.Activity.Deadline));
        CreateMap<SubmissionCreateDto, Submission>();
    }
}
