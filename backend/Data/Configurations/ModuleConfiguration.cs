using LMS.Constants;

namespace LMS.Data.Configuration;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules");

        builder.HasKey(module => module.ModuleId);

        builder.Property(module => module.Name)
            .IsRequired()
            .HasMaxLength(ModelConstants.ModuleNameMaxLength);

        builder.Property(module => module.Description)
            .IsRequired()
            .HasMaxLength(ModelConstants.DescriptionMaxLength);

        builder.Property(module => module.StartDate)
            .IsRequired();

        builder.Property(module => module.EndDate)
            .IsRequired();

        builder.Property(module => module.CreatedAt)
            .IsRequired();

        builder.Property(module => module.UpdatedAt)
            .IsRequired();

        builder.HasMany(module => module.Activities)
            .WithOne(activity => activity.Module)
            .HasForeignKey(activity => activity.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}