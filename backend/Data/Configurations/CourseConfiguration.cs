using LMS.Constants;

namespace LMS.Data.Configuration;
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(course => course.CourseId);

        builder.Property(course => course.Name)
            .IsRequired()
            .HasMaxLength(ModelConstants.CourseNameMaxLength);

        builder.Property(course => course.Description)
            .IsRequired()
            .HasMaxLength(ModelConstants.DescriptionMaxLength);

        builder.Property(course => course.StartDate)
            .IsRequired();

        builder.Property(course => course.EndDate)
            .IsRequired();

        builder.Property(course => course.CreatedAt)
            .IsRequired();

        builder.Property(course => course.UpdatedAt)
            .IsRequired();

        builder.HasMany(course => course.Modules)
            .WithOne(module => module.Course)
            .HasForeignKey(module => module.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(course => course.Enrollments)
            .WithOne(enrollment => enrollment.Course)
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}