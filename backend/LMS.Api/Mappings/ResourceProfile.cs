using AutoMapper;
using LMS.Api.DTOs.Resources;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between resources and their DTOs.
/// </summary>
public class ResourceProfile : Profile
{
    public ResourceProfile()
    {
        CreateMap<Resource, ResourceDto>();

        // Neither DTO carries the id, the owner or the timestamps.
        // The service sets those after the map.
        CreateMap<ResourceCreateDto, Resource>();
        CreateMap<ResourceUpdateDto, Resource>();
    }
}
