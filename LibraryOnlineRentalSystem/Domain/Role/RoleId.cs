using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Role;
public class RoleId: EntityId
{
    public RoleId(Guid value) : base(value)
    {
    }

    public RoleId(string value) : base(value)
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