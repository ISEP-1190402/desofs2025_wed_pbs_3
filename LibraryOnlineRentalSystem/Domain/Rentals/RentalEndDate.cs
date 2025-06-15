using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalEndDate : IValueObject
{
    public DateTime EndDateTime { get; private set; }

    protected RentalEndDate() { }

    
    public RentalEndDate(DateTime endDateTime, DateTime? startDateTime = null)
    {
        if (endDateTime < DateTime.UtcNow.AddMinutes(-5)) // Allow 5 minutes of clock skew
        {
            throw new BusinessRulesException("End date cannot be in the past");
        }

        if (endDateTime > DateTime.UtcNow.AddYears(1)) // Max 1 year in the future
        {
            throw new BusinessRulesException("End date cannot be more than 1 year in the future");
        }

        
        if (startDateTime.HasValue && endDateTime <= startDateTime.Value)
        {
            throw new BusinessRulesException("End date must be after start date");
        }
        
        if (startDateTime.HasValue && (endDateTime - startDateTime.Value).TotalDays > 30) // Max 30 days rental period
        {
            throw new BusinessRulesException("Rental period cannot exceed 30 days");
        }

        EndDateTime = endDateTime;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        var that = (RentalEndDate)obj;
        return EndDateTime == that.EndDateTime;
    }

    public override int GetHashCode() => EndDateTime.GetHashCode();
    public override string ToString() => EndDateTime.ToString("o");
}