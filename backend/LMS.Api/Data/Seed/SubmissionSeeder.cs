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

    // Erik's extra submissions - one per new DotNet task, each showing a
    // different review state (see ActivitySeeder for the matching tasks).
    public static readonly Guid ErikConsoleCalculatorSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000015");

    public static readonly Guid ErikCollectionsExerciseSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000016");

    public static readonly Guid ErikJwtAuthenticationSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000017");

    public static readonly Guid ErikIntegrationTestsSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000018");

    public static readonly Guid ErikInputValidationSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000019");

    // Erik's submissions in the three finished modules before C#
    // Fundamentals - mostly approved, with one needs-completion.
    public static readonly Guid ErikGitWorkflowSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000021");

    public static readonly Guid ErikControlFlowSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000022");

    public static readonly Guid ErikSimpleCalculatorSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000023");

    public static readonly Guid ErikClassDesignSubmissionId =
        Guid.Parse("70000000-0000-0000-0000-000000000024");

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
            // Erik - approved, on time.
            new()
            {
                SubmissionId = ErikGitWorkflowSubmissionId,
                ActivityId = ActivitySeeder.GitWorkflowTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Git workflow exercise submitted.",
                SubmittedAt = now.AddDays(-54),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Clean commit history and good use of branches. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-51),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, approved.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000025"),
                ActivityId = ActivitySeeder.GitWorkflowTaskId,
                StudentId = Student(6),
                Text = "Git workflow exercise submitted.",
                SubmittedAt = now.AddDays(-53),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Good branch naming and clear pull request description. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-50),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, needs completion.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000026"),
                ActivityId = ActivitySeeder.GitWorkflowTaskId,
                StudentId = Student(11),
                Text = "Git workflow exercise submitted.",
                SubmittedAt = now.AddDays(-53),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Everything was committed directly to main. Please redo this using a feature branch and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-50),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - approved.
            new()
            {
                SubmissionId = ErikControlFlowSubmissionId,
                ActivityId = ActivitySeeder.ControlFlowPracticeId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Control flow practice submitted.",
                SubmittedAt = now.AddDays(-44),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Logic is correct and easy to follow. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-42),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Approved.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000027"),
                ActivityId = ActivitySeeder.ControlFlowPracticeId,
                StudentId = Student(5),
                Text = "Control flow practice submitted.",
                SubmittedAt = now.AddDays(-43),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Well organized and correct. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-41),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - needs completion, on time.
            new()
            {
                SubmissionId = ErikSimpleCalculatorSubmissionId,
                ActivityId = ActivitySeeder.SimpleCalculatorTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Simple calculator submitted.",
                SubmittedAt = now.AddDays(-42),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Division by zero isn't handled and crashes the program. Please fix that and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-39),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, approved.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000028"),
                ActivityId = ActivitySeeder.SimpleCalculatorTaskId,
                StudentId = Student(9),
                Text = "Simple calculator submitted.",
                SubmittedAt = now.AddDays(-41),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Handles edge cases well, including division by zero. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-39),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - approved, on time.
            new()
            {
                SubmissionId = ErikClassDesignSubmissionId,
                ActivityId = ActivitySeeder.ClassDesignTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Class design exercise submitted.",
                SubmittedAt = now.AddDays(-34),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Good use of interfaces and a clean class hierarchy. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-32),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, approved.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000029"),
                ActivityId = ActivitySeeder.ClassDesignTaskId,
                StudentId = Student(13),
                Text = "Class design exercise submitted.",
                SubmittedAt = now.AddDays(-33),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Well modeled and easy to extend. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-31),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, needs completion.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000030"),
                ActivityId = ActivitySeeder.ClassDesignTaskId,
                StudentId = Student(4),
                Text = "Class design exercise submitted.",
                SubmittedAt = now.AddDays(-32),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "The classes work, but there's no interface here, just one big class. Please refactor and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-30),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - approved, on time.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000031"),
                ActivityId = ActivitySeeder.LoggingTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Logging submitted.",
                SubmittedAt = now.AddDays(-11),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Logging is consistent and covers all the endpoints. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-8),
                CreatedAt = now,
                UpdatedAt = now
            },

            // On time, approved.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000032"),
                ActivityId = ActivitySeeder.LoggingTaskId,
                StudentId = Student(8),
                Text = "Logging submitted.",
                SubmittedAt = now.AddDays(-10),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Good log levels and clear messages. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-8),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - approved, late.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000033"),
                ActivityId = ActivitySeeder.PaginationTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Pagination submitted (late).",
                SubmittedAt = now.AddDays(-5),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "This was late, but the pagination works well, including the edge cases. Try to submit on time next round.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-2),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Late, approved anyway.
            new()
            {
                SubmissionId = Guid.Parse("70000000-0000-0000-0000-000000000034"),
                ActivityId = ActivitySeeder.PaginationTaskId,
                StudentId = Student(6),
                Text = "Pagination submitted (late).",
                SubmittedAt = now.AddDays(-6),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Late, but correct and well tested. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-3),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - needs completion, on time.
            new()
            {
                SubmissionId = AspNetSubmissionOneId,
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Completed ASP.NET Core Web API assignment.",
                SubmittedAt = now.AddDays(-10),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Good overall structure and the GET endpoints work well. The POST endpoint is missing input validation, so please add that and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-6),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Late, not reviewed yet.
            new()
            {
                SubmissionId = AspNetSubmissionTwoId,
                ActivityId = ActivitySeeder.AspNetApiTaskId,
                StudentId = UserSeeder.StudentTwoId,
                Text = "ASP.NET Core Web API assignment submitted.",
                SubmittedAt = now,
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
                SubmittedAt = now.AddDays(-11),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "The API works for the happy path, but there's no error handling on the POST endpoint. Please add that and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-9),
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
                SubmittedAt = now.AddDays(-10),
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
                SubmittedAt = now.AddDays(-13),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Solid implementation overall. The endpoints are well organized and the error handling is thorough. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-10),
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
                SubmittedAt = now.AddDays(-6),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "This came in after the deadline, and the validation from the previous round still isn't in place. Please resubmit with that fixed.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-4),
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
                SubmittedAt = now.AddDays(-5),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - approved, on time.
            new()
            {
                SubmissionId = ErikConsoleCalculatorSubmissionId,
                ActivityId = ActivitySeeder.ConsoleCalculatorTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Console calculator submitted.",
                SubmittedAt = now.AddDays(-26),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Clean and correct implementation. The menu loop and the arithmetic methods are both easy to follow. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-22),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - approved, late.
            new()
            {
                SubmissionId = ErikCollectionsExerciseSubmissionId,
                ActivityId = ActivitySeeder.CollectionsExerciseTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Collections exercise submitted (late).",
                SubmittedAt = now.AddDays(-16),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "This was late, but the LINQ queries are well done and the code is easy to read. Try to submit on time next round.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-13),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - needs completion, late.
            new()
            {
                SubmissionId = ErikJwtAuthenticationSubmissionId,
                ActivityId = ActivitySeeder.JwtAuthenticationTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "JWT authentication submitted (late).",
                SubmittedAt = now.AddDays(-3),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "This was late, and the token isn't validated on every endpoint yet. Please add that validation and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-1),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - not reviewed yet, on time.
            new()
            {
                SubmissionId = ErikIntegrationTestsSubmissionId,
                ActivityId = ActivitySeeder.IntegrationTestsTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Integration tests submitted.",
                SubmittedAt = now.AddDays(-1),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Erik - not reviewed yet, late.
            new()
            {
                SubmissionId = ErikInputValidationSubmissionId,
                ActivityId = ActivitySeeder.InputValidationTaskId,
                StudentId = UserSeeder.StudentOneId,
                Text = "Input validation submitted (late).",
                SubmittedAt = now.AddDays(-1),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                SubmissionId = ReactSubmissionId,
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = UserSeeder.StudentThreeId,
                Text = "React component exercise completed.",
                SubmittedAt = now.AddDays(-6),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Well structured components with clear separation of concerns. The code is easy to follow - nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-3),
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
                SubmittedAt = now.AddDays(-7),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Great component structure and consistent naming throughout. Nice work.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-4),
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
                SubmittedAt = now.AddDays(-1),
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
                SubmittedAt = now.AddDays(-5),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "The components look good, but the props aren't typed correctly in a few places. Please fix the types and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now.AddDays(-2),
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
                SubmittedAt = now.AddDays(-2),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "This was submitted late and is still missing tests. Please add tests and resubmit.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now,
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
                SubmittedAt = now.AddDays(-4),
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
                SubmittedAt = now.AddDays(-1),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "This was late, but the work itself is good overall. Try to submit on time next round.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Submissions.AddRange(submissions);

        await context.SaveChangesAsync();
    }
}
