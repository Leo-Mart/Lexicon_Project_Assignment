using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Api.Data.Configurations;

public class ActivityResourceConfiguration : IEntityTypeConfiguration<ActivityResource>
{
    public void Configure(EntityTypeBuilder<ActivityResource> builder)
    {
        builder.ToTable("ActivityResources");

        builder.HasKey(activityResource => new
        {
            activityResource.ActivityId,
            activityResource.ResourceId
        });

        builder.HasOne(activityResource => activityResource.Activity)
            .WithMany(activity => activity.ActivityResources)
            .HasForeignKey(activityResource => activityResource.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(activityResource => activityResource.Resource)
            .WithMany(resource => resource.ActivityResources)
            .HasForeignKey(activityResource => activityResource.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
