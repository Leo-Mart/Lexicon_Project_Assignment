using Microsoft.AspNetCore.Identity;

namespace LMS.Api.Data.Seed;

public static class RoleSeeder
{
    public const string TeacherRole = "Teacher";
    public const string StudentRole = "Student";

    public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(TeacherRole))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(TeacherRole));
        }

        if (!await roleManager.RoleExistsAsync(StudentRole))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(StudentRole));
        }
    }
}
