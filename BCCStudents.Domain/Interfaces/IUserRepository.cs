using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IUserRepository
    {
        int RegisterUser(RegisterUserModel user);
        bool IsUserRegistered();
        UserModel GetUserByUsername(string username);
        void UpdateLastLogin(int userId, DateTime lastLogin);
        UserModel GetUserById(int id);
        string GetFullName(int userId);
        List<UserModel> GetAllUsers();

        // Permissions Management
        void UpdatePermissions(int userId, string permissionsJson);
        void UpdatePassword(int userId, string passwordHash);
        void UpdateUser(int userId, string fullName, string email, string role, string permissionsJson);
    }
}


