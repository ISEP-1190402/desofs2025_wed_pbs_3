using System.Text.Json.Serialization;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class UpdateRentalStatusDTO
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
}
