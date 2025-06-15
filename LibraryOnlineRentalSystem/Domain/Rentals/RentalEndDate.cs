namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalEndDate
{
    public RentalEndDate()
    {
    }

    public RentalEndDate(DateTime endDateTime)
    {
        EndDateTime = endDateTime;
    }

    public DateTime EndDateTime { get; set; }
}