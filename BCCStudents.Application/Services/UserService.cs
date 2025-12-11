using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        //UserSession UserSession = new UserSession();

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public void RegisterUser(string username, string fullName, string email, string password, string role)
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
                CreatedAt = DateTime.Now
            };

            _userRepository.RegisterUser(user);
            
        }
        public bool IsCurrentUserAdmin()
        {
            var user = _userRepository.GetUserById(UserSession.Id);
            return user != null && user.Role?.ToLower() == "administrator";
        }
        private string HashPassword(string password)
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
    }
}



