using BCCStudents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Interfaces
{
    public interface IUserService
    {
        bool Login(string username, string password, out int userId);
        void RegisterUser(string username, string fullName, string email, string password, string role);
        bool IsUsernameExists(string username);
        bool IsUserRegistered();
        bool IsCurrentUserAdmin();
        void UpdateLastLogin(int userId, DateTime lastLogin);
        UserModel GetUserByUsername(string username);
        string GetFullName(int userId);
        UserModel GetUserById(int userId);
        List<UserModel> GetAllUsers();
    }
}


