using AutoMapper;
using LMS.Api.DTOs.Activities;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

public class ActivityProfile : Profile
{
    public ActivityProfile()
    {
        CreateMap<Activity, ActivityDto>();
        CreateMap<ActivityCreateDto, Activity>();
        CreateMap<ActivityUpdateDto, Activity>();
    }
}
