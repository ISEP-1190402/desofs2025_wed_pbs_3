using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Role;

namespace LibraryOnlineRentalSystem.Domain.User;

public class User : Entity<UserId>, IAggregateRoot
{
    protected User() { }
    public User(string id, string name, string email, string roleId, string userName, string phoneNumber, string nif, string biography)
    {
        Id = new UserId(id);
        Name = new Name(name);
        Email = new Email(email);
        RoleId = new RoleId(roleId);
        UserName = new UserName(userName);
        PhoneNumber = new PhoneNumber(phoneNumber);
        Nif = new NIF(nif);
        Biography = new Biography(biography);
    }
    
    public User(string name, string email, string roleId, string userName, string phoneNumber, string nif, string biography)
    {
        Id = new UserId(Guid.NewGuid());
        Name = new Name(name);
        Email = new Email(email);
        RoleId = new RoleId(roleId);
        UserName = new UserName(userName);
        PhoneNumber = new PhoneNumber(phoneNumber);
        Nif = new NIF(nif);
        Biography = new Biography(biography);
    }

    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public RoleId RoleId { get; private set; }
    public UserName UserName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public NIF Nif { get; private set; }
    public Biography Biography { get; private set; }
    
    public void ChangeBiography(string biography)
    {
        Biography = new Biography(biography);
    }

    public void ChangeEmail(string email)
    {
        Email = new Email(email);
    }

    public void ChangePhoneNumber(string phoneNumber)
    {
        PhoneNumber = new PhoneNumber(phoneNumber);
    }

    public void ChangeName(string name)
    {
        Name = new Name(name);
    }

    public void ChangeRoleId(string roleId)
    {
        RoleId = new RoleId(roleId);
    }
}
