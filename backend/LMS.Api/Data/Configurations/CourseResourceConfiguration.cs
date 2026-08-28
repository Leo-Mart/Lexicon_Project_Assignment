
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Api.Data.Configurations;

public class CourseResourceConfiguration : IEntityTypeConfiguration<CourseResource>
{
    public void Configure(EntityTypeBuilder<CourseResource> builder)
    {
        builder.ToTable("CourseResources");

        builder.HasKey(courseResource => new
        {
            courseResource.CourseId,
            courseResource.ResourceId
        });

        builder.HasOne(courseResource => courseResource.Course)
            .WithMany(course => course.CourseResources)
            .HasForeignKey(courseResource => courseResource.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(courseResource => courseResource.Resource)
            .WithMany(resource => resource.CourseResources)
            .HasForeignKey(courseResource => courseResource.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
