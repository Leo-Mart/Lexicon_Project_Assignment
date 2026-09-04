using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class ModuleSeeder
{
    public static readonly Guid CSharpBasicsModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000001");

    public static readonly Guid AspNetCoreModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000002");

    public static readonly Guid EntityFrameworkModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000003");

    public static readonly Guid TypeScriptModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000004");

    public static readonly Guid ReactModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000005");

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Modules.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var modules = new List<Module>
        {
            new()
            {
                ModuleId = CSharpBasicsModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "C# Fundamentals",
                Description = "Introduction to C# and object-oriented programming.",
                StartDate = new DateOnly(2026, 8, 17),
                EndDate = new DateOnly(2026, 9, 18),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = AspNetCoreModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "ASP.NET Core",
                Description = "Building web APIs with ASP.NET Core.",
                StartDate = new DateOnly(2026, 9, 21),
                EndDate = new DateOnly(2026, 10, 23),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = EntityFrameworkModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Entity Framework Core",
                Description = "Database access and persistence with Entity Framework Core.",
                StartDate = new DateOnly(2026, 10, 26),
                EndDate = new DateOnly(2026, 11, 27),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = TypeScriptModuleId,
                CourseId = CourseSeeder.FrontendCourseId,
                Name = "TypeScript",
                Description = "Strongly typed frontend development with TypeScript.",
                StartDate = new DateOnly(2026, 8, 17),
                EndDate = new DateOnly(2026, 9, 18),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = ReactModuleId,
                CourseId = CourseSeeder.FrontendCourseId,
                Name = "React",
                Description = "Building component-based frontend applications with React.",
                StartDate = new DateOnly(2026, 9, 21),
                EndDate = new DateOnly(2026, 10, 23),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Modules.AddRange(modules);

        await context.SaveChangesAsync();
    }
}
