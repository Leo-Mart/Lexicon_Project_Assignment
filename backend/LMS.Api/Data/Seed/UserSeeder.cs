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

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var teacher = new User
        {
            UserId = TeacherId,
            Name = "Anna Andersson",
            Email = "anna.teacher@example.com",
            Role = UserType.Teacher,
            CreatedAt = now,
            UpdatedAt = now
        };

        var studentOne = new User
        {
            UserId = StudentOneId,
            Name = "Erik Svensson",
            Email = "erik.student@example.com",
            Role = UserType.Student,
            CreatedAt = now,
            UpdatedAt = now
        };

        var studentTwo = new User
        {
            UserId = StudentTwoId,
            Name = "Maria Johansson",
            Email = "maria.student@example.com",
            Role = UserType.Student,
            CreatedAt = now,
            UpdatedAt = now
        };

        var studentThree = new User
        {
            UserId = StudentThreeId,
            Name = "Johan Karlsson",
            Email = "johan.student@example.com",
            Role = UserType.Student,
            CreatedAt = now,
            UpdatedAt = now
        };

        var passwordHasher = new PasswordHasher<User>();

        teacher.PasswordHash =
            passwordHasher.HashPassword(teacher, "Teacher123!");

        studentOne.PasswordHash =
            passwordHasher.HashPassword(studentOne, "Student123!");

        studentTwo.PasswordHash =
            passwordHasher.HashPassword(studentTwo, "Student123!");

        studentThree.PasswordHash =
            passwordHasher.HashPassword(studentThree, "Student123!");

        context.Users.AddRange(
            teacher,
            studentOne,
            studentTwo,
            studentThree
        );

        await context.SaveChangesAsync();
    }
}
