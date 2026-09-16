using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class ResourceSeeder
{
    public static readonly Guid CSharpGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000001");

    public static readonly Guid AspNetDocumentationResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000002");

    public static readonly Guid EntityFrameworkGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000003");

    public static readonly Guid TypeScriptGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000004");

    public static readonly Guid ReactGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000005");

    // Resources for the extra DotNet-course modules.
    public static readonly Guid CourseSyllabusResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000006");

    public static readonly Guid GitGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000007");

    public static readonly Guid OopGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000008");

    public static readonly Guid AuthenticationGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000009");

    public static readonly Guid TestingGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000010");

    public static readonly Guid DockerGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000011");

    // Resources for the extra ASP.NET Core module tasks.
    public static readonly Guid JwtGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000012");

    public static readonly Guid ModelValidationGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000013");

    public static readonly Guid LoggingGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000014");

    public static readonly Guid PaginationGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000015");

    public static readonly Guid IntegrationTestingGuideResourceId =
        Guid.Parse("60000000-0000-0000-0000-000000000016");

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Resources.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var resources = new List<Resource>
        {
            new()
            {
                ResourceId = CSharpGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "C# Fundamentals Guide",
                Description = "Study material covering basic C# concepts.",
                Content = "Introduction to variables, conditions, loops, methods and classes in C#.",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = AspNetDocumentationResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "ASP.NET Core Documentation",
                Description = "Reference material for ASP.NET Core Web API development.",
                Uri = "https://learn.microsoft.com/aspnet/core/",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = EntityFrameworkGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Entity Framework Core Guide",
                Description = "Reference material for Entity Framework Core.",
                Uri = "https://learn.microsoft.com/ef/core/",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = TypeScriptGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "TypeScript Guide",
                Description = "Study material for TypeScript.",
                Uri = "https://www.typescriptlang.org/docs/",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = ReactGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "React Guide",
                Description = "Reference material for React development.",
                Uri = "https://react.dev/learn",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = CourseSyllabusResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Course Syllabus",
                Description = "Schedule, grading policy and expectations for the course.",
                Content = "Ten modules, running from fundamentals through a final capstone project. Graded submissions use Approved/Needs completion; resubmit after Needs completion.",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = GitGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Git & GitHub Guide",
                Description = "Reference material for using Git and GitHub.",
                Uri = "https://docs.github.com/en/get-started",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = OopGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "OOP in C# Guide",
                Description = "Reference material for object-oriented programming in C#.",
                Uri = "https://learn.microsoft.com/dotnet/csharp/fundamentals/tutorials/oop",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = AuthenticationGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "ASP.NET Core Authentication Guide",
                Description = "Reference material for authentication and authorization.",
                Uri = "https://learn.microsoft.com/aspnet/core/security/",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = TestingGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "xUnit Testing Guide",
                Description = "Reference material for unit and integration testing.",
                Uri = "https://learn.microsoft.com/dotnet/core/testing/unit-testing-with-dotnet-test",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = DockerGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Docker for .NET Guide",
                Description = "Reference material for containerizing .NET applications.",
                Uri = "https://learn.microsoft.com/dotnet/core/docker/introduction",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = JwtGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "JWT Bearer Authentication Guide",
                Description = "Reference material for issuing and validating JWT bearer tokens.",
                Uri = "https://learn.microsoft.com/aspnet/core/security/authentication/jwt-authn",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = ModelValidationGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Model Validation Guide",
                Description = "Reference material for validating request models in a Web API.",
                Uri = "https://learn.microsoft.com/aspnet/core/mvc/models/validation",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = LoggingGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Logging in .NET Guide",
                Description = "Reference material for structured logging with the .NET logging APIs.",
                Uri = "https://learn.microsoft.com/dotnet/core/extensions/logging",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = PaginationGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Pagination Guide",
                Description = "Study material covering paging strategies for API list endpoints.",
                Content = "Use skip/take with a page and pageSize query parameter, cap pageSize, and return total count alongside the page of results.",
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ResourceId = IntegrationTestingGuideResourceId,
                CreatedByTeacherId = UserSeeder.TeacherId,
                Name = "Integration Tests in ASP.NET Core Guide",
                Description = "Reference material for testing a Web API with WebApplicationFactory.",
                Uri = "https://learn.microsoft.com/aspnet/core/test/integration-tests",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Resources.AddRange(resources);

        await context.SaveChangesAsync();
    }
}
