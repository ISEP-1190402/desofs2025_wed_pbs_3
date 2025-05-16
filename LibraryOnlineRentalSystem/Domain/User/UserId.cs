using System.Text.Json.Serialization;
using LibraryOnlineRentalSystem.Domain.Shared;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserId : EntityId
{
    [JsonConstructor]
    public UserId(Guid value) : base(value)
    {

        if (value == Guid.Empty) throw new BusinessRulesException("Guid cannot be null or empty.");
    }
    [JsonConstructor]

    public UserId(String value) : base(value)
    {
        if (String.IsNullOrEmpty(value)) throw new BusinessRulesException("Guid cannot be null or empty.");
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

