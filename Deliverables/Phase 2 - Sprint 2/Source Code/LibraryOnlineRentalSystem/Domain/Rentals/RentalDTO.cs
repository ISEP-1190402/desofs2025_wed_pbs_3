using Newtonsoft.Json;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalDTO
{
    [JsonConstructor]
    public RentalDTO(string idRentalValue, string startDate, string endDate, string reservedBookId, string rentalStatus
        , string userEmail)
    {
        IdRentalValue = idRentalValue;
        StartDate = startDate;
        EndDate = endDate;
        ReservedBookId = reservedBookId;
        RentalStatus = rentalStatus;
        UserEmail = userEmail;
    }

    public string IdRentalValue { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string ReservedBookId { get; set; }
    public string RentalStatus { get; set; }
    public string UserEmail { get; set; }
}