namespace LibraryOnlineRentalSystem.Domain.Rentals;

public enum Status
{
    Pending = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4
}

public static class BookingStatusHelper
{
    public static Status? GetStatusById(int id)
    {
        if (Enum.IsDefined(typeof(Status), id))
            return (Status)id;
        return null;
    }

    public static int GetIdByStatus(Status status)
    {
        return (int)status;
    }
}