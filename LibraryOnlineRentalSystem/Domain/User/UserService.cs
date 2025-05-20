using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Role;
using static LibraryOnlineRentalSystem.Controllers.UserController;



namespace LibraryOnlineRentalSystem.Domain.User;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWorkUnity _workUnit;
    private readonly IAuditLogger _auditLogger;

    public UserService(
        IUserRepository userRepository,
        IWorkUnity workUnit,
        IAuditLogger auditLogger)
    {
        _userRepository = userRepository;
        _workUnit = workUnit;
        _auditLogger = auditLogger;
    }
    
    public async Task CreateUserAsync(NewUserDTO req)
    {
        if (await _userRepository.GetByEmailAsync(req.Email) != null)
            throw new BusinessRulesException("Email already in use");

        if (await _userRepository.GetByUsernameAsync(req.UserName) != null)
            throw new BusinessRulesException("Username already in use");

        var user = new User(
            req.Name,
            req.Email,
            req.RoleId,
            req.UserName,
            req.PhoneNumber,
            req.Nif,
            req.Biography
        );

        await _userRepository.AddAsync(user);
        await _workUnit.CommitAsync();
        await _auditLogger.LogAsync($"User {req.UserName} created manually", "UserCreation");
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(id));
        if (user == null) return null;

        return new UserDTO(
            user.Id.AsGuid(),
            user.Name.FullName,
            user.Email.EmailAddress,
            user.RoleId.AsGuid(),
            user.Nif.TaxID,
            user.UserName.Tag,
            user.Biography.Description,
            user.PhoneNumber.Number
        );
    }

    public async Task<List<UserDTO>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(user => new UserDTO(
            user.Id.AsGuid(),
            user.Name.FullName,
            user.Email.EmailAddress,
            user.RoleId.AsGuid(),
            user.Nif.TaxID,
            user.UserName.Tag,
            user.Biography.Description,
            user.PhoneNumber.Number
        )).ToList();
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(id));
        if (user == null)
            throw new BusinessRulesException("User not found");

        if (request.Biography != null)
            user.ChangeBiography(request.Biography);

        if (request.PhoneNumber != null)
            user.ChangePhoneNumber(request.PhoneNumber);

        if (request.RoleId != null)
            user.ChangeRoleId(request.RoleId);

        if (request.Name != null)
            user.ChangeName(request.Name);

        if (request.Email != null)
            user.ChangeEmail(request.Email);

        await _workUnit.CommitAsync();
        await _auditLogger.LogAsync($"User {id} updated profile.", "ProfileUpdate");
    }


    //public async Task DeleteUserAsync(Guid id)
    //{
     //   var user = await _userRepository.GetByIdAsync(new UserId(id));
       // if (user == null) return;

        //_userRepository.Delete(user);
       // await _workUnit.CommitAsync();
      //  await _auditLogger.LogAsync($"User {id} deleted.", "UserDeletion");
    //}
}
