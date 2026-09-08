using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class EnrollmentSeeder
{
    public static async Task SeedAsync(LMSDbContext context)
    {
        DateTime now = DateTime.UtcNow;

        await EnrollAsync(context, UserSeeder.StudentOneId, CourseSeeder.DotNetCourseId, now);
        await EnrollAsync(context, UserSeeder.StudentTwoId, CourseSeeder.DotNetCourseId, now);
        await EnrollAsync(context, UserSeeder.StudentThreeId, CourseSeeder.FrontendCourseId, now);

        // The extra students UserSeeder adds (ids ...0004 to ...0030),
        // split evenly across both courses.
        for (int i = 4; i <= 30; i++)
        {
            Guid studentId = Guid.Parse($"20000000-0000-0000-0000-{i:D12}");
            Guid courseId = i % 2 == 0 ? CourseSeeder.DotNetCourseId : CourseSeeder.FrontendCourseId;

            await EnrollAsync(context, studentId, courseId, now);
        }
    }

    private static async Task EnrollAsync(
        LMSDbContext context,
        Guid studentId,
        Guid courseId,
        DateTime now)
    {
        bool alreadyEnrolled = await context.Enrollments
            .AnyAsync(enrollment => enrollment.StudentId == studentId);

        if (alreadyEnrolled)
        {
            return;
        }

        context.Enrollments.Add(new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = now
        });

        await context.SaveChangesAsync();
    }
}
