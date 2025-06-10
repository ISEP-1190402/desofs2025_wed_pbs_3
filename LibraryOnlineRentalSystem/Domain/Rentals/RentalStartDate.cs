namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalStartDate
{
    public RentalStartDate()
    {
    }

    public RentalStartDate(DateTime startDateTime)
    {
        StartDateTime = startDateTime;
    }

    public DateTime StartDateTime { get; set; }
}