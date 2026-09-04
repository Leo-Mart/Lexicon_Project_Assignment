using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class CourseSeeder
{
    public static readonly Guid DotNetCourseId =
        Guid.Parse("30000000-0000-0000-0000-000000000001");

    public static readonly Guid FrontendCourseId =
        Guid.Parse("30000000-0000-0000-0000-000000000002");

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Courses.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var courses = new List<Course>
        {
            new()
            {
                CourseId = DotNetCourseId,
                Name = "C# and .NET Development",
                Description = "Backend development with C#, .NET, ASP.NET Core and Entity Framework Core.",
                StartDate = new DateOnly(2026, 8, 17),
                EndDate = new DateOnly(2027, 2, 12),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                CourseId = FrontendCourseId,
                Name = "Frontend Development",
                Description = "Frontend development with HTML, CSS, JavaScript, TypeScript and React.",
                StartDate = new DateOnly(2026, 8, 17),
                EndDate = new DateOnly(2027, 2, 12),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Courses.AddRange(courses);

        await context.SaveChangesAsync();
    }
}
