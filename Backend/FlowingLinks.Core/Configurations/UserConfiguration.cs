using Microsoft.EntityFrameworkCore;
using FlowingLinks.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingLinks.Core.Configurations;

internal class UserConfiguration : ConfigurationBase<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasIndex(u => u.Username)
            .IsUnique();
    }
}
