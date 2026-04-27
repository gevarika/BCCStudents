using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IUserService
    {
        bool Login(string username, string password, out int userId);
        int RegisterUser(string username, string fullName, string email, string password, string role, string permissionsJson = null);
        bool IsUsernameExists(string username);
        bool IsUserRegistered();
        bool IsCurrentUserAdmin();
        void UpdateLastLogin(int userId, DateTime lastLogin);
        UserModel GetUserByUsername(string username);
        string GetFullName(int userId);
        UserModel GetUserById(int userId);
        List<UserModel> GetAllUsers();

        // Permissions Management
        void UpdateUser(int userId, string fullName, string email, string role, string permissionsJson);
        void UpdateUserPermissions(int userId, string permissionsJson);
        void UpdateUserPassword(int userId, string newPassword);
        string HashPassword(string password);
    }
}


