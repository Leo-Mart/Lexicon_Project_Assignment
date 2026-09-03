using AutoMapper;
using LMS.Api.DTOs.Resources;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between resources and their DTOs.
/// </summary>
public class SubmissionsProfile : Profile
{
    public SubmissionsProfile()
    {
        CreateMap<Submission, SubmissionDto>();

        // Neither DTO carries the id, the owner or the timestamps.
        // The service sets those after the map.
        CreateMap<SubmissionCreateDto, Submission>();
    }
}
