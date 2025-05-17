using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Role;

namespace LibraryOnlineRentalSystem.Domain.User;

public class User : Entity<UserId>, IAggregateRoot
{
    public User(Name name, Email email, RoleId roleId, UserName userName, PhoneNumber phoneNumber, NIF nif,
        Biography biography)
    {
        if (roleId == null)
            throw new BusinessRulesException("Every user requires a role.");

        Id = new UserId(Guid.NewGuid());
        Name = (Name)name.Clone();
        UserName = (UserName)userName.Clone();
        Email = (Email)email.Clone();
        PhoneNumber = (PhoneNumber)phoneNumber.Clone();
        RoleId = roleId;
        Nif = (NIF)nif.Clone();
        Biography = (Biography)biography.Clone();
    }

    public User(string id, Name name, Email email, RoleId roleId, UserName userName, PhoneNumber phoneNumber, NIF nif,
        Biography biography)
    {
        if (roleId == null)
            throw new BusinessRulesException("Every user requires a role.");

        Id = new UserId(Guid.Parse(id));
        Name = (Name)name.Clone();
        UserName = (UserName)userName.Clone();
        Email = (Email)email.Clone();
        PhoneNumber = (PhoneNumber)phoneNumber.Clone();
        RoleId = roleId;
        Nif = (NIF)nif.Clone();
        Biography = (Biography)biography.Clone();
    }

    public Name Name { get; private set; }

    public UserName UserName { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public NIF Nif { get; private set; }

    public Biography Biography { get; private set; }

    public Email Email { get; private set; }

    public RoleId RoleId { get; private set; }


    public void ChangeBiography(Biography biography)
    {
        Biography = (Biography)biography.Clone();
    }

    public void ChangeEmail(Email email)
    {
        Email = (Email)email.Clone();
    }

    public void ChangeRoleId(RoleId roleId)
    {
        if (roleId == null)
            throw new BusinessRulesException("Every user requires a role.");
        RoleId = roleId;
    }
}