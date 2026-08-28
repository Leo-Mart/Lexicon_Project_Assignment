using LMS.Api.Constants;
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.UserId);

        builder.Property(user => user.Name)
            .IsRequired()
            .HasMaxLength(ModelConstants.UserNameMaxLength);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(ModelConstants.EmailMaxLength);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(ModelConstants.PasswordHashMaxLength);

        builder.Property(user => user.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt)
            .IsRequired();

        builder.HasMany(user => user.CreatedResources)
            .WithOne(resource => resource.CreatedByTeacher)
            .HasForeignKey(resource => resource.CreatedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(user => user.Submissions)
            .WithOne(submission => submission.Student)
            .HasForeignKey(submission => submission.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(user => user.FeedbackSubmissions)
            .WithOne(submission => submission.FeedbackByTeacher)
            .HasForeignKey(submission => submission.FeedbackByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
