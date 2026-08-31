using AutoMapper;
using LMS.Api.DTOs.Users;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between users and their DTOs.
/// </summary>
public class UserProfile : Profile
{
    public UserProfile()
    {
        // User.Email is nullable, UserDto.Email is not.
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty));

        // Identity rejects an empty UserName and this app has no separate one.
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        // A null means the client did not send that field, so keep what is stored.
        // Without this a name-only PUT nulls Email and sets Status 0, which is Deleted.
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Name, opt => opt.PreCondition(src => src.Name is not null))
            .ForMember(dest => dest.Email, opt => opt.PreCondition(src => src.Email is not null))
            .ForMember(dest => dest.Status, opt => opt.PreCondition(src => src.Status is not null))
            .ForMember(dest => dest.UserName, opt =>
            {
                opt.PreCondition(src => src.Email is not null);
                opt.MapFrom(src => src.Email);
            });
    }
}
