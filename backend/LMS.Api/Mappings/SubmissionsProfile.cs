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
        CreateMap<Submission, SubmissionDto>();
        CreateMap<SubmissionCreateDto, Submission>();
    }
}
