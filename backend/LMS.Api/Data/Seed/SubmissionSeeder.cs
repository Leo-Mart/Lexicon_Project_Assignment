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
                Text = "Cloned the starter repo, created a feature branch for each change, and opened a pull request for every one with a short description. Commit messages follow the conventional commits style we went over in class.",
                SubmittedAt = now.AddDays(-54),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Clean commit history, sensible branch names, and your PR descriptions actually explain the change. This is exactly the workflow we're after.",
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
                Text = "Made a develop branch off main, then a feature branch off that for the actual task. Two commits, then opened a PR into develop with before/after screenshots in the description.",
                SubmittedAt = now.AddDays(-53),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Good branch naming, and the screenshots in the PR made it easy to review. Nice work.",
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
                Text = "Made the change described in the assignment and pushed it up. Let me know if you want me to change anything.",
                SubmittedAt = now.AddDays(-53),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Everything was committed directly to main. Please redo this using a feature branch and a pull request, then resubmit.",
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
                Text = "Solved the grade calculator with a switch expression, the FizzBuzz variant with if/else, and the number-guessing loop with a while. Added a few extra test values at the bottom of Main to check the edge cases.",
                SubmittedAt = now.AddDays(-44),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Logic is correct and easy to follow, and I like that you tested the edge cases yourself. Nice work.",
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
                Text = "Completed all three control-flow exercises. Went with nested if-statements for the grade calculator since it read clearer to me than a switch.",
                SubmittedAt = now.AddDays(-43),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Well organized and correct, and the if-statements are readable. Nice work.",
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
                Text = "Console calculator that reads two numbers and an operator (+, -, *, /) and prints the result. Loops so you can keep calculating until you type 'q'.",
                SubmittedAt = now.AddDays(-42),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "Division by zero isn't handled and crashes the program. Please add a check for that and resubmit.",
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
                Text = "Calculator supports add, subtract, multiply and divide. Wrapped the divide operation in a check so dividing by zero prints a message instead of crashing.",
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
                Text = "Modeled an IShape interface with Area() and Perimeter(), implemented by Circle, Rectangle and Triangle. Main builds a List<IShape> and loops over it to print each shape's numbers, to show the polymorphism working.",
                SubmittedAt = now.AddDays(-34),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Good use of interfaces and a clean class hierarchy - the polymorphism demo in Main is a nice touch. Nice work.",
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
                Text = "Created an IShape interface with Area() and Perimeter(), implemented by Circle, Square and Triangle. Each class validates its own inputs in the constructor, e.g. no negative side lengths.",
                SubmittedAt = now.AddDays(-33),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Well modeled and easy to extend, and I like the constructor validation. Nice work.",
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
                Text = "Shape class with a ShapeType field and if-statements that calculate area and perimeter based on the type. Handles circle, square and triangle.",
                SubmittedAt = now.AddDays(-32),
                ReviewStatus = SubmissionReviewStatus.NeedsCompletion,
                Feedback = "The math works, but there's no interface here, just one class with if-statements. Please refactor into an interface with a class per shape and resubmit.",
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
                Text = "Injected ILogger<T> into each controller. Logging Information on successful requests, Warning on validation failures, and the global exception handler logs unhandled exceptions at Error level with the stack trace.",
                SubmittedAt = now.AddDays(-11),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "Logging is consistent and covers all the endpoints, and the log levels are used sensibly. Nice work.",
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
                Text = "Added ILogger to the controllers and logged around the main actions - request received, entity created or not found, and any errors caught in the try/catch blocks.",
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
                Text = "Added page and pageSize query parameters to the GET list endpoints, defaulting to page 1 with a size of 10 and capped at 50. The response includes the total item count alongside the page of results. Sorry this one's late, got stuck on the total-count header for a while.",
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
                Text = "Implemented paging on the list endpoint with Skip/Take, page and pageSize as query parameters. Tested with a few different page sizes and an out-of-range page number to make sure it doesn't throw.",
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
                Text = "Built the Web API with GET, GET by id, POST, PUT and DELETE endpoints, backed by EF Core. Repo link is in the README along with instructions for running the migrations.",
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
                Text = "Web API with all five endpoints working against the database. Tested each one manually with Postman before submitting - collection is attached.",
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
                Text = "Implemented the Web API with the required endpoints. POST creates a new resource and returns 201 with the location header pointing at the new item.",
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
                Text = "Completed the Web API assignment - all endpoints are implemented, and I added a couple of extra query parameters for filtering on top of what was asked.",
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
                Text = "Web API with full CRUD support and proper status codes. Wrapped the database calls in try/catch so unexpected errors return a 500 with a message instead of crashing the app.",
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
                Text = "Made the changes from the last round of feedback and resubmitting now. Sorry this is a bit late, work got in the way this week.",
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
                Text = "Sorry this is late - finished the last two endpoints tonight. Everything should be working now, let me know if not.",
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
                Text = "Menu-driven console calculator with add, subtract, multiply, divide and a quit option. Runs in a loop so you can do multiple calculations without restarting the program.",
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
                Text = "Used LINQ to filter, sort and group the sample data set - examples with Where, OrderBy, GroupBy and Select in Program.cs, each with a comment explaining what it's doing. Sorry this is late, got stuck on the pagination task first.",
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
                Text = "Added JWT bearer authentication - the login endpoint issues a token, and [Authorize] is on the endpoints that need it. Sorry this is a bit late, the token expiry logic took longer than expected to get right.",
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
                Text = "Added integration tests using WebApplicationFactory for the main CRUD endpoints - covers the happy path plus a couple of error cases like a 404 on an unknown id.",
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
                Text = "Added data annotations to the request models for required fields and length limits. The API now returns 400 with the validation errors instead of a 500. Apologies for the late submission.",
                SubmittedAt = now.AddDays(-1),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                SubmissionId = ReactSubmissionId,
                ActivityId = ActivitySeeder.ReactTaskId,
                StudentId = UserSeeder.StudentThreeId,
                Text = "Built a small app with reusable Card, Button and List components. State is lifted to the parent and passed down via props, and each component has its own props interface.",
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
                Text = "Split the UI into a header, an item list and an item component, each with its own props interface. Used useState for the filter input and useMemo so the filtering doesn't run on every render.",
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
                Text = "Finished the component exercise - a bit late, ran into some trouble getting the click handler to update the right item's state yesterday.",
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
                Text = "Built the list, card and details-panel components from the assignment. The details panel updates when you click an item in the list.",
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
                Text = "Components are done and working - just didn't have time to add tests before the deadline. Sorry about that.",
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
                Text = "Completed the exercise with the required components. Used context instead of passing the shared state down through several layers of props.",
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
                Text = "Sorry this is late - built the components over the weekend instead of during the week. Everything renders correctly and updates on interaction.",
                SubmittedAt = now.AddDays(-1),
                ReviewStatus = SubmissionReviewStatus.Approved,
                Feedback = "This was late, but the work itself is good overall. Try to submit on time next round.",
                FeedbackByTeacherId = UserSeeder.TeacherId,
                FeedbackAt = now,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        // Fill in most of the remaining enrolled students on each overdue
        // task, reusing one already-written text/feedback pair per task, so
        // the overdue-submissions dashboard isn't dominated by every student
        // on every task. A handful of students (2, 20, 26, 30, 25, 27, 29)
        // are deliberately left out everywhere, so some overdue rows remain.
        int fillerSeq = 1;
        Submission Filler(
            Guid activityId, int studentNumber, string text, string feedback,
            int submittedDaysAgo, int feedbackDaysAgo) => new()
        {
            SubmissionId = Guid.Parse($"71000000-0000-0000-0000-{fillerSeq++:D12}"),
            ActivityId = activityId,
            StudentId = Student(studentNumber),
            Text = text,
            SubmittedAt = now.AddDays(submittedDaysAgo),
            ReviewStatus = SubmissionReviewStatus.Approved,
            Feedback = feedback,
            FeedbackByTeacherId = UserSeeder.TeacherId,
            FeedbackAt = now.AddDays(feedbackDaysAgo),
            CreatedAt = now,
            UpdatedAt = now
        };

        // Students 4, 6, 8, 10, 12, 14, 16, 18, 22, 24, 28 catch up on most
        // DotNet tasks; 17, 19, 21, 23 catch up on the React task.
        int[] commonFill = { 4, 6, 8, 10, 12, 14, 16, 18, 22, 24, 28 };
        var fillerGroups = new (
            Guid ActivityId, int[] StudentNumbers, string Text,
            string Feedback, int SubmittedDaysAgo, int FeedbackDaysAgo)[]
        {
            (ActivitySeeder.GitWorkflowTaskId,
                new[] { 4, 8, 10, 12, 14, 16, 18, 22, 24, 28 },
                "Cloned the starter repo, created a feature branch for each change, and opened a pull request for every one with a short description. Commit messages follow the conventional commits style we went over in class.",
                "Clean commit history, sensible branch names, and your PR descriptions actually explain the change. This is exactly the workflow we're after.",
                -54, -51),
            (ActivitySeeder.SimpleCalculatorTaskId, commonFill,
                "Calculator supports add, subtract, multiply and divide. Wrapped the divide operation in a check so dividing by zero prints a message instead of crashing.",
                "Handles edge cases well, including division by zero. Nice work.",
                -41, -39),
            (ActivitySeeder.ClassDesignTaskId,
                new[] { 6, 8, 10, 12, 14, 16, 18, 22, 24, 28 },
                "Created an IShape interface with Area() and Perimeter(), implemented by Circle, Square and Triangle. Each class validates its own inputs in the constructor, e.g. no negative side lengths.",
                "Well modeled and easy to extend, and I like the constructor validation. Nice work.",
                -33, -31),
            (ActivitySeeder.ConsoleCalculatorTaskId, commonFill,
                "Menu-driven console calculator with add, subtract, multiply, divide and a quit option. Runs in a loop so you can do multiple calculations without restarting the program.",
                "Clean and correct implementation. The menu loop and the arithmetic methods are both easy to follow. Nice work.",
                -26, -22),
            (ActivitySeeder.CollectionsExerciseTaskId, commonFill,
                "Used LINQ to filter, sort and group the sample data set - examples with Where, OrderBy, GroupBy and Select in Program.cs, each with a comment explaining what it's doing. Sorry this is late, got stuck on the pagination task first.",
                "This was late, but the LINQ queries are well done and the code is easy to read. Try to submit on time next round.",
                -16, -13),
            (ActivitySeeder.AspNetApiTaskId,
                new[] { 14, 16, 18, 22, 24, 28 },
                "Web API with full CRUD support and proper status codes. Wrapped the database calls in try/catch so unexpected errors return a 500 with a message instead of crashing the app.",
                "Solid implementation overall. The endpoints are well organized and the error handling is thorough. Nice work.",
                -13, -10),
            (ActivitySeeder.JwtAuthenticationTaskId, commonFill,
                "Added JWT bearer authentication - the login endpoint issues a token, and [Authorize] protects the endpoints that need it.",
                "Works correctly and follows the pattern we went over in class. Nice work.",
                -7, -4),
            (ActivitySeeder.InputValidationTaskId, commonFill,
                "Added data annotations to the request models for required fields and length limits, and the API now returns 400 with the validation errors instead of a 500.",
                "Validation is thorough and the error responses are clear. Nice work.",
                -5, -2),
            (ActivitySeeder.LoggingTaskId,
                new[] { 4, 6, 10, 12, 14, 16, 18, 22, 24, 28 },
                "Injected ILogger<T> into each controller. Logging Information on successful requests, Warning on validation failures, and the global exception handler logs unhandled exceptions at Error level with the stack trace.",
                "Logging is consistent and covers all the endpoints, and the log levels are used sensibly. Nice work.",
                -11, -8),
            (ActivitySeeder.PaginationTaskId,
                new[] { 4, 8, 10, 12, 14, 16, 18, 22, 24, 28 },
                "Added page and pageSize query parameters to the GET list endpoints, defaulting to page 1 with a size of 10 and capped at 50. The response includes the total item count alongside the page of results. Sorry this one's late, got stuck on the total-count header for a while.",
                "This was late, but the pagination works well, including the edge cases. Try to submit on time next round.",
                -5, -2),
            (ActivitySeeder.ReactTaskId,
                new[] { 17, 19, 21, 23 },
                "Split the UI into a header, an item list and an item component, each with its own props interface. Used useState for the filter input and useMemo so the filtering doesn't run on every render.",
                "Great component structure and consistent naming throughout. Nice work.",
                -7, -4)
        };

        foreach (var group in fillerGroups)
        {
            foreach (int studentNumber in group.StudentNumbers)
            {
                submissions.Add(Filler(
                    group.ActivityId, studentNumber, group.Text,
                    group.Feedback, group.SubmittedDaysAgo,
                    group.FeedbackDaysAgo));
            }
        }

        context.Submissions.AddRange(submissions);

        await context.SaveChangesAsync();
    }
}
