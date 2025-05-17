using LibraryOnlineRentalSystem.Domain.Common;
using LibraryOnlineRentalSystem.Domain.User;


namespace LibraryOnlineRentalSystem.Domain.User
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        //private readonly IRoleRepository _roleRepository;
        private readonly IWorkUnity _workUnit;
        private readonly IKeycloakService _keycloakService;
        private readonly IAuditLogger _auditLogger;

        public UserService(
            IUserRepository userRepository,
            //IRoleRepository roleRepository,
            IWorkUnity workUnit,
            IKeycloakService keycloakService,
            IAuditLogger auditLogger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            //_roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _workUnit = workUnit ?? throw new ArgumentNullException(nameof(workUnit));
            _keycloakService = keycloakService ?? throw new ArgumentNullException(nameof(keycloakService));
            _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        }

        public async Task<User> RegisterUserAsync(
            string name,
            string email,
            string roleId,
            string userName,
            string phoneNumber,
            string nif,
            string biography,
            string password)
        {
            
            ValidateInput(name, email, userName, phoneNumber, nif, password);

      
            // No método RegisterUserAsync:
            if (await _userRepository.GetByEmailAsync(email).ConfigureAwait(false) != null)
                throw new BusinessRulesException("Email being used");

            if (await _userRepository.GetByUsernameAsync(userName).ConfigureAwait(false) != null)
                throw new BusinessRulesException("Username being used");
            
            //if (!await _roleRepository.ExistsAsync(new RoleId(roleId)))
            //    throw new BusinessRulesException("Role inválida");
            
            var user = new User(name, email, roleId, userName, phoneNumber, nif, biography);
            await _userRepository.AddAsync(user);
            await _workUnit.CommitAsync();

         
            //var keycloakUserId = await _keycloakService.RegisterUserAsync(
            //    username: userName,
            //    email: email,
                //password: password,
            //    enabled: true,
                //emailVerified: false, 
                //requiredActions: new[] { "CONFIGURE_TOTP", "VERIFY_EMAIL" } 
           // );

          
            //user.SetKeycloakId(keycloakUserId);
            await _workUnit.CommitAsync();

            // 6. Auditoria
            await _auditLogger.LogAsync($"Novo usuário registrado: {userName}", "Registration");

            return user;
        }


        public async Task UpdateUserEmailAsync(string userId, string newEmail)
        {
            ValidateEmail(newEmail);

            var user = await GetUserByIdAsync(userId);
            if (await _userRepository.GetByEmailAsync(newEmail).ConfigureAwait(false) != null)
                throw new BusinessRulesException("Email being used");
            
            user.ChangeEmail(newEmail);
            await _workUnit.CommitAsync();
            
       
            await _auditLogger.LogAsync($"User {userId} has updated email to {newEmail}", "EmailUpdate");
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(new UserId(userId));
            if (user == null)
                throw new BusinessRulesException("User not found");

            
            return user;
        }
        
        private void ValidateInput(string name, string email, string userName, string phoneNumber, string nif, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
                throw new BusinessRulesException("Password must contain 12+ characters");

            ValidateEmail(email);
            ValidateUsername(userName);
            ValidatePhoneNumber(phoneNumber);
            ValidateNIF(nif);
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new BusinessRulesException("Invalid email");

     
            if (email.Contains("\n") || email.Contains("\r"))
                throw new BusinessRulesException("Email contains invalid characters");
        }

        private void ValidateUsername(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new BusinessRulesException("Username cannot be empty");

   
            if (userName.Contains("'") || userName.Contains(";") || userName.Contains("--"))
                throw new BusinessRulesException("Username contains invalid characters");
        }

        private void ValidatePhoneNumber(string phoneNumber)
        {
            // Validação básica (ajuste conforme necessidades)
            if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 8)
                throw new BusinessRulesException("Invalid phone number");
        }

        private void ValidateNIF(string nif)
        {
            // Validação básica para NIF português
            if (string.IsNullOrWhiteSpace(nif) || nif.Length != 9 || !nif.All(char.IsDigit))
                throw new BusinessRulesException("NIF must be a 9-digit number");
        }
    }

    public interface IUserService
    {
        Task<User> RegisterUserAsync(
            string name,
            string email,
            string roleId,
            string userName,
            string phoneNumber,
            string nif,
            string biography);

        Task UpdateUserEmailAsync(string userId, string newEmail);
        Task<User> GetUserByIdAsync(string userId);
    }

    public interface IKeycloakService
    {
        Task<string> RegisterUserAsync(string username, string email, bool enabled);
        Task UpdateUserEmailAsync(string keycloakUserId, string newEmail);
    }

    public interface IAuditLogger
    {
        Task LogAsync(string message, string category);
    }
}