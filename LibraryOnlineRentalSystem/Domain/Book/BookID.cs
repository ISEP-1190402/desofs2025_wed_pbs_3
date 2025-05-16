using System.Text.Json.Serialization;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookID : EntityId
{
    [JsonConstructor]
    public BookID(Guid value) : base(value)
    {
    }

    public BookID(String value) : base(value)
    {
    }

    override
        protected Object createFromString(String text)
    {
        return new Guid(text);
    }

    override
        public String AsString()
    {
        Guid obj = (Guid)base.ObjValue;
        return obj.ToString();
    }

    public Guid AsGuid()
    {
        return (Guid)base.ObjValue;
    }
}