using System.Text.Json.Serialization;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class CreatedRentalDTO
{
    [JsonConstructor]
    public CreatedRentalDTO(string startDate, string endDate, string reservedBookId, string userEmail)
    {
        StartDate = startDate;
        EndDate = endDate;
        ReservedBookId = reservedBookId;
        UserEmail = userEmail;
    }

    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string ReservedBookId { get; set; }
    public string UserEmail { get; set; }
}