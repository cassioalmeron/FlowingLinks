using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FlowingLinks.Core.Models;

namespace FlowingLinks.Core.Configurations;

public abstract class ConfigurationBase<T> : IEntityTypeConfiguration<T> where T : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Configure Id as identity (auto-increment) for all entities
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();
            
        // Call the derived class configuration
        ConfigureEntity(builder);
    }
    
    /// <summary>
    /// Override this method to configure specific entity properties
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}