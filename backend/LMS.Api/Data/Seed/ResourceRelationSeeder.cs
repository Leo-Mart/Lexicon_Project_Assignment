using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Data.Seed;

public static class ResourceRelationSeeder
{
    public static async Task SeedAsync(LMSDbContext context)
    {
        if (!await context.CourseResources.AnyAsync())
        {
            List<CourseResource> courseResources =
            [
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.CSharpGuideResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.EntityFrameworkGuideResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.FrontendCourseId,
                    ResourceId = ResourceSeeder.TypeScriptGuideResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.FrontendCourseId,
                    ResourceId = ResourceSeeder.ReactGuideResourceId
                }
            ];

            context.CourseResources.AddRange(courseResources);
        }

        if (!await context.ModuleResources.AnyAsync())
        {
            List<ModuleResource> moduleResources =
            [
                new()
                {
                    ModuleId = ModuleSeeder.CSharpBasicsModuleId,
                    ResourceId = ResourceSeeder.CSharpGuideResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.AspNetCoreModuleId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.EntityFrameworkModuleId,
                    ResourceId = ResourceSeeder.EntityFrameworkGuideResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.TypeScriptModuleId,
                    ResourceId = ResourceSeeder.TypeScriptGuideResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.ReactModuleId,
                    ResourceId = ResourceSeeder.ReactGuideResourceId
                }
            ];

            context.ModuleResources.AddRange(moduleResources);
        }

        if (!await context.ActivityResources.AnyAsync())
        {
            List<ActivityResource> activityResources =
            [
                new()
                {
                    ActivityId = ActivitySeeder.CSharpLectureId,
                    ResourceId = ResourceSeeder.CSharpGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.CSharpPracticeId,
                    ResourceId = ResourceSeeder.CSharpGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetApiTaskId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.EntityFrameworkLectureId,
                    ResourceId = ResourceSeeder.EntityFrameworkGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.TypeScriptPracticeId,
                    ResourceId = ResourceSeeder.TypeScriptGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.ReactTaskId,
                    ResourceId = ResourceSeeder.ReactGuideResourceId
                }
            ];

            context.ActivityResources.AddRange(activityResources);
        }

        await context.SaveChangesAsync();
    }
}
