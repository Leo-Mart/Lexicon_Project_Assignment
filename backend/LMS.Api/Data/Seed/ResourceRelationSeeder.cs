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
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.CourseSyllabusResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.GitGuideResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.OopGuideResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.AuthenticationGuideResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.TestingGuideResourceId
                },
                new()
                {
                    CourseId = CourseSeeder.DotNetCourseId,
                    ResourceId = ResourceSeeder.DockerGuideResourceId
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
                },
                new()
                {
                    ModuleId = ModuleSeeder.GitModuleId,
                    ResourceId = ResourceSeeder.GitGuideResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.ObjectOrientedProgrammingModuleId,
                    ResourceId = ResourceSeeder.OopGuideResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.AuthenticationModuleId,
                    ResourceId = ResourceSeeder.AuthenticationGuideResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.AutomatedTestingModuleId,
                    ResourceId = ResourceSeeder.TestingGuideResourceId
                },
                new()
                {
                    ModuleId = ModuleSeeder.DockerDeploymentModuleId,
                    ResourceId = ResourceSeeder.DockerGuideResourceId
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
                    ActivityId = ActivitySeeder.AspNetCoreLectureId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCorePracticeId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCoreAuthLectureId,
                    ResourceId = ResourceSeeder.AuthenticationGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCoreAuthLectureId,
                    ResourceId = ResourceSeeder.JwtGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCoreValidationLectureId,
                    ResourceId = ResourceSeeder.ModelValidationGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCoreLoggingLectureId,
                    ResourceId = ResourceSeeder.LoggingGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCorePaginationLectureId,
                    ResourceId = ResourceSeeder.PaginationGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCoreTestingLectureId,
                    ResourceId = ResourceSeeder.IntegrationTestingGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.AspNetCoreTestingLectureId,
                    ResourceId = ResourceSeeder.TestingGuideResourceId
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
                },
                new()
                {
                    ActivityId = ActivitySeeder.JwtAuthenticationTaskId,
                    ResourceId = ResourceSeeder.AuthenticationGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.JwtAuthenticationTaskId,
                    ResourceId = ResourceSeeder.JwtGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.InputValidationTaskId,
                    ResourceId = ResourceSeeder.ModelValidationGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.InputValidationTaskId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.LoggingTaskId,
                    ResourceId = ResourceSeeder.LoggingGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.LoggingTaskId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.PaginationTaskId,
                    ResourceId = ResourceSeeder.PaginationGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.PaginationTaskId,
                    ResourceId = ResourceSeeder.AspNetDocumentationResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.IntegrationTestsTaskId,
                    ResourceId = ResourceSeeder.IntegrationTestingGuideResourceId
                },
                new()
                {
                    ActivityId = ActivitySeeder.IntegrationTestsTaskId,
                    ResourceId = ResourceSeeder.TestingGuideResourceId
                }
            ];

            context.ActivityResources.AddRange(activityResources);
        }

        await context.SaveChangesAsync();
    }
}
