using LibraryOnlineRentalSystem.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryOnlineRentalSystem.Repository.UserRepository;

public class ConfigUserEntityType : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.AsGuid(),
                guid => new UserID(guid))
            .IsRequired();

        builder.OwnsOne(u => u.Name)
            .Property(n => n.FullName)
            .HasColumnName("Name")
            .IsRequired();

        builder.OwnsOne(u => u.Email)
            .Property(e => e.EmailAddress)
            .HasColumnName("Email")
            .IsRequired();

        builder.OwnsOne(u => u.UserName)
            .Property(un => un.Tag)
            .HasColumnName("UserName")
            .IsRequired();

        builder.OwnsOne(u => u.PhoneNumber)
            .Property(p => p.Number)
            .HasColumnName("PhoneNumber")
            .IsRequired();

        builder.OwnsOne(u => u.Nif)
            .Property(n => n.TaxID)
            .HasColumnName("NIF")
            .IsRequired();

        builder.OwnsOne(u => u.Biography)
            .Property(b => b.Description)
            .HasColumnName("Biography");

        builder.ToTable("Users");
    }
}
