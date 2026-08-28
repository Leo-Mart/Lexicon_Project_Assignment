using LMS.Api.Constants;
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Api.Data.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(submission => submission.SubmissionId);

        builder.Property(submission => submission.Text)
            .IsRequired()
            .HasMaxLength(ModelConstants.DescriptionMaxLength);

        builder.Property(submission => submission.SubmittedAt)
            .IsRequired();

        builder.Property(submission => submission.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(submission => submission.Feedback)
            .HasMaxLength(ModelConstants.DescriptionMaxLength)
            .IsRequired(false);

        builder.Property(submission => submission.FeedbackByTeacherId)
            .IsRequired(false);

        builder.Property(submission => submission.FeedbackAt)
            .IsRequired(false);

        builder.Property(submission => submission.CreatedAt)
            .IsRequired();

        builder.Property(submission => submission.UpdatedAt)
            .IsRequired();

        builder.HasOne(submission => submission.Activity)
            .WithMany(activity => activity.Submissions)
            .HasForeignKey(submission => submission.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(submission => submission.Student)
            .WithMany(user => user.Submissions)
            .HasForeignKey(submission => submission.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(submission => submission.FeedbackByTeacher)
            .WithMany(user => user.FeedbackSubmissions)
            .HasForeignKey(submission => submission.FeedbackByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
