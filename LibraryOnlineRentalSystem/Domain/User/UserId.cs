using System.Text.Json.Serialization;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserId : EntityId
{
    [JsonConstructor]
    public UserId(Guid value) : base(value)
    {
        if (value == Guid.Empty) throw new BusinessRulesException("Guid cannot be null or empty.");
    }

    [JsonConstructor]
    public UserId(string value) : base(value)
    {
        if (string.IsNullOrEmpty(value)) throw new BusinessRulesException("Guid cannot be null or empty.");
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