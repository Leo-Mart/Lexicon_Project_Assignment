using LMS.Api.Enums.Model;
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class SubmissionSeeder
{
    public static readonly Guid AspNetSubmissionOneId =
        Guid.Parse("70000000-0000-0000-0000-000000000001");

    public static readonly Guid AspNetSubmissionTwoId =
        Guid.Parse("70000000-0000-0000-0000-000000000002");

    public static readonly Guid ReactSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000003");

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Submissions.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var submissions = new List<Submission>
        {
            new()
            {
                SubmissionId = AspNetSubmissionOneId,
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Completed ASP.NET Core Web API assignment.",
                SubmittedAt = new DateTime(
                    2026, 10, 1, 14, 30, 0, DateTimeKind.Utc
                ),
                Status = SubmissionStatus.Submitted,
                Feedback = "Good work. Clear structure and correct use of endpoints.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(
                    2026, 10, 3, 10, 0, 0, DateTimeKind.Utc
                ),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                SubmissionId = AspNetSubmissionTwoId,
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = UserSeeder.StudentTwoId,
                Text = "ASP.NET Core Web API assignment submitted.",
                SubmittedAt = new DateTime(
                    2026, 10, 3, 9, 0, 0, DateTimeKind.Utc
                ),
                Status = SubmissionStatus.Late,
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                SubmissionId = ReactSubmissionId,
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = UserSeeder.StudentThreeId,
                Text = "React component exercise completed.",
                SubmittedAt = new DateTime(
                    2026, 10, 2, 13, 0, 0, DateTimeKind.Utc
                ),
                Status = SubmissionStatus.Submitted,
                Feedback = "Well structured components.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(
                    2026, 10, 4, 11, 0, 0, DateTimeKind.Utc
                ),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Submissions.AddRange(submissions);

        await context.SaveChangesAsync();
    }
}
