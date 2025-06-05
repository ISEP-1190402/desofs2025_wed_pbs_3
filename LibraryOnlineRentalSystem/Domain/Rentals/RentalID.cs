using System.Text.Json.Serialization;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.IdentityModel.Tokens;

namespace LibraryOnlineRentalSystem.Domain.Rentals;

public class RentalID : EntityId
{
    
    [JsonConstructor]
    public RentalID(string value) : base(value)
    {
        if (string.IsNullOrEmpty(value)) throw new BusinessRulesException("ID cannot be null or empty.");
        rentalID = value;
    }

    private string rentalID { get; }
    override
        protected object createFromString(string text)
    {
        return text;
    }

    override
        public string AsString()
    {
        var obj = ObjValue;
        return obj.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (RentalID) obj;

        return this.rentalID.Equals(that.rentalID);
    }

    public override string ToString()
    {
        return $"{rentalID}";
    }
    
}