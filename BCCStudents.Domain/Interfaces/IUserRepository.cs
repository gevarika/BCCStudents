using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IUserRepository
    {
        void RegisterUser(RegisterUserModel user);
        bool IsUserRegistered();
        UserModel GetUserByUsername(string username);
        void UpdateLastLogin(int userId, DateTime lastLogin);
        UserModel GetUserById(int id);
        string GetFullName(int userId);
        List<UserModel> GetAllUsers();
    }
}


