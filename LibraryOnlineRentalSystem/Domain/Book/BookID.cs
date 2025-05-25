using System.Text.Json.Serialization;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.IdentityModel.Tokens;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookID : EntityId
{
    
    [JsonConstructor]
    public BookID(string value) : base(value)
    {
        if (string.IsNullOrEmpty(value)) throw new BusinessRulesException("ID cannot be null or empty.");
        bookID = value;
    }

    private string bookID { get; }
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

        var that = (BookID) obj;

        return this.bookID.Equals(that.bookID);
    }

    public override string ToString()
    {
        return $"{bookID}";
    }
    
    
    
    
    
}