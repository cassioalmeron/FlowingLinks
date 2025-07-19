using Microsoft.EntityFrameworkCore;
using FlowingLinks.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingLinks.Core.Configurations;

internal class LinkLabelConfiguration : ConfigurationBase<LinkLabel>
{
    public override void Configure(EntityTypeBuilder<LinkLabel> builder)
    {
        builder.ToTable("LinkLabel");

        // Configure composite primary key (overrides the Id configuration from base)
        builder.HasKey(ll => new { ll.LinkId, ll.LabelId });

        // Configure foreign key relationships
        builder.HasOne(ll => ll.Link)
            .WithMany(l => l.LinkLabels)
            .HasForeignKey(ll => ll.LinkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ll => ll.Label)
            .WithMany(l => l.LinkLabels)
            .HasForeignKey(ll => ll.LabelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure unique combination of LinkId and LabelId
        builder.HasIndex(ll => new { ll.LinkId, ll.LabelId })
            .IsUnique();
    }
    
    protected override void ConfigureEntity(EntityTypeBuilder<LinkLabel> builder)
    {
        // No additional configuration needed for LinkLabel
    }
} 