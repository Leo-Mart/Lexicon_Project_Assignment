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

    // Extra DotNet-course modules so the module list has real depth: a run
    // of finished modules before C# Fundamentals, and upcoming ones after
    // Entity Framework Core.
    public static readonly Guid GitModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000006");

    public static readonly Guid ProgrammingFundamentalsModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000007");

    public static readonly Guid ObjectOrientedProgrammingModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000008");

    public static readonly Guid AuthenticationModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000009");

    public static readonly Guid AutomatedTestingModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000010");

    public static readonly Guid DockerDeploymentModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000011");

    public static readonly Guid CapstoneProjectModuleId =
        Guid.Parse("40000000-0000-0000-0000-000000000012");

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Modules.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        DateOnly today = DateOnly.FromDateTime(now);

        // Modules within a course run one after another - each of the two
        // courses below is a sequence of non-overlapping date ranges, with
        // exactly one module (if any) straddling today.
        var modules = new List<Module>
        {
            // Finished modules leading up to C# Fundamentals.
            new()
            {
                ModuleId = GitModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Git & Version Control",
                Description = "Using Git and GitHub for version control and collaboration.",
                StartDate = today.AddDays(-58),
                EndDate = today.AddDays(-49),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = ProgrammingFundamentalsModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Programming Fundamentals",
                Description = "Core programming concepts: variables, control flow and problem solving.",
                StartDate = today.AddDays(-48),
                EndDate = today.AddDays(-39),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = ObjectOrientedProgrammingModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Object-Oriented Programming",
                Description = "Classes, interfaces, inheritance and polymorphism in C#.",
                StartDate = today.AddDays(-38),
                EndDate = today.AddDays(-29),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Finished: both its tasks are already graded.
            new()
            {
                ModuleId = CSharpBasicsModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "C# Fundamentals",
                Description = "Introduction to C# and object-oriented programming.",
                StartDate = today.AddDays(-28),
                EndDate = today.AddDays(-15),
                CreatedAt = now,
                UpdatedAt = now
            },

            // In progress: its tasks have deadlines both before and after
            // today.
            new()
            {
                ModuleId = AspNetCoreModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "ASP.NET Core",
                Description = "Building web APIs with ASP.NET Core.",
                StartDate = today.AddDays(-14),
                EndDate = today.AddDays(10),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Not started yet: nothing here has a submission.
            new()
            {
                ModuleId = EntityFrameworkModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Entity Framework Core",
                Description = "Database access and persistence with Entity Framework Core.",
                StartDate = today.AddDays(13),
                EndDate = today.AddDays(40),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Upcoming modules after Entity Framework Core.
            new()
            {
                ModuleId = AuthenticationModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Authentication & Authorization",
                Description = "Securing applications with authentication and role-based authorization.",
                StartDate = today.AddDays(43),
                EndDate = today.AddDays(55),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = AutomatedTestingModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Automated Testing",
                Description = "Unit and integration testing with xUnit.",
                StartDate = today.AddDays(58),
                EndDate = today.AddDays(70),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = DockerDeploymentModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Docker & Deployment",
                Description = "Containerizing and deploying .NET applications with Docker.",
                StartDate = today.AddDays(73),
                EndDate = today.AddDays(85),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ModuleId = CapstoneProjectModuleId,
                CourseId = CourseSeeder.DotNetCourseId,
                Name = "Capstone Project",
                Description = "Building a complete application from the ground up.",
                StartDate = today.AddDays(88),
                EndDate = today.AddDays(110),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Finished: its one task is already submitted.
            new()
            {
                ModuleId = TypeScriptModuleId,
                CourseId = CourseSeeder.FrontendCourseId,
                Name = "TypeScript",
                Description = "Strongly typed frontend development with TypeScript.",
                StartDate = today.AddDays(-30),
                EndDate = today.AddDays(-16),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Finished: its one task's deadline is already in the past.
            new()
            {
                ModuleId = ReactModuleId,
                CourseId = CourseSeeder.FrontendCourseId,
                Name = "React",
                Description = "Building component-based frontend applications with React.",
                StartDate = today.AddDays(-15),
                EndDate = today.AddDays(-1),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Modules.AddRange(modules);

        await context.SaveChangesAsync();
    }
}
