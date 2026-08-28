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
            }
        };

        context.Resources.AddRange(resources);

        await context.SaveChangesAsync();
    }
}
