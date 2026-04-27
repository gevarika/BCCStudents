using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BCCStudents.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;
        //UserSession UserSession = new UserSession();

        public UserService(IUserRepository userRepository, IUpStreamChangeTracker upStreamChangeTracker = null)
        {
            _userRepository = userRepository;
            _upStreamChangeTracker = upStreamChangeTracker; // Optional - თუ null-ია, sync არ მოხდება
        }
        public UserModel GetUserByUsername(string username)
        { return _userRepository.GetUserByUsername(username); }
        public bool IsUserRegistered()
        {
            return _userRepository.IsUserRegistered();
        }
        public bool Login(string username, string password, out int userId)
        {
            userId = -1;
            var user = _userRepository.GetUserByUsername(username);

            if (user == null)
                return false;

            if (VerifyPassword(password, user.Password))
            {
                userId = user.Id;

                // სესიის გაწერა პირდაპირ აქ
                UserSession.Id = user.Id;
                UserSession.UserName = user.UserName;
                UserSession.FullName = user.FullName;
                UserSession.Email = user.Email;
                UserSession.Role = user.Role;
                UserSession.LastLogin = user.LastLogin;

                _userRepository.UpdateLastLogin(userId, DateTime.Now);

                return true;
            }

            return false;
        }
        public void UpdateLastLogin(int userId, DateTime lastLogin)
        {
            _userRepository.UpdateLastLogin(userId, lastLogin);

        }
        private bool VerifyPassword(string password, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString() == storedHash;
            }
        }
        public bool IsUsernameExists(string username)
        {
            return _userRepository.GetUserByUsername(username) != null;
        }

        public int RegisterUser(string username, string fullName, string email, string password, string role, string permissionsJson = null)
        {
            // შეამოწმე არის თუ არა username უკვე არსებული
            if (IsUsernameExists(username))
            {
                throw new InvalidOperationException($"მომხმარებელი '{username}' უკვე არსებობს!");
            }

            string passwordHash = HashPassword(password);

            var user = new RegisterUserModel
            {
                UserName = username,
                FullName = fullName,
                Email = email,
                Password = passwordHash,
                Role = role,
                Permissions = permissionsJson,
                CreatedAt = DateTime.Now
            };

            int userId = _userRepository.RegisterUser(user);

            // Sync to server
            if (_upStreamChangeTracker != null && userId > 0)
            {
                var createdUser = _userRepository.GetUserById(userId);
                if (createdUser != null)
                {
                    _upStreamChangeTracker.TrackUserChange(userId, SyncOperationType.Insert, createdUser);
                }
            }

            return userId;
        }
        public bool IsCurrentUserAdmin()
        {
            var user = _userRepository.GetUserById(UserSession.Id);
            return user != null && user.Role?.ToLower() == "administrator";
        }
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
        public UserModel GetUserById(int userId)
        {
            return _userRepository.GetUserById(userId);
        }
        public string GetFullName(int userId)
        { return _userRepository.GetFullName(userId); }
        public List<UserModel> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        // Permissions Management
        public void UpdateUser(int userId, string fullName, string email, string role, string permissionsJson)
        {
            _userRepository.UpdateUser(userId, fullName, email, role, permissionsJson);

            // Sync to server
            if (_upStreamChangeTracker != null)
            {
                var updatedUser = _userRepository.GetUserById(userId);
                if (updatedUser != null)
                {
                    _upStreamChangeTracker.TrackUserChange(userId, SyncOperationType.Update, updatedUser);
                }
            }
        }

        public void UpdateUserPermissions(int userId, string permissionsJson)
        {
            _userRepository.UpdatePermissions(userId, permissionsJson);

            // Sync to server
            if (_upStreamChangeTracker != null)
            {
                var updatedUser = _userRepository.GetUserById(userId);
                if (updatedUser != null)
                {
                    _upStreamChangeTracker.TrackUserChange(userId, SyncOperationType.Update, updatedUser);
                }
            }
        }

        public void UpdateUserPassword(int userId, string newPassword)
        {
            string passwordHash = HashPassword(newPassword);
            _userRepository.UpdatePassword(userId, passwordHash);

            // Sync to server (Password changes are also synced)
            if (_upStreamChangeTracker != null)
            {
                var updatedUser = _userRepository.GetUserById(userId);
                if (updatedUser != null)
                {
                    _upStreamChangeTracker.TrackUserChange(userId, SyncOperationType.Update, updatedUser);
                }
            }
        }
    }
}



