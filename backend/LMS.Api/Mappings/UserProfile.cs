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
        // Read path: the entity as the API returns it.
        //
        // Implementation detail:
        // - Id, Name and Status map by name convention.
        // - User.Email is nullable, UserDto.Email is not. Without the ForMember
        //   below, a null is copied across and lands in a property declared as
        //   non-null. The ?? turns it into "" instead.
        //
        // TODO:
        // - Add Role to UserDto when a screen needs to display it. Identity
        //   keeps roles in AspNetUserRoles and User has no navigation property
        //   for them, so this map cannot read one. Set it in UserService after
        //   mapping, using UserManager.GetRolesAsync for a single user and a
        //   join on LMSDbContext.UserRoles and .Roles for a list, to avoid N+1.
        CreateMap<User, UserDto>()
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email ?? string.Empty)
            );

        // Create path: a new User built from the request body.
        //
        // Implementation detail:
        // - Name and Email map by name convention.
        // - UserName is set from Email because Identity's UserValidator rejects
        //   an empty UserName and this app has no separate username.
        // - Status is not mapped, so a new User keeps the initializer value
        //   (Active) and a client cannot choose its own.
        // - Password and Role do not exist on User at all. They stay as
        //   separate arguments to UserService.CreateUserAsync.
        // - The Ignore calls change nothing today: UserCreateDto has no member
        //   with any of those names, so nothing maps to them anyway. They guard
        //   against later. Add a CreatedAt property to UserCreateDto and
        //   AutoMapper starts mapping it by name convention, letting a client
        //   set the entity's own timestamp - silently, because nobody reopens
        //   this file to check.
        //
        // TODO:
        // - UsersController.CreateUser: replace the new User { ... } block with
        //   _mapper.Map<User>(request). Delete the Id = Guid.NewGuid() line
        //   instead of moving it here; User.Id is ValueGeneratedOnAdd, so EF
        //   assigns a sequential Guid on save, which is also better for the
        //   clustered index than a random one.
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollment, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedResources, opt => opt.Ignore())
            .ForMember(dest => dest.Submissions, opt => opt.Ignore())
            .ForMember(dest => dest.FeedbackSubmissions, opt => opt.Ignore());

        // Update path: a partial update merged onto a User already loaded from
        // the database.
        //
        // Implementation detail:
        // - Name and Email are skipped when the client sent null, so the stored
        //   value survives. That is what PreCondition does: it decides whether
        //   to assign at all, while MapFrom decides the value.
        // - UserName follows Email so the two never drift apart. It is guarded
        //   on src.Email because UserUpdateDto has no UserName of its own.
        // - Status is guarded the same way. UserUpdateDto.Status is
        //   UserStatus?, so "not sent" is expressible and an omitted status
        //   leaves the stored one alone instead of writing (UserStatus)0.
        // - Call it as Map(dto, existingUser). Map<User>(dto) builds a new User
        //   instead of merging onto the stored one, so every member the client
        //   omitted stays at its default rather than keeping the stored value,
        //   and Id and CreatedAt come back empty.
        // - The Ignore calls guard the same way as on the create map, and
        //   matter more here: this map runs against a tracked entity, so a
        //   future DTO property could otherwise replace collections EF has
        //   already loaded.
        //
        // TODO:
        // - Not wired up yet. UpdateUserAsync takes the fields as separate
        //   parameters and owns the tracked entity, so applying this map means
        //   either running it inside UserService or changing that signature to
        //   take UserUpdateDto.
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
            })
            .ForMember(dest => dest.Status, opt =>
            {
                opt.PreCondition(src => src.Status is not null);
                opt.MapFrom(src => src.Status);
            })
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollment, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedResources, opt => opt.Ignore())
            .ForMember(dest => dest.Submissions, opt => opt.Ignore())
            .ForMember(dest => dest.FeedbackSubmissions, opt => opt.Ignore());
    }
}
