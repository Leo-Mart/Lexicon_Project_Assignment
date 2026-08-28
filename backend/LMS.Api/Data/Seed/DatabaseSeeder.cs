using LMS.Api.Data;

namespace LMS.Api.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(LMSDbContext context)
    {
        await UserSeeder.SeedAsync(context);
        await CourseSeeder.SeedAsync(context);
        await ModuleSeeder.SeedAsync(context);
        await ActivitySeeder.SeedAsync(context);
        await ResourceSeeder.SeedAsync(context);
        await EnrollmentSeeder.SeedAsync(context);
        await SubmissionSeeder.SeedAsync(context);
    }
}
