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

        // Extra students, by their generated id suffix (see UserSeeder).
        Guid Student(int n) => Guid.Parse($"20000000-0000-0000-0000-{n:D12}");

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
                ReviewStatus = SubmissionReviewStatus.Approved,
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
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, needs completion.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000004"),
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = Student(4),
                Text = "Web API assignment submitted.",
                SubmittedAt = new DateTime(2026, 9, 30, 10, 0, 0, DateTimeKind.Utc),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Missing error handling on the POST endpoint.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(2026, 10, 2, 9, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, not reviewed yet.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000005"),
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = Student(6),
                Text = "Web API assignment submitted.",
                SubmittedAt = new DateTime(2026, 9, 29, 16, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, approved.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000006"),
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = Student(8),
                Text = "Web API assignment submitted.",
                SubmittedAt = new DateTime(2026, 9, 28, 12, 0, 0, DateTimeKind.Utc),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Solid implementation.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(2026, 9, 30, 10, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Late, needs completion.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000007"),
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = Student(10),
                Text = "Web API assignment submitted (late).",
                SubmittedAt = new DateTime(2026, 10, 3, 20, 0, 0, DateTimeKind.Utc),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Late, and still missing validation.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(2026, 10, 5, 9, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Late, not reviewed yet.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000008"),
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = Student(12),
                Text = "Web API assignment submitted (late).",
                SubmittedAt = new DateTime(2026, 10, 4, 8, 0, 0, DateTimeKind.Utc),
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
                    2026, 9, 1, 13, 0, 0, DateTimeKind.Utc
                ),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Well structured components.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(
                    2026, 9, 3, 11, 0, 0, DateTimeKind.Utc
                ),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, approved.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000009"),
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = Student(5),
                Text = "React component exercise submitted.",
                SubmittedAt = new DateTime(2026, 8, 31, 15, 0, 0, DateTimeKind.Utc),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Great component structure.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(2026, 9, 2, 9, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Late, not reviewed yet.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000010"),
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = Student(7),
                Text = "React component exercise submitted (late).",
                SubmittedAt = new DateTime(2026, 9, 3, 10, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, needs completion.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000011"),
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = Student(9),
                Text = "React component exercise submitted.",
                SubmittedAt = new DateTime(2026, 8, 30, 11, 0, 0, DateTimeKind.Utc),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Props aren't typed correctly.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Late, needs completion.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000012"),
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = Student(11),
                Text = "React component exercise submitted (late).",
                SubmittedAt = new DateTime(2026, 9, 5, 9, 0, 0, DateTimeKind.Utc),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Submitted late and still missing tests.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(2026, 9, 6, 9, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, not reviewed yet.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000013"),
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = Student(13),
                Text = "React component exercise submitted.",
                SubmittedAt = new DateTime(2026, 8, 31, 9, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Late, approved anyway.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000014"),
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = Student(15),
                Text = "React component exercise submitted (late).",
                SubmittedAt = new DateTime(2026, 9, 2, 14, 0, 0, DateTimeKind.Utc),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Late but good work overall.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = new DateTime(2026, 9, 4, 9, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Submissions.AddRange(submissions);

        await context.SaveChangesAsync();
    }
}
