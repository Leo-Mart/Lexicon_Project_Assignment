using LMS.Api.Enums.Model;
using LMS.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class UserSeeder
{
    public static readonly Guid TeacherId =
        Guid.Parse("10000000-0000-0000-0000-000000000001");

    public static readonly Guid StudentOneId =
        Guid.Parse("20000000-0000-0000-0000-000000000001");

    public static readonly Guid StudentTwoId =
        Guid.Parse("20000000-0000-0000-0000-000000000002");

    public static readonly Guid StudentThreeId =
        Guid.Parse("20000000-0000-0000-0000-000000000003");

    public static async Task SeedAsync(UserManager<User> userManager)
    {
        await CreateUserAsync(
            userManager,
            TeacherId,
            "Anna Andersson",
            "anna.teacher@example.com",
            "Teacher123!",
            RoleSeeder.TeacherRole
        );

        await CreateUserAsync(
            userManager,
            StudentOneId,
            "Erik Svensson",
            "erik.student@example.com",
            "Student123!",
            RoleSeeder.StudentRole
        );

        await CreateUserAsync(
            userManager,
            StudentTwoId,
            "Maria Johansson",
            "maria.student@example.com",
            "Student123!",
            RoleSeeder.StudentRole
        );

        await CreateUserAsync(
            userManager,
            StudentThreeId,
            "Johan Karlsson",
            "johan.student@example.com",
            "Student123!",
            RoleSeeder.StudentRole
        );
    }

    private static async Task CreateUserAsync(
    UserManager<User> userManager,
    Guid id,
    string name,
    string email,
    string password,
    string role)
    {
        User? existingUserById = await userManager.FindByIdAsync(id.ToString());

        if (existingUserById is not null)
        {
            return;
        }

        User? existingUserByEmail = await userManager.FindByEmailAsync(email);

        if (existingUserByEmail is not null)
        {
            return;
        }

        User user = new()
        {
            Id = id,
            Name = name,
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        IdentityResult createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", createResult.Errors.Select(error => error.Description))
            );
        }

        IdentityResult roleResult = await userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", roleResult.Errors.Select(error => error.Description))
            );
        }
    }

}
