using System.Text.Json.Serialization;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookID : EntityId
{
    [JsonConstructor]
    public BookID(Guid value) : base(value)
    {
    }

    public BookID(string value) : base(value)
    {
    }

    override
        protected object createFromString(string text)
    {
        return new Guid(text);
    }

    override
        public string AsString()
    {
        var obj = (Guid)ObjValue;
        return obj.ToString();
    }

    public Guid AsGuid()
    {
        return (Guid)ObjValue;
    }
}