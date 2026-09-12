using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public UserRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public int RegisterUser(RegisterUserModel user)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                var now = user.CreatedAt == default ? DateTime.Now : user.CreatedAt;
                string query = @"INSERT INTO Users (Username, FullName, Email, Password, Role, Permissions, CreatedAt, LastLogin, UpdatedAt) 
                         VALUES (@Username, @FullName, @Email, @Password, @Role, @Permissions, @CreatedAt, @LastLogin, @UpdatedAt);
                         SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.UserName);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@LastLogin", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@Permissions", (object)user.Permissions ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", now);
                    cmd.Parameters.AddWithValue("@UpdatedAt", now);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        // მომხმარებლის არსებობის შემოწმება
        public bool IsUserRegistered()
        {
            using (var connection = _connectionProvider.GetLocalConnection())
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
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                return ReadUserByUsername(conn, username);
            }
        }

        private static UserModel ReadUserByUsername(MySqlConnection conn, string username)
        {
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
                            Permissions = reader["Permissions"]?.ToString(),
                            CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null,
                            LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : (DateTime?)null,
                            UpdatedAt = ReadUpdatedAt(reader)
                        };
                    }
                }
            }

            return null;
        }
        public void UpdateLastLogin(int userId, DateTime lastLogin)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "UPDATE Users SET LastLogin = @LastLogin, UpdatedAt = @UpdatedAt WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LastLogin", lastLogin);
                    cmd.Parameters.AddWithValue("@UpdatedAt", lastLogin);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public UserModel GetUserById(int id)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
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
                                Password = reader["Password"]?.ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"]?.ToString(),
                                Role = reader["Role"]?.ToString(),
                                Permissions = reader["Permissions"]?.ToString(),
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null,
                                LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : (DateTime?)null,
                                UpdatedAt = ReadUpdatedAt(reader)
                            };
                        }
                    }
                }
            }

            return null; // თუ არ მოიძებნა
        }
        public string GetFullName(int userId)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
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
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();

                var query = "SELECT * FROM Users";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = reader["Username"].ToString(),
                                Password = reader["Password"]?.ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"]?.ToString(),
                                Role = reader["Role"]?.ToString(),
                                Permissions = reader["Permissions"]?.ToString(),
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null,
                                LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : (DateTime?)null,
                                UpdatedAt = ReadUpdatedAt(reader)
                            });
                        }
                    }
                }
            }

            return users;
        }

        // Permissions Management
        public void UpdatePermissions(int userId, string permissionsJson)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "UPDATE Users SET Permissions = @Permissions, UpdatedAt = @UpdatedAt WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Permissions", (object)permissionsJson ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePassword(int userId, string passwordHash)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "UPDATE Users SET Password = @Password, UpdatedAt = @UpdatedAt WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", passwordHash);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUser(int userId, string fullName, string email, string role, string permissionsJson)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = @"UPDATE Users 
                                SET FullName = @FullName, 
                                    Email = @Email, 
                                    Role = @Role, 
                                    Permissions = @Permissions,
                                    UpdatedAt = @UpdatedAt
                                WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@Permissions", (object)permissionsJson ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(int userId)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "DELETE FROM Users WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetAdminCount()
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE LOWER(Role) = 'administrator'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private static DateTime ReadUpdatedAt(MySqlDataReader reader)
        {
            if (reader["UpdatedAt"] != DBNull.Value)
            {
                return Convert.ToDateTime(reader["UpdatedAt"]);
            }

            if (reader["CreatedAt"] != DBNull.Value)
            {
                return Convert.ToDateTime(reader["CreatedAt"]);
            }

            return DateTime.UtcNow;
        }
    }
}


