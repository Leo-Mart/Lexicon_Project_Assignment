using AutoMapper;
using LMS.Api.Constants;
using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Users;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userManager.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<IdentityResult> CreateUserAsync(
        User user,
        string password,
        string role)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        IdentityResult createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return createResult;
        }

        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return roleResult;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateUserStatusAsync(
        Guid userId,
        UserStatus status)
    {
        User? user = await FindUserByIdAsync(userId);

        if (user is null)
        {
            return UserNotFoundResult();
        }

        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> UpdateUserAsync(Guid userId, UserUpdateDto request)
    {
        User? user = await FindUserByIdAsync(userId);

        if (user is null)
        {
            return UserNotFoundResult();
        }

        // Merges onto the tracked entity. Which members a null skips is
        // declared by the PreConditions in UserProfile, not repeated here.
        // Role is not a User property, so the map cannot carry it - Identity
        // keeps it in AspNetUserRoles and it stays a UserManager call below.
        _mapper.Map(request, user);

        if (request.Role is not null)
        {
            IdentityResult roleResult = await UpdateRoleAsync(user, request.Role);

            if (!roleResult.Succeeded)
            {
                return roleResult;
            }
        }

        user.UpdatedAt = DateTime.UtcNow;

        return await _userManager.UpdateAsync(user);
    }

    private async Task<User?> FindUserByIdAsync(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }
    private static IdentityResult UserNotFoundResult()
    {
        return IdentityResult.Failed(
            new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found."
            }
        );
    }
    private async Task<IdentityResult> UpdateRoleAsync(User user, string role)
    {
        if (role != RoleConstants.Student &&
            role != RoleConstants.Teacher)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "InvalidRole",
                    Description = "Role must be Student or Teacher."
                }
            );
        }

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains(role))
        {
            return IdentityResult.Success;
        }

        if (currentRoles.Count > 0)
        {
            IdentityResult removeResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    currentRoles
                );

            if (!removeResult.Succeeded)
            {
                return removeResult;
            }
        }

        return await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<PagedResponse<UserWithCourseDto>> GetAllWithCourseAsync(UserQueryParametersDto query)
    {
        IQueryable<User> usersQuery = _userManager.Users
            .AsNoTracking()
            .Include(user => user.Enrollment)
                .ThenInclude(enrollment => enrollment!.Course);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string search = query.Search.Trim();

            usersQuery = usersQuery.Where(user =>
                user.Name.Contains(search) ||
                (user.Email != null && user.Email.Contains(search)) ||
                (user.Enrollment != null &&
                user.Enrollment.Course.Name.Contains(search)));
        }

        usersQuery = query.SortBy.ToLowerInvariant() switch
        {
            "email" => query.Direction == "desc"
                ? usersQuery.OrderByDescending(user => user.Email)
                : usersQuery.OrderBy(user => user.Email),

            "status" => query.Direction == "desc"
                ? usersQuery.OrderByDescending(user => user.Status)
                : usersQuery.OrderBy(user => user.Status),

            "course" => query.Direction == "desc"
                ? usersQuery.OrderByDescending(user => user.Enrollment!.Course.Name)
                : usersQuery.OrderBy(user => user.Enrollment!.Course.Name),

            _ => query.Direction == "desc"
                ? usersQuery.OrderByDescending(user => user.Name)
                : usersQuery.OrderBy(user => user.Name)
        };

        int totalCount = await usersQuery.CountAsync();

        List<User> users = await usersQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        List<UserWithCourseDto> items = [];

        foreach (User user in users)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            items.Add(new UserWithCourseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Status = user.Status,
                Role = roles.FirstOrDefault() ?? string.Empty,
                CourseId = user.Enrollment?.CourseId,
                CourseName = user.Enrollment?.Course.Name
            });
        }

        return new PagedResponse<UserWithCourseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}

