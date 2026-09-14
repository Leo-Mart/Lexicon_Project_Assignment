using AutoMapper;
using LMS.Api.DTOs.Activities;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

public class ActivityProfile : Profile
{
    public ActivityProfile()
    {
        CreateMap<Activity, ActivityDto>()
            .ForMember(
                dto => dto.ActivityResources,
                opt => opt.MapFrom(x => x.ActivityResources.Select(ar => ar.Resource))
            );
        CreateMap<ActivityCreateDto, Activity>();
        CreateMap<ActivityUpdateDto, Activity>();
    }
}
