using LMS.Api.Data;
using LMS.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS.Api.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
       LMSDbContext context,
       UserManager<User> userManager,
       RoleManager<IdentityRole<Guid>> roleManager)
    {
        await RoleSeeder.SeedAsync(roleManager);
        await UserSeeder.SeedAsync(userManager);
        await CourseSeeder.SeedAsync(context);
        await ModuleSeeder.SeedAsync(context);
        await ActivitySeeder.SeedAsync(context);
        await ResourceSeeder.SeedAsync(context);
        await EnrollmentSeeder.SeedAsync(context);
        await SubmissionSeeder.SeedAsync(context);
    }
}
