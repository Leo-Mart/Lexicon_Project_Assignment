using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class EnrollmentSeeder
{
    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Enrollments.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var enrollments = new List<Enrollment>
        {
            new()
            {
                StudentId = UserSeeder.StudentOneId,
                CourseId = CourseSeeder.DotNetCourseId,
                EnrolledAt = now
            },

            new()
            {
                StudentId = UserSeeder.StudentTwoId,
                CourseId = CourseSeeder.DotNetCourseId,
                EnrolledAt = now
            },

            new()
            {
                StudentId = UserSeeder.StudentThreeId,
                CourseId = CourseSeeder.FrontendCourseId,
                EnrolledAt = now
            }
        };

        context.Enrollments.AddRange(enrollments);

        await context.SaveChangesAsync();
    }
}
