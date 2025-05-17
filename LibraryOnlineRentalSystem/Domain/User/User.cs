using LibraryOnlineRentalSystem.Domain.Role;
using LibraryOnlineRentalSystem.Domain.Shared;

namespace LibraryOnlineRentalSystem.Domain.User;

public class User: Entity<UserId>, IAggregateRoot {
    
    public User(Name name, Email email, RoleId roleId, UserName userName, PhoneNumber  phoneNumber, NIF nif, Biography biography)
    {
        if (roleId == null)
            throw new BusinessRulesException("Every user requires a role.");

        this.Id = new UserId(Guid.NewGuid());
        this.Name = (Name)name.Clone();
        this.UserName = (UserName)userName.Clone();
        this.Email = (Email)email.Clone();
        this.PhoneNumber = (PhoneNumber)phoneNumber.Clone();
        this.RoleId = roleId;
        this.Nif = (NIF)nif.Clone();
        this.Biography = (Biography)biography.Clone();
        
    }
    public User(string id, Name name, Email email, RoleId roleId, UserName userName, PhoneNumber  phoneNumber, NIF nif, Biography biography)
    {
        if (roleId == null)
            throw new BusinessRulesException("Every user requires a role.");

        this.Id = new UserId(Guid.Parse(id));
        this.Name = (Name)name.Clone();
        this.UserName = (UserName)userName.Clone();
        this.Email = (Email)email.Clone();
        this.PhoneNumber = (PhoneNumber)phoneNumber.Clone();
        this.RoleId = roleId;
        this.Nif = (NIF)nif.Clone();
        this.Biography = (Biography)biography.Clone();
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
        this.Biography = (Biography)biography.Clone();
    }

    public void ChangeEmail(Email email)
    {
        this.Email = (Email)email.Clone();
    }

    public void ChangeRoleId(RoleId roleId)
    {
        if (roleId == null)
            throw new BusinessRulesException("Every user requires a role.");
        this.RoleId = roleId;
    }

}
