using LibraryOnlineRentalSystem.Domain.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryOnlineRentalSystem.Repository.RoleRepository;

public class ConfigRoleEntityType : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.AsGuid(),
                guid => new RoleId(guid))
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.ToTable("Roles");
    }
} 