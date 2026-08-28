using LMS.Constants;

namespace LMS.Data.Configuration;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");

        builder.HasKey(resource => resource.ResourceId);

        builder.Property(resource => resource.Name)
            .IsRequired()
            .HasMaxLength(ModelConstants.ResourceNameMaxLength);

        builder.Property(resource => resource.Description)
            .IsRequired()
            .HasMaxLength(ModelConstants.DescriptionMaxLength);

       builder.Property(resource => resource.Content)
            .HasMaxLength(ModelConstants.LongTextMaxLength)
            .IsRequired(false);

        builder.Property(resource => resource.Uri)
            .HasMaxLength(ModelConstants.UriMaxLength)
            .IsRequired(false);

        builder.Property(resource => resource.CreatedAt)
            .IsRequired();

        builder.Property(resource => resource.UpdatedAt)
            .IsRequired();

        builder.HasOne(resource => resource.CreatedByTeacher)
            .WithMany(user => user.CreatedResources)
            .HasForeignKey(resource => resource.CreatedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
