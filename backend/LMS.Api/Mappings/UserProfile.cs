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

        // A null means the client did not send the field, so the stored value stands.
        // PreCondition per member, not ForAllMembers: Condition receives the value
        // already converted to the destination type, so a null UserStatus? arrives
        // as (UserStatus)0 and passes the check.
        CreateMap<UserUpdateDto, User>()
            .ForMember(
                dest => dest.Name,
                opt =>
                {
                    opt.PreCondition(src => src.Name is not null);
                    opt.MapFrom(src => src.Name);
                }
            )
            .ForMember(
                dest => dest.Email,
                opt =>
                {
                    opt.PreCondition(src => src.Email is not null);
                    opt.MapFrom(src => src.Email);
                }
            )
            .ForMember(
                dest => dest.UserName,
                opt =>
                {
                    opt.PreCondition(src => src.Email is not null);
                    opt.MapFrom(src => src.Email);
                }
            )
            .ForMember(
                dest => dest.Status,
                opt =>
                {
                    opt.PreCondition(src => src.Status is not null);
                    opt.MapFrom(src => src.Status);
                }
            );
    }
}
