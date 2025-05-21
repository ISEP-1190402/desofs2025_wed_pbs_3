using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.Role;

public class Role : Entity<RoleId>, IAggregateRoot
{
    protected Role() { }

    public Role(string name, string description)
    {
        Id = new RoleId(Guid.NewGuid());
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
}