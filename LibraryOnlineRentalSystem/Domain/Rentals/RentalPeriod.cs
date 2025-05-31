using System.Globalization;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalPeriod
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    public RentalPeriod(DateTime startDateTime, DateTime endDateTime)
    {
        if (IsPeriodValid(startDateTime,endDateTime))
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime; 
        }
    }

    private bool IsPeriodValid(DateTime startDateTime, DateTime endDateTime)
    {
        return startDateTime > DateTime.Now && endDateTime > startDateTime;
    }

}