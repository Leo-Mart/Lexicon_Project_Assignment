using LMS.Api.Constants;
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Api.Data.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasKey(activity => activity.ActivityId);

        builder.Property(activity => activity.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(activity => activity.Name)
            .IsRequired()
            .HasMaxLength(ModelConstants.ActivityNameMaxLength);

        builder.Property(activity => activity.Description)
            .IsRequired()
            .HasMaxLength(ModelConstants.DescriptionMaxLength);

        builder.Property(activity => activity.StartAt)
            .IsRequired();

        builder.Property(activity => activity.EndAt)
            .IsRequired();

        builder.Property(activity => activity.CreatedAt)
            .IsRequired();

        builder.Property(activity => activity.UpdatedAt)
            .IsRequired();

        builder.Property(activity => activity.Deadline)
            .IsRequired(false);

        builder.HasMany(activity => activity.Submissions)
            .WithOne(submission => submission.Activity)
            .HasForeignKey(submission => submission.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
