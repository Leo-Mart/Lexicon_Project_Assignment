
using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Api.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(enrollment => new
        {
            enrollment.StudentId,
            enrollment.CourseId
        });

        builder.HasIndex(enrollment => enrollment.StudentId)
            .IsUnique();

        builder.Property(enrollment => enrollment.EnrolledAt)
            .IsRequired();

        builder.HasOne(enrollment => enrollment.Student)
            .WithOne(user => user.Enrollment)
            .HasForeignKey<Enrollment>(enrollment => enrollment.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(enrollment => enrollment.Course)
            .WithMany(course => course.Enrollments)
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
