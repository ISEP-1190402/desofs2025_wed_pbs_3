using LibraryOnlineRentalSystem.Domain.Rentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryOnlineRentalSystem.Repository.RentalRepository;

public class ConfigRentalEntityType : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(b => b.Id);

        builder.OwnsOne(b => b.StartDate,
            startDate => { startDate.Property("StartDateTime").IsRequired(); });

        builder.OwnsOne(b => b.EndDate,
            endDate => { endDate.Property("EndDateTime").IsRequired(); });
        builder.OwnsOne(b => b.RentedBookIdentifier,
            rentedBookIdentifier => { rentedBookIdentifier.Property("BookId").IsRequired(); });
        builder.OwnsOne(b => b.StatusOfRental,
            status => { status.Property("RentStatus").IsRequired(); });
        builder.OwnsOne(b => b.EmailUser,
            emailAddress => { emailAddress.Property("EmailAddress").IsRequired(); });
    }
}