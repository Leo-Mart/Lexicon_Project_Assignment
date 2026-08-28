using LMS.Api.Enums.Model;
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class ActivitySeeder
{
    public static readonly Guid CSharpLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000001");

    public static readonly Guid CSharpPracticeId =
        Guid.Parse("50000000-0000-0000-0000-000000000002");

    public static readonly Guid AspNetApiTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000003");

    public static readonly Guid EntityFrameworkLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000004");

    public static readonly Guid TypeScriptPracticeId =
        Guid.Parse("50000000-0000-0000-0000-000000000005");

    public static readonly Guid ReactTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000006");

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Activities.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var activities = new List<Activity>
        {
            new()
            {
                ActivityId = CSharpLectureId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Lecture,
                Name = "Introduction to C#",
                Description = "Introduction to C# syntax, types and basic language features.",
                StartAt = new DateTime(2026, 8, 18, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 8, 18, 12, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = CSharpPracticeId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Practice,
                Name = "C# Practice",
                Description = "Practice variables, conditions, loops and methods.",
                StartAt = new DateTime(2026, 8, 20, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 8, 20, 15, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetApiTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Build a Web API",
                Description = "Create a small ASP.NET Core Web API.",
                StartAt = new DateTime(2026, 9, 28, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 10, 2, 16, 0, 0, DateTimeKind.Utc),
                Deadline = new DateTime(2026, 10, 2, 16, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = EntityFrameworkLectureId,
                ModuleId = ModuleSeeder.EntityFrameworkModuleId,
                Type = ActivityType.Lecture,
                Name = "Entity Framework Core Introduction",
                Description = "Introduction to EF Core, DbContext and migrations.",
                StartAt = new DateTime(2026, 10, 27, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 10, 27, 12, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = TypeScriptPracticeId,
                ModuleId = ModuleSeeder.TypeScriptModuleId,
                Type = ActivityType.Practice,
                Name = "TypeScript Practice",
                Description = "Practice interfaces, types and functions in TypeScript.",
                StartAt = new DateTime(2026, 8, 24, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 8, 24, 15, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = ReactTaskId,
                ModuleId = ModuleSeeder.ReactModuleId,
                Type = ActivityType.Task,
                Name = "React Component Exercise",
                Description = "Build a small React application using reusable components.",
                StartAt = new DateTime(2026, 9, 28, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 10, 2, 16, 0, 0, DateTimeKind.Utc),
                Deadline = new DateTime(2026, 10, 2, 16, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Activities.AddRange(activities);

        await context.SaveChangesAsync();
    }
}
