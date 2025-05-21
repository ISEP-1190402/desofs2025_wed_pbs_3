using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.Role;
using static LibraryOnlineRentalSystem.Controllers.UserController;
using LibraryOnlineRentalSystem.Repository.RoleRepository;

namespace LibraryOnlineRentalSystem.Domain.User;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWorkUnity _workUnit;
    private readonly IAuditLogger _auditLogger;
    private readonly IRoleRepository _roleRepository;
    private readonly PasswordService _passwordService;
    private const string DEFAULT_USER_ROLE_NAME = "User";

    public UserService(
        IUserRepository userRepository,
        IWorkUnity workUnit,
        IAuditLogger auditLogger,
        IRoleRepository roleRepository,
        PasswordService passwordService)
    {
        _userRepository = userRepository;
        _workUnit = workUnit;
        _auditLogger = auditLogger;
        _roleRepository = roleRepository;
        _passwordService = passwordService;
    }
    
    public async Task CreateUserAsync(NewUserDTO req)
    {
        if (await _userRepository.GetByEmailAsync(req.Email) != null)
            throw new BusinessRulesException("Email already in use");

        if (await _userRepository.GetByUsernameAsync(req.UserName) != null)
            throw new BusinessRulesException("Username already in use");

        var userRole = await _roleRepository.GetByNameAsync(DEFAULT_USER_ROLE_NAME);
        if (userRole == null)
            throw new BusinessRulesException("Default user role not found");

        try
        {
            // Hash the password
            var hashedPassword = _passwordService.HashPassword(req.Password);

            // Create user in our database with hashed password
            var user = new User(
                req.Name,
                req.Email,
                userRole.Id.AsString(),
                req.UserName,
                req.PhoneNumber,
                req.Nif,
                req.Biography,
                hashedPassword
            );

            await _userRepository.AddAsync(user);
            await _workUnit.CommitAsync();
            await _auditLogger.LogAsync($"User {req.UserName} created successfully", "UserCreation");
        }
        catch (Exception ex)
        {
            throw new BusinessRulesException("Failed to create user: " + ex.Message);
        }
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
