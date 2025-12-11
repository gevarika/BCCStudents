using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;
using MySql.Data.MySqlClient;
using BCCStudents.Infrastructure.Data;

namespace BCCStudents.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public void RegisterUser(RegisterUserModel user)
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Users (Username, FullName, Email, Password, Role, CreatedAt, LastLogin) 
                         VALUES (@Username, @FullName, @Email, @Password, @Role, @CreatedAt, @LastLogin)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.UserName);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@LastLogin", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // მომხმარებლის არსებობის შემოწმება
        public bool IsUserRegistered()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Users";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return userCount > 0;
                }
            }
        }
        public UserModel GetUserByUsername(string username)
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Users WHERE Username = @Username";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = username,
                                Password = reader["Password"].ToString(),
                                FullName = reader["FullName"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Role = reader["Role"]?.ToString(),
                                LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : DateTime.MinValue
                            };
                        }
                    }
                }
            }

            return null;
        }
        public void UpdateLastLogin(int userId, DateTime lastLogin)
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();
                string query = "UPDATE Users SET LastLogin = @LastLogin WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LastLogin", lastLogin);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public UserModel GetUserById(int id)
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();

                var query = "SELECT * FROM Users WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"]?.ToString(),
                                Role = reader["Role"]?.ToString(),
                                //CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null,
                                //LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : (DateTime?)null
                            };
                        }
                    }
                }
            }

            return null; // თუ არ მოიძებნა
        }
        public string GetFullName(int userId)
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();
                var query = "SELECT FullName FROM Users WHERE Id = @UserId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string fullname = reader["FullName"].ToString();
                            return fullname;
                        }
                    }
                }

                return "უცნობი მომხმარებელი";
            }
        }
        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();

                var query = "SELECT * FROM Users";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add( new UserModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"]?.ToString(),
                                Role = reader["Role"]?.ToString(),
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null,
                                LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime( reader["LastLogin"]) : (DateTime?)null
                            });
                        }
                    }
                }
            }

            return users;
        }
    }
}


