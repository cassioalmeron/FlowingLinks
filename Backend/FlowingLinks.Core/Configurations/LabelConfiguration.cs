using Microsoft.EntityFrameworkCore;
using FlowingLinks.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingLinks.Core.Configurations;

internal class LabelConfiguration : ConfigurationBase<Label>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Label");

        builder.HasIndex(u => u.Name)
            .IsUnique();
    }
}