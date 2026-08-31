using AutoMapper;
using LMS.Api.DTOs.Users;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // TODO UsersController: delete the private MapToDto helper and inject
        // IMapper instead. Replace its three call sites in GetUsers, GetUser
        // and CreateUser with _mapper.Map<UserDto>(user). For the list, prefer
        // _mapper.Map<IEnumerable<UserDto>>(users) over users.Select(...).
        CreateMap<User, UserDto>()
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email ?? string.Empty)
            );

        // TODO UsersController.CreateUser: replace the whole new User { ... }
        // block with _mapper.Map<User>(request). Drop the Id = Guid.NewGuid()
        // line rather than moving it here - the PK is ValueGeneratedOnAdd, so
        // EF assigns a sequential Guid on save. Password and Role stay as
        // separate arguments to CreateUserAsync; they are not mapped.
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        // Partial update: only assigns members the client actually sent.
        // Must be called as Map(dto, existingUser) so the PreConditions merge
        // onto stored values. Map<User>(dto) on a fresh instance skips them
        // and yields a User with empty Name/Email.
        //
        // TODO Not usable from the controller as written. UpdateUserAsync takes
        // (id, name, email, status, role) and fetches the tracked User itself,
        // so this map has to run inside UserService as _mapper.Map(dto, user) -
        // which means IUserService starts referencing LMS.Api.DTOs. Either
        // accept that, or change the signature to take UserUpdateDto directly.
        // Whichever wins, delete the now-duplicated null checks in
        // UpdateUserAsync so the merge rules live in one place only.
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Name, opt =>
            {
                opt.PreCondition(src => src.Name is not null);
                opt.MapFrom(src => src.Name);
            })
            .ForMember(dest => dest.Email, opt =>
            {
                opt.PreCondition(src => src.Email is not null);
                opt.MapFrom(src => src.Email);
            })
            .ForMember(dest => dest.UserName, opt =>
            {
                opt.PreCondition(src => src.Email is not null);
                opt.MapFrom(src => src.Email);
            });
    }
}
