using LMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Api.Data.Configurations;

public class ModuleResourceConfiguration : IEntityTypeConfiguration<ModuleResource>
{
    public void Configure(EntityTypeBuilder<ModuleResource> builder)
    {
        builder.ToTable("ModuleResources");

        builder.HasKey(moduleResource => new
        {
            moduleResource.ModuleId,
            moduleResource.ResourceId
        });

        builder.HasOne(moduleResource => moduleResource.Module)
            .WithMany(module => module.ModuleResources)
            .HasForeignKey(moduleResource => moduleResource.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(moduleResource => moduleResource.Resource)
            .WithMany(resource => resource.ModuleResources)
            .HasForeignKey(moduleResource => moduleResource.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
