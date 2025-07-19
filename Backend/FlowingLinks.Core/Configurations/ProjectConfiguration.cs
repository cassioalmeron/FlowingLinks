using Microsoft.EntityFrameworkCore;
using FlowingLinks.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingLinks.Core.Configurations;

internal class ProjectConfiguration : ConfigurationBase<Project>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Project");

        builder.HasIndex(p => new { p.UserId, p.Name })
            .IsUnique();
    }
}
