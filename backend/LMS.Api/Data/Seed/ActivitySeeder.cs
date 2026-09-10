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

    // Extra DotNet-course tasks so one student (Erik) can show every review
    // state on its own unique activity.
    public static readonly Guid ConsoleCalculatorTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000007");

    public static readonly Guid CollectionsExerciseTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000008");

    public static readonly Guid JwtAuthenticationTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000009");

    public static readonly Guid IntegrationTestsTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000010");

    public static readonly Guid DatabaseMigrationsTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000011");

    public static readonly Guid QueryOptimizationTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000012");

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

            // Deadlines below are relative to now, not hardcoded calendar
            // dates, so they and their submissions never drift into the
            // future no matter when this gets seeded.
            new()
            {
                ActivityId = ConsoleCalculatorTaskId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Task,
                Name = "Console Calculator",
                Description = "Build a console app that performs basic arithmetic using methods.",
                StartAt = now.AddDays(-24),
                EndAt = now.AddDays(-15),
                Deadline = now.AddDays(-15),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = CollectionsExerciseTaskId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Task,
                Name = "Collections Exercise",
                Description = "Work with lists, dictionaries and LINQ to solve small data problems.",
                StartAt = now.AddDays(-20),
                EndAt = now.AddDays(-10),
                Deadline = now.AddDays(-10),
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
                StartAt = now.AddDays(-14),
                EndAt = now.AddDays(-3),
                Deadline = now.AddDays(-3),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = JwtAuthenticationTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Add JWT Authentication",
                Description = "Secure the Web API with JWT-based authentication and authorization.",
                StartAt = now.AddDays(-16),
                EndAt = now.AddDays(-6),
                Deadline = now.AddDays(-6),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = IntegrationTestsTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Write Integration Tests",
                Description = "Add integration tests for the Web API's endpoints.",
                StartAt = now.AddDays(-10),
                EndAt = now.AddDays(4),
                Deadline = now.AddDays(4),
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
                StartAt = now.AddDays(-13),
                EndAt = now.AddDays(-13).AddHours(3),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = DatabaseMigrationsTaskId,
                ModuleId = ModuleSeeder.EntityFrameworkModuleId,
                Type = ActivityType.Task,
                Name = "Database Migrations",
                Description = "Create and apply EF Core migrations for a small schema.",
                StartAt = now.AddDays(-12),
                EndAt = now.AddDays(-5),
                Deadline = now.AddDays(-5),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = QueryOptimizationTaskId,
                ModuleId = ModuleSeeder.EntityFrameworkModuleId,
                Type = ActivityType.Task,
                Name = "Query Optimization",
                Description = "Write efficient EF Core LINQ queries and inspect the generated SQL.",
                StartAt = now.AddDays(-2),
                EndAt = now.AddDays(10),
                Deadline = now.AddDays(10),
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
                StartAt = now.AddDays(-14),
                EndAt = now.AddDays(-3),
                Deadline = now.AddDays(-3),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Activities.AddRange(activities);

        await context.SaveChangesAsync();
    }
}
