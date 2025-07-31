using Microsoft.EntityFrameworkCore;
using FlowingLinks.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingLinks.Core.Configurations;

internal class LinkConfiguration : ConfigurationBase<Link>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Link> builder)
    {
        builder.ToTable("Link");
        
        // Configure primary key
        builder.HasKey(l => l.Id);
        
        // Configure unique index on URL per user
        builder.HasIndex(l => new { l.Url, l.UserId })
            .IsUnique();
            
        // Configure foreign key relationship with User
        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure Favorite field
        builder.Property(l => l.Favorite)
            .IsRequired()
            .HasDefaultValue(false);
    }
}