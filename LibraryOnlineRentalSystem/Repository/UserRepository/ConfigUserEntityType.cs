using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryOnlineRentalSystem.Repository.UserRepository;

namespace LibraryOnlineRentalSystem.Repository.UserRepository;

public class ConfigUserEntityType : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Name)
            .HasConversion(
                name => name.name,
                value => new Name(value))
            .IsRequired();
            
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.email,
                value => new Email(value))
            .IsRequired();
            
        builder.Property(u => u.RoleId)
            .HasConversion(
                roleId => roleId.AsString(),
                value => new RoleId(value))
            .IsRequired();
            
        builder.Property(u => u.UserName)
            .HasConversion(
                userName => userName.username,
                value => new UserName(value))
            .IsRequired();
            
        builder.Property(u => u.PhoneNumber)
            .HasConversion(
                phoneNumber => phoneNumber.Number,
                value => new PhoneNumber(value))
            .IsRequired();
            
        builder.Property(u => u.Nif)
            .HasConversion(
                nif => nif.nif,
                value => new NIF(value))
            .IsRequired();
            
        builder.Property(u => u.Biography)
            .HasConversion(
                biography => biography.biography,
                value => new Biography(value))
            .IsRequired();
            
        // Add unique constraints
        builder.HasIndex(u => u.Email.email).IsUnique();
        builder.HasIndex(u => u.UserName.username).IsUnique();
        builder.HasIndex(u => u.Nif.nif).IsUnique();
    }
} 