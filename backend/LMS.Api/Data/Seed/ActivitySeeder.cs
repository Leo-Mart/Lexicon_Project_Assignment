using LMS.Api.Enums.Model;
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class ActivitySeeder
{
    public static readonly Guid CSharpLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000001");

    public static readonly Guid CSharpPracticeId =
        Guid.Parse("50000000-0000-0000-0000-000000000002");

    public static readonly Guid AspNetApiTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000003");

    public static readonly Guid EntityFrameworkLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000004");

    public static readonly Guid TypeScriptPracticeId =
        Guid.Parse("50000000-0000-0000-0000-000000000005");

    public static readonly Guid ReactTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000006");

    // Extra DotNet-course tasks so one student (Erik) can show every review
    // state on its own unique activity.
    public static readonly Guid ConsoleCalculatorTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000007");

    public static readonly Guid CollectionsExerciseTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000008");

    public static readonly Guid JwtAuthenticationTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000009");

    public static readonly Guid IntegrationTestsTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000010");

    public static readonly Guid DatabaseMigrationsTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000011");

    public static readonly Guid QueryOptimizationTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000012");

    public static readonly Guid InputValidationTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000013");

    // Activities for the three new finished modules leading up to C#
    // Fundamentals.
    public static readonly Guid GitLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000014");

    public static readonly Guid GitWorkflowTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000015");

    public static readonly Guid ProgrammingBasicsLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000016");

    public static readonly Guid ControlFlowPracticeId =
        Guid.Parse("50000000-0000-0000-0000-000000000017");

    public static readonly Guid SimpleCalculatorTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000018");

    public static readonly Guid OopLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000019");

    public static readonly Guid ClassDesignTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000020");

    // Two more ASP.NET Core tasks so that module alone covers every review
    // state (the other four only cover needs-completion/not-reviewed).
    public static readonly Guid LoggingTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000021");

    public static readonly Guid PaginationTaskId =
        Guid.Parse("50000000-0000-0000-0000-000000000022");

    // ASP.NET Core only had tasks - add a lecture, a practice and a code
    // review session so it has the same variety as the other modules.
    public static readonly Guid AspNetCoreLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000023");

    public static readonly Guid AspNetCorePracticeId =
        Guid.Parse("50000000-0000-0000-0000-000000000024");

    public static readonly Guid AspNetCoreCodeReviewId =
        Guid.Parse("50000000-0000-0000-0000-000000000025");

    // Lectures start at a realistic 10:30, not whatever time the seeder
    // happened to run at.
    private static DateTime LectureStart(DateTime now, int dayOffset) =>
        now.Date.AddDays(dayOffset).AddHours(10).AddMinutes(30);
    private static DateTime LectureEnd(DateTime now, int dayOffset) =>
   now.Date.AddDays(dayOffset).AddHours(12).AddMinutes(00);
    private static DateTime TaskStart(DateTime now, int dayOffset) =>
        now.Date.AddDays(dayOffset).AddHours(13).AddMinutes(00);
    private static DateTime TaskEnd(DateTime now, int dayOffset) =>
 now.Date.AddDays(dayOffset).AddHours(23).AddMinutes(59);
    // A lecture ahead of each of the topic-specific tasks, not just the one
    // at the very start of the module.
    public static readonly Guid AspNetCoreAuthLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000026");

    public static readonly Guid AspNetCoreTestingLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000027");

    public static readonly Guid AspNetCoreValidationLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000028");

    public static readonly Guid AspNetCoreLoggingLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000029");

    public static readonly Guid AspNetCorePaginationLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000030");

    public static readonly Guid AspNetCoreRoutingLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000031");

    public static readonly Guid AspNetCoreDiConfigLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000032");

    public static readonly Guid AspNetCoreMiddlewareLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000033");

    public static readonly Guid AspNetCoreErrorHandlingLectureId =
        Guid.Parse("50000000-0000-0000-0000-000000000034");

    public static readonly Guid AspNetCoreIdentity =
        Guid.Parse("50000000-0000-0000-0000-000000000035");
    public static readonly Guid AspNetCoreSeeding =
          Guid.Parse("50000000-0000-0000-0000-000000000036");

    public static async Task SeedAsync(LMSDbContext context)
    {
        if (await context.Activities.AnyAsync())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        var activities = new List<Activity>
        {
            new()
            {
                ActivityId = GitLectureId,
                ModuleId = ModuleSeeder.GitModuleId,
                Type = ActivityType.Lecture,
                Name = "Introduction to Git",
                Description = "Introduction to version control, commits, branches and pull requests.",
                StartAt = LectureStart(now, -57),
                EndAt = LectureEnd(now, -57),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = GitWorkflowTaskId,
                ModuleId = ModuleSeeder.GitModuleId,
                Type = ActivityType.Task,
                Name = "Git Workflow Exercise",
                Description = "Practice branching, committing and opening a pull request.",
                StartAt = TaskStart(now, -52),
                EndAt = TaskEnd(now, -52),
                Deadline = TaskEnd(now, -52),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = ProgrammingBasicsLectureId,
                ModuleId = ModuleSeeder.ProgrammingFundamentalsModuleId,
                Type = ActivityType.Lecture,
                Name = "Programming Basics",
                Description = "Introduction to variables, control flow and problem solving.",
                StartAt = LectureStart(now, -47),
                EndAt = LectureEnd(now, -47),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = ControlFlowPracticeId,
                ModuleId = ModuleSeeder.ProgrammingFundamentalsModuleId,
                Type = ActivityType.Practice,
                Name = "Control Flow Practice",
                Description = "Practice conditions and loops with small problems.",
                StartAt = now.AddDays(-45),
                EndAt = now.AddDays(-45).AddHours(3),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = SimpleCalculatorTaskId,
                ModuleId = ModuleSeeder.ProgrammingFundamentalsModuleId,
                Type = ActivityType.Task,
                Name = "Simple Calculator",
                Description = "Build a simple calculator that reads two numbers and an operator.",
                StartAt = TaskStart(now, -40),
                EndAt = TaskEnd(now, -40),
                Deadline = TaskEnd(now, -40),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = OopLectureId,
                ModuleId = ModuleSeeder.ObjectOrientedProgrammingModuleId,
                Type = ActivityType.Lecture,
                Name = "OOP Concepts",
                Description = "Classes, interfaces, inheritance and polymorphism in C#.",
                StartAt = LectureStart(now, -37),
                EndAt = LectureEnd(now, -37),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = ClassDesignTaskId,
                ModuleId = ModuleSeeder.ObjectOrientedProgrammingModuleId,
                Type = ActivityType.Task,
                Name = "Class Design Exercise",
                Description = "Model a small domain using classes, interfaces and inheritance.",
                StartAt = TaskStart(now, -31),
                EndAt = TaskEnd(now, -31),
                Deadline = TaskEnd(now, -31),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = CSharpLectureId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Lecture,
                Name = "Introduction to C#",
                Description = "Introduction to C# syntax, types and basic language features.",
                StartAt = new DateTime(2026, 8, 18, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 8, 18, 12, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = CSharpPracticeId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Practice,
                Name = "C# Practice",
                Description = "Practice variables, conditions, loops and methods.",
                StartAt = new DateTime(2026, 8, 20, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 8, 20, 15, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Deadlines below are relative to now, not hardcoded calendar
            // dates, so they and their submissions never drift into the
            // future no matter when this gets seeded.
            new()
            {
                ActivityId = ConsoleCalculatorTaskId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Task,
                Name = "Console Calculator",
                Description = "Build a console app that performs basic arithmetic using methods.",
                StartAt = TaskStart(now, -24),
                EndAt = TaskEnd(now, -24),
                Deadline = TaskEnd(now, -24),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = CollectionsExerciseTaskId,
                ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                Type = ActivityType.Task,
                Name = "Collections Exercise",
                Description = "Work with lists, dictionaries and LINQ to solve small data problems.",
                StartAt = TaskStart(now, -18),
                EndAt = TaskEnd(now, -18),
                Deadline = TaskEnd(now, -18),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Introduction to ASP.NET Core",
                Description = "Introduction to controllers, routing and dependency injection in ASP.NET Core.",
                StartAt = LectureStart(now, -14),
                EndAt = LectureEnd(now, -14),
                CreatedAt = now,
                UpdatedAt = now
            },
            // new()
            // {
            //     ActivityId = LoggingTaskId,
            //     ModuleId = ModuleSeeder.AspNetCoreModuleId,
            //     Type = ActivityType.Task,
            //     Name = "Add Logging",
            //     Description = "Add structured logging to the Web API's endpoints.",
            //     StartAt = now.AddDays(-13),
            //     EndAt = now.AddDays(-9),
            //     Deadline = now.AddDays(-9),
            //     CreatedAt = now,
            //     UpdatedAt = now
            // },

            // Two lectures a week for the module's run - roughly one every
            // other day.
            new()
            {
                ActivityId = AspNetCoreRoutingLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Routing & Controllers",
                Description = "Attribute routing, route parameters and organizing endpoints across controllers.",
                StartAt = LectureStart(now, -12),
                EndAt = LectureEnd(now, -12),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreDiConfigLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Dependency Injection & Configuration",
                Description = "Service lifetimes, the built-in DI container, and reading configuration and secrets.",
                StartAt = LectureStart(now, -10),
                EndAt = LectureEnd(now, -10),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreAuthLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Authentication & Authorization",
                Description = "Securing a Web API with JWT bearer tokens and role-based authorization.",
                StartAt = LectureStart(now, -8),
                EndAt = LectureEnd(now, -8),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreMiddlewareLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Middleware & the Request Pipeline",
                Description = "How a request flows through middleware, and where to hook in custom behavior.",
                StartAt = LectureStart(now, -6),
                EndAt = LectureEnd(now, -6),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreValidationLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Input Validation & Model Binding",
                Description = "Validating incoming request models with data annotations and returning clear errors.",
                StartAt = LectureStart(now, -4),
                EndAt = LectureEnd(now, -4),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreLoggingLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Structured Logging",
                Description = "Using ILogger, log levels, and structured logging for a Web API.",
                StartAt = LectureStart(now, -2),
                EndAt = LectureEnd(now, -2),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCorePaginationLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Pagination & Filtering Large Result Sets",
                Description = "Paging list endpoints with page/pageSize query parameters and a total count.",
                StartAt = LectureStart(now, 0),
                EndAt = LectureEnd(now, 0),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreTestingLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Testing ASP.NET Core APIs",
                Description = "Integration testing a Web API with WebApplicationFactory.",
                StartAt = LectureStart(now, 1),
                EndAt = LectureEnd(now, 1),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreErrorHandlingLectureId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Lecture,
                Name = "Error Handling & Problem Details",
                Description = "Centralized exception handling and returning RFC 7807 Problem Details responses.",
                StartAt = LectureStart(now, 4),
                EndAt = LectureEnd(now, 4),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCorePracticeId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Routing & Dependency Injection Practice",
                Description = "Practice defining routes and registering services with the built-in DI container.",
                StartAt = TaskStart(now, -5),
                EndAt = TaskEnd(now, -5),
                Deadline = TaskEnd(now, -5),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetApiTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Build a Web API",
                Description = "Create a small ASP.NET Core Web API.",
                StartAt = TaskStart(now, -8),
                EndAt = TaskEnd(now, -8),
                Deadline = TaskEnd(now, -8),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = AspNetCoreCodeReviewId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Other,
                Name = "Web API Code Review Session",
                Description = "Live review of common mistakes seen in the Web API submissions so far.",
                StartAt = LectureStart(now, -9),
                EndAt = LectureEnd(now, -9),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                ActivityId = AspNetCoreIdentity,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Add Identity Authentication",
                Description = "Wire up ASP.NET Core Identity for user accounts, replacing the manual JWT login flow.",
                StartAt = TaskStart(now, 3),
                EndAt = TaskEnd(now, 3),
                Deadline = TaskEnd(now, 3),
                CreatedAt = now,
                UpdatedAt = now
            },
               new()
            {
                ActivityId = AspNetCoreSeeding,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Database Seeding",
                Description = "Write an EF Core seeder that inserts sample data on startup if the database is empty.",
                StartAt = TaskStart(now, 0),
                EndAt = TaskEnd(now, 0),
                Deadline = TaskEnd(now, 0),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = JwtAuthenticationTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Add JWT Authentication",
                Description = "Secure the Web API with JWT-based authentication and authorization.",
                StartAt = TaskStart(now, -6),
                EndAt = TaskEnd(now, -6),
                Deadline = TaskEnd(now, -6),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = InputValidationTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Add Input Validation",
                Description = "Add request validation to the Web API's endpoints.",
                StartAt = TaskStart(now, -4),
                EndAt = TaskEnd(now, -4),
                Deadline = TaskEnd(now, -4),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = LoggingTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Add Logging",
                Description = "Add structured logging to the Web API's endpoints.",
                StartAt = TaskStart(now, -9),
                EndAt = TaskEnd(now, -9),
                Deadline = TaskEnd(now, -9),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                ActivityId = PaginationTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Add Pagination",
                Description = "Add pagination to the Web API's list endpoints.",
                StartAt = TaskStart(now, -7),
                EndAt = TaskEnd(now, -7),
                Deadline = TaskEnd(now, -7),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = IntegrationTestsTaskId,
                ModuleId = ModuleSeeder.AspNetCoreModuleId,
                Type = ActivityType.Task,
                Name = "Write Integration Tests",
                Description = "Add integration tests for the Web API's endpoints.",
                StartAt = TaskStart(now, 1),
                EndAt = TaskEnd(now, 1),
                Deadline = TaskEnd(now, 1),
                CreatedAt = now,
                UpdatedAt = now
            },

            // Entity Framework Core hasn't started yet (see ModuleSeeder),
            // so nothing in it has a submission - both tasks are still
            // open, and the lecture kicks off right when the module does.
            new()
            {
                ActivityId = EntityFrameworkLectureId,
                ModuleId = ModuleSeeder.EntityFrameworkModuleId,
                Type = ActivityType.Lecture,
                Name = "Entity Framework Core Introduction",
                Description = "Introduction to EF Core, DbContext and migrations.",
                StartAt = LectureStart(now, 13),
                EndAt = LectureEnd(now, 13),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = DatabaseMigrationsTaskId,
                ModuleId = ModuleSeeder.EntityFrameworkModuleId,
                Type = ActivityType.Task,
                Name = "Database Migrations",
                Description = "Create and apply EF Core migrations for a small schema.",
                StartAt = TaskStart(now, 25),
                EndAt = TaskEnd(now, 25),
                Deadline = TaskEnd(now, 25),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = QueryOptimizationTaskId,
                ModuleId = ModuleSeeder.EntityFrameworkModuleId,
                Type = ActivityType.Task,
                Name = "Query Optimization",
                Description = "Write efficient EF Core LINQ queries and inspect the generated SQL.",
                StartAt = TaskStart(now, 32),
                EndAt = TaskEnd(now, 32),
                Deadline = TaskEnd(now, 32),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = TypeScriptPracticeId,
                ModuleId = ModuleSeeder.TypeScriptModuleId,
                Type = ActivityType.Practice,
                Name = "TypeScript Practice",
                Description = "Practice interfaces, types and functions in TypeScript.",
                StartAt = new DateTime(2026, 8, 24, 9, 0, 0, DateTimeKind.Utc),
                EndAt = new DateTime(2026, 8, 24, 15, 0, 0, DateTimeKind.Utc),
                CreatedAt = now,
                UpdatedAt = now
            },

            new()
            {
                ActivityId = ReactTaskId,
                ModuleId = ModuleSeeder.ReactModuleId,
                Type = ActivityType.Task,
                Name = "React Component Exercise",
                Description = "Build a small React application using reusable components.",
                StartAt = TaskStart(now, -3),
                EndAt = TaskEnd(now, -3),
                Deadline = TaskEnd(now, -3),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Activities.AddRange(activities);

        await context.SaveChangesAsync();
    }
}
