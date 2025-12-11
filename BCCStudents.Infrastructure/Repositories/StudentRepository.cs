using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;

namespace BCCStudents.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public StudentRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        #region ==================== INSERT - მოსწავლის ჩასმა ====================

        /// <summary>
        /// ახალი მოსწავლის ჩასმა (სრული ობიექტით)
        /// </summary>
        /// <param name="student">მოსწავლის ობიექტი</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი მოსწავლის ID</returns>
        public int InsertStudent(Student student, MySqlConnection connection = null, MySqlTransaction transaction = null)
        {
            bool useExternalConnection = connection != null;
            var conn = connection ?? _dbHelper.GetLocalConnection();

            try
            {
                if (!useExternalConnection)
                    conn.Open();

                var query = @"INSERT INTO Students 
                              (FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address, 
                               RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath, 
                               user_id, Balance, Info, UpdatedAt, IsDeleted) 
                              VALUES 
                              (@FirstName, @LastName, @Age, @ParentName, @PhoneNumber, @Id_Numb, @Address,
                               @RegistrationDate, @StudentCode, @Status, @IdCardPath, @AdditionalDocsPath,
                               @user_id, @Balance, @Info, @UpdatedAt, @IsDeleted);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@FirstName", student.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", student.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Age", student.Age);
                    cmd.Parameters.AddWithValue("@ParentName", student.ParentName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id_Numb", student.Id_Numb);
                    cmd.Parameters.AddWithValue("@Address", student.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegistrationDate", student.RegistrationDate);
                    cmd.Parameters.AddWithValue("@StudentCode", student.StudentCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", student.Status);
                    cmd.Parameters.AddWithValue("@IdCardPath", student.IdCardPath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AdditionalDocsPath", student.AdditionalDocsPath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@user_id", student.User_Id);
                    cmd.Parameters.AddWithValue("@Balance", student.Balance);
                    cmd.Parameters.AddWithValue("@Info", student.Info ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@IsDeleted", false);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            finally
            {
                if (!useExternalConnection && conn != null)
                    conn.Dispose();
            }
        }

        /// <summary>
        /// ახალი მოსწავლის ჩასმა (მინიმალური მონაცემებით)
        /// </summary>
        /// <param name="firstName">სახელი</param>
        /// <param name="lastName">გვარი</param>
        /// <param name="phone">ტელეფონი</param>
        /// <returns>ახალი მოსწავლის ID</returns>
        public int InsertStudentBasic(string firstName, string lastName, string phone)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO Students 
                              (FirstName, LastName, PhoneNumber, Age, ParentName, Id_Numb, Address,
                               RegistrationDate, Balance, UpdatedAt, IsDeleted) 
                              VALUES 
                              (@FirstName, @LastName, @PhoneNumber, 0, '', 0, '',
                               @RegistrationDate, 0, @UpdatedAt, 0);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", lastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// ახალი მოსწავლის ჩასმა (სახელი, გვარი, ტელეფონი, პირადი ნომერი და მშობლის სახელი)
        /// </summary>
        public int InsertStudentWithDetails(string firstName, string lastName, string phone, long personalId, string parentName)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO Students 
                              (FirstName, LastName, PhoneNumber, Id_Numb, ParentName, Age, Address,
                               RegistrationDate, Balance, UpdatedAt, IsDeleted) 
                              VALUES 
                              (@FirstName, @LastName, @PhoneNumber, @Id_Numb, @ParentName, 0, '',
                               @RegistrationDate, 0, @UpdatedAt, 0);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", lastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id_Numb", personalId);
                    cmd.Parameters.AddWithValue("@ParentName", parentName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion

        #region ==================== SELECT - მოსწავლის წაკითხვა ====================

        /// <summary>
        /// მოსწავლის მიღება ID-ით
        /// </summary>
        public Student GetStudentById(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapStudentFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// მოსწავლის მიღება კოდით
        /// </summary>
        public Student GetStudentByCode(string studentCode)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE StudentCode = @StudentCode";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentCode", studentCode);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapStudentFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// მოსწავლის მიღება პირადი ნომრით
        /// </summary>
        public Student GetStudentByPersonalId(long personalId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE Id_Numb = @PersonalId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PersonalId", personalId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapStudentFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// მოსწავლის მიღება სახელით და გვარით
        /// </summary>
        public Student GetStudentByFullName(string firstName, string lastName)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE FirstName = @FirstName AND LastName = @LastName";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapStudentFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ყველა მოსწავლის მიღება
        /// </summary>
        public List<Student> GetAllStudents()
        {
            var students = new List<Student>();

            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE IsDeleted = 0 ORDER BY FirstName, LastName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(MapStudentFromReader(reader));
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// მხოლოდ აქტიური მოსწავლეების მიღება (Status = 'Active' ან მსგავსი)
        /// </summary>
        public List<Student> GetAllActiveStudents()
        {
            var students = new List<Student>();

            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE IsDeleted = 0 AND Status = 1
                              ORDER BY FirstName, LastName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(MapStudentFromReader(reader));
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// არააქტიური მოსწავლეების მიღება
        /// </summary>
        public List<Student> GetAllInactiveStudents()
        {
            var students = new List<Student>();

            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE IsDeleted = 0 AND Status = 0
                              ORDER BY FirstName, LastName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(MapStudentFromReader(reader));
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// წაშლილი მოსწავლეების მიღება (IsDeleted = 1)
        /// </summary>
        public List<Student> GetAllDeletedStudents()
        {
            var students = new List<Student>();

            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                              RegistrationDate, StudentCode, Status, IdCardPath, AdditionalDocsPath,
                              user_id, Balance, Info, UpdatedAt, IsDeleted
                              FROM Students WHERE IsDeleted = 1
                              ORDER BY FirstName, LastName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(MapStudentFromReader(reader));
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// მოსწავლის ID-ის მიღება კოდით
        /// </summary>
        public int? GetStudentIdByCode(string studentCode)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Id FROM Students WHERE StudentCode = @StudentCode";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentCode", studentCode);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : (int?)null;
                }
            }
        }

        /// <summary>
        /// მოსწავლის სრული სახელის მიღება ID-ით
        /// </summary>
        public string GetStudentFullNameById(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT CONCAT(FirstName, ' ', LastName) FROM Students WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        /// <summary>
        /// მოსწავლის კოდის მიღება ID-ით
        /// </summary>
        public string GetStudentCodeById(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT StudentCode FROM Students WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        /// <summary>
        /// მოსწავლის ბალანსის მიღება ID-ით
        /// </summary>
        public decimal GetStudentBalanceById(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Balance FROM Students WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლეების მიღება DataTable-ად (ComboBox-ებისთვის)
        /// </summary>
        public DataTable GetStudentsAsDataTable()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Id, FirstName, LastName, StudentCode FROM Students WHERE IsDeleted = 0 ORDER BY FirstName, LastName";
                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// Students ცხრილის ცარიელობის შემოწმება
        /// </summary>
        public bool IsStudentsTableEmpty()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var cmd = new MySqlCommand("SELECT EXISTS (SELECT 1 FROM Students LIMIT 1)", connection);
                var result = cmd.ExecuteScalar();
                return !Convert.ToBoolean(result);
            }
        }

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება ID-ით
        /// </summary>
        public bool StudentExists(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(1) FROM Students WHERE Id = @StudentId AND IsDeleted = 0";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება კოდით
        /// </summary>
        public bool StudentExistsByCode(string studentCode)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(1) FROM Students WHERE StudentCode = @StudentCode AND IsDeleted = 0";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentCode", studentCode);
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება პირადი ნომრით
        /// </summary>
        public bool StudentExistsByPersonalId(long personalId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(1) FROM Students WHERE Id_Numb = @PersonalId AND IsDeleted = 0";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PersonalId", personalId);
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის მიღება
        /// </summary>
        public int GetStudentsCount()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Students WHERE IsDeleted = 0", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// აქტიური მოსწავლეთა რაოდენობის მიღება
        /// </summary>
        public int GetActiveStudentsCount()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Students WHERE IsDeleted = 0 AND (Status IS NULL OR Status = '' OR Status = 'Active')", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        #endregion

        #region ==================== UPDATE - მოსწავლის განახლება (სრული) ====================

        /// <summary>
        /// მოსწავლის სრული განახლება (ყველა ველი)
        /// </summary>
        public bool UpdateStudent(Student student)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE Students 
                              SET FirstName = @FirstName, LastName = @LastName, Age = @Age, 
                                  ParentName = @ParentName, PhoneNumber = @PhoneNumber, Id_Numb = @Id_Numb,
                                  Address = @Address, RegistrationDate = @RegistrationDate, 
                                  StudentCode = @StudentCode, Status = @Status, Info = @Info,
                                  IdCardPath = @IdCardPath, AdditionalDocsPath = @AdditionalDocsPath, 
                                  user_id = @user_id, Balance = @Balance, UpdatedAt = @UpdatedAt
                              WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", student.Id);
                    cmd.Parameters.AddWithValue("@FirstName", student.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", student.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Age", student.Age);
                    cmd.Parameters.AddWithValue("@ParentName", student.ParentName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id_Numb", student.Id_Numb);
                    cmd.Parameters.AddWithValue("@Address", student.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegistrationDate", student.RegistrationDate);
                    cmd.Parameters.AddWithValue("@StudentCode", student.StudentCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", student.Status ? "Active" : "Inactive");
                    cmd.Parameters.AddWithValue("@Info", student.Info ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdCardPath", student.IdCardPath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AdditionalDocsPath", student.AdditionalDocsPath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@user_id", student.User_Id);
                    cmd.Parameters.AddWithValue("@Balance", student.Balance);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - დინამიური ველების განახლება ====================

        /// <summary>
        /// მხოლოდ კონკრეტული ველების განახლება (დინამიური)
        /// </summary>
        public bool UpdateStudentFields(int studentId, Dictionary<string, object> changedFields)
        {
            if (changedFields == null || changedFields.Count == 0) return false;

            // დაშვებული ველების სია (SQL Injection-ისგან დაცვა)
            var allowedFields = new HashSet<string>
            {
                "FirstName", "LastName", "ParentName", "Age", "PhoneNumber", 
                "Id_Numb", "Address", "Info", "Balance", "Status", "StudentCode",
                "IdCardPath", "AdditionalDocsPath"
            };

            // ფილტრაცია მხოლოდ დაშვებული ველებით
            var validFields = changedFields.Where(f => allowedFields.Contains(f.Key)).ToList();
            if (validFields.Count == 0) return false;

            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();

                // დინამიური SQL აგება
                var setClauses = validFields.Select(f => $"{f.Key} = @{f.Key}").ToList();
                setClauses.Add("UpdatedAt = @UpdatedAt");
                
                var query = $"UPDATE Students SET {string.Join(", ", setClauses)} WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    foreach (var field in validFields)
                    {
                        cmd.Parameters.AddWithValue($"@{field.Key}", field.Value ?? DBNull.Value);
                    }

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - ცალკეული ველების განახლება ====================

        /// <summary>
        /// მოსწავლის სახელის განახლება
        /// </summary>
        public bool UpdateStudentFirstName(int studentId, string firstName)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET FirstName = @FirstName, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@FirstName", firstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის გვარის განახლება
        /// </summary>
        public bool UpdateStudentLastName(int studentId, string lastName)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET LastName = @LastName, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@LastName", lastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ტელეფონის განახლება
        /// </summary>
        public bool UpdateStudentPhone(int studentId, string phone)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET PhoneNumber = @PhoneNumber, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის მისამართის განახლება
        /// </summary>
        public bool UpdateStudentAddress(int studentId, string address)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Address = @Address, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Address", address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ასაკის განახლება
        /// </summary>
        public bool UpdateStudentAge(int studentId, int age)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Age = @Age, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის მშობლის სახელის განახლება
        /// </summary>
        public bool UpdateStudentParentName(int studentId, string parentName)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET ParentName = @ParentName, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@ParentName", parentName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის პირადი ნომრის განახლება
        /// </summary>
        public bool UpdateStudentPersonalId(int studentId, long personalId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Id_Numb = @Id_Numb, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Id_Numb", personalId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის სტატუსის განახლება (Active/Inactive)
        /// </summary>
        public bool UpdateStudentStatus(int studentId, string status)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Status", status ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის კოდის განახლება
        /// </summary>
        public bool UpdateStudentCode(int studentId, string studentCode)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET StudentCode = @StudentCode, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@StudentCode", studentCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ინფორმაციის (შენიშვნის) განახლება
        /// </summary>
        public bool UpdateStudentInfo(int studentId, string info)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Info = @Info, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Info", info ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის პირადობის ბარათის გზის განახლება
        /// </summary>
        public bool UpdateStudentIdCardPath(int studentId, string path)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET IdCardPath = @IdCardPath, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@IdCardPath", path ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის დამატებითი დოკუმენტების გზის განახლება
        /// </summary>
        public bool UpdateStudentAdditionalDocsPath(int studentId, string path)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET AdditionalDocsPath = @AdditionalDocsPath, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@AdditionalDocsPath", path ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - ბალანსის მართვა ====================

        /// <summary>
        /// მოსწავლის ბალანსის განახლება (ახალი მნიშვნელობით)
        /// </summary>
        public bool UpdateStudentBalance(int studentId, decimal balance)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Balance = @Balance, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Balance", balance);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ბალანსის გაზრდა
        /// </summary>
        public bool IncrementStudentBalance(int studentId, decimal amount)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Balance = Balance + @Amount, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ბალანსის შემცირება
        /// </summary>
        public bool DecrementStudentBalance(int studentId, decimal amount)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET Balance = Balance - @Amount, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== DELETE - მოსწავლის წაშლა ====================

        /// <summary>
        /// მოსწავლის წაშლა (Soft Delete - IsDeleted = 1)
        /// </summary>
        public bool DeleteStudent(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის აღდგენა (Soft Delete-დან - IsDeleted = 0)
        /// </summary>
        public bool RestoreStudent(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET IsDeleted = 0, UpdatedAt = @UpdatedAt WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის სრული წაშლა (Hard Delete - ჩანაწერის წაშლა)
        /// გამოიყენეთ ფრთხილად!
        /// </summary>
        public bool HardDeleteStudent(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "DELETE FROM Students WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== STATISTICS - სტატისტიკის მეთოდები ====================

        /// <summary>
        /// მოსწავლეთა რაოდენობა ჯგუფების მიხედვით
        /// </summary>
        public DataTable GetStudentCountByGroup()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT g.Name AS GroupName, COUNT(sg.StudentId) AS StudentCount
                              FROM `Groups` g
                              LEFT JOIN StudentGroups sg ON g.Id = sg.GroupId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              WHERE g.Status = 1
                              GROUP BY g.Id, g.Name
                              ORDER BY g.Name";

                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// მოსწავლეთა რეგისტრაცია თვეების მიხედვით
        /// </summary>
        public DataTable GetStudentsByMonth()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT DATE_FORMAT(RegistrationDate, '%Y-%m') AS Month, COUNT(*) AS StudentCount
                              FROM Students
                              WHERE IsDeleted = 0
                              GROUP BY DATE_FORMAT(RegistrationDate, '%Y-%m')
                              ORDER BY Month DESC
                              LIMIT 12";

                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        #endregion

        #region ==================== SEARCH & FILTER - ძებნა და ფილტრაცია ====================

        /// <summary>
        /// მოსწავლეების ფილტრაცია (სახელით, ჯგუფით, თარიღით)
        /// </summary>
        public DataTable FilterStudents(string name, int? groupId, int? subGroupId, DateTime? startDate, DateTime? endDate)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT DISTINCT s.Id, s.FirstName, s.LastName, s.PhoneNumber, s.Address, 
                              s.RegistrationDate, s.StudentCode, g.Name AS GroupName
                              FROM Students s
                              LEFT JOIN StudentGroups sg ON s.Id = sg.StudentId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              LEFT JOIN StudentSubGroups ssg ON s.Id = ssg.StudentId AND ssg.Status = 1 AND (ssg.IsDeleted = 0 OR ssg.IsDeleted IS NULL)
                              WHERE s.IsDeleted = 0";

                if (!string.IsNullOrEmpty(name))
                    query += " AND (s.FirstName LIKE @Name OR s.LastName LIKE @Name)";
                if (groupId.HasValue)
                    query += " AND sg.GroupId = @GroupId";
                if (subGroupId.HasValue)
                    query += " AND ssg.SubGroupId = @SubGroupId";
                if (startDate.HasValue)
                    query += " AND s.RegistrationDate >= @StartDate";
                if (endDate.HasValue)
                    query += " AND s.RegistrationDate <= @EndDate";

                query += " ORDER BY s.LastName, s.FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(name))
                        cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
                    if (groupId.HasValue)
                        cmd.Parameters.AddWithValue("@GroupId", groupId.Value);
                    if (subGroupId.HasValue)
                        cmd.Parameters.AddWithValue("@SubGroupId", subGroupId.Value);
                    if (startDate.HasValue)
                        cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        cmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        /// <summary>
        /// მოსწავლეების ძებნა სახელით და გვარით ჯგუფში
        /// </summary>
        public List<Student> SearchStudentsByNameAndGroup(string text, int groupId)
        {
            var students = new List<Student>();
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT s.* FROM Students s
                              INNER JOIN StudentGroups sg ON s.Id = sg.StudentId
                              WHERE sg.GroupId = @GroupId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              AND s.IsDeleted = 0
                              AND (s.FirstName LIKE @Text OR s.LastName LIKE @Text OR CONCAT(s.FirstName, ' ', s.LastName) LIKE @Text)
                              ORDER BY s.LastName, s.FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Text", "%" + text + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(MapStudentFromReader(reader));
                        }
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// მოსწავლეების ძებნა სახელით ყველა ჯგუფში
        /// </summary>
        public List<Student> SearchStudentsByNameAcrossAllGroups(string name)
        {
            var students = new List<Student>();
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT DISTINCT s.* FROM Students s
                              WHERE s.IsDeleted = 0
                              AND (s.FirstName LIKE @Name OR s.LastName LIKE @Name OR CONCAT(s.FirstName, ' ', s.LastName) LIKE @Name)
                              ORDER BY s.LastName, s.FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(MapStudentFromReader(reader));
                        }
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// მოსწავლეების ძებნა ველით და მნიშვნელობით (ოფციონალური ჯგუფით)
        /// მოსწავლე ბრუნდება მხოლოდ ერთხელ, ჯგუფები მძიმით გამოყოფილი
        /// </summary>
        public List<Student> SearchStudents(string fieldName, string searchText, int? groupId = null)
        {
            var result = new List<Student>();

            // ველის სახელის ვალიდაცია SQL Injection-ისგან დასაცავად
            var allowedFields = new List<string> { "FirstName", "LastName", "ParentName", "Age", "Id_Numb", "Address", "StudentCode", "PhoneNumber" };
            if (!allowedFields.Contains(fieldName))
                return result;

            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();

                string query;
                if (groupId.HasValue && groupId.Value > 0)
                {
                    // კონკრეტული ჯგუფის ძებნა - მოსწავლე ერთხელ ჩანს ამ ჯგუფის სახელით
                    query = $@"
                        SELECT s.Id, s.FirstName, s.LastName, s.Age, s.ParentName, s.PhoneNumber, 
                               s.Id_Numb, s.Address, s.RegistrationDate, s.StudentCode, s.IdCardPath, 
                               s.AdditionalDocsPath, s.user_id, s.Balance, s.Status, s.Info, s.UpdatedAt,
                               sg.Status AS ActiveStatus, sg.DateOfPayment, sg.PaymentStatus, sg.Price, sg.Discount,
                               sg.GroupId AS GroupId, g.Name AS GroupName
                        FROM Students s
                        INNER JOIN StudentGroups sg ON sg.StudentId = s.Id AND sg.GroupId = @groupId
                        INNER JOIN `Groups` g ON g.Id = sg.GroupId
                        WHERE s.IsDeleted = 0 AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                        AND s.{fieldName} LIKE @searchText
                        ORDER BY s.LastName, s.FirstName";
                }
                else
                {
                    // ყველა ჯგუფში ძებნა - მოსწავლე ერთხელ ჩანს, ჯგუფები მძიმით გამოყოფილი
                    query = $@"
                        SELECT s.Id, s.FirstName, s.LastName, s.Age, s.ParentName, s.PhoneNumber, 
                               s.Id_Numb, s.Address, s.RegistrationDate, s.StudentCode, s.IdCardPath, 
                               s.AdditionalDocsPath, s.user_id, s.Balance, s.Status, s.Info, s.UpdatedAt,
                               MAX(sg.Status) AS ActiveStatus, 
                               MAX(sg.DateOfPayment) AS DateOfPayment, 
                               MAX(sg.PaymentStatus) AS PaymentStatus, 
                               MAX(sg.Price) AS Price, 
                               MAX(sg.Discount) AS Discount,
                               MIN(sg.GroupId) AS GroupId, 
                               GROUP_CONCAT(DISTINCT g.Name ORDER BY g.Name SEPARATOR ', ') AS GroupName
                        FROM Students s
                        LEFT JOIN StudentGroups sg ON sg.StudentId = s.Id AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                        LEFT JOIN `Groups` g ON g.Id = sg.GroupId
                        WHERE s.IsDeleted = 0 AND s.{fieldName} LIKE @searchText
                        GROUP BY s.Id, s.FirstName, s.LastName, s.Age, s.ParentName, s.PhoneNumber, 
                                 s.Id_Numb, s.Address, s.RegistrationDate, s.StudentCode, s.IdCardPath, 
                                 s.AdditionalDocsPath, s.user_id, s.Balance, s.Status, s.Info, s.UpdatedAt
                        ORDER BY s.LastName, s.FirstName";
                }

                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (groupId.HasValue && groupId.Value > 0)
                        cmd.Parameters.AddWithValue("@groupId", groupId.Value);

                    cmd.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(MapStudentWithGroupFromReader(reader));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// მოსწავლეების მიღება ჯგუფების ინფორმაციით
        /// მოსწავლე ბრუნდება მხოლოდ ერთხელ, ჯგუფები მძიმით გამოყოფილი
        /// </summary>
        public List<Student> GetAllStudentsWithGroups()
        {
            var students = new List<Student>();
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT s.Id, s.FirstName, s.LastName, s.Age, s.ParentName, s.PhoneNumber, 
                                     s.Id_Numb, s.Address, s.RegistrationDate, s.StudentCode, s.IdCardPath, 
                                     s.AdditionalDocsPath, s.user_id, s.Balance, s.Status, s.Info, s.UpdatedAt, s.IsDeleted,
                                     GROUP_CONCAT(DISTINCT g.Name ORDER BY g.Name SEPARATOR ', ') AS GroupName 
                              FROM Students s
                              LEFT JOIN StudentGroups sg ON s.Id = sg.StudentId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE s.IsDeleted = 0
                              GROUP BY s.Id, s.FirstName, s.LastName, s.Age, s.ParentName, s.PhoneNumber, 
                                       s.Id_Numb, s.Address, s.RegistrationDate, s.StudentCode, s.IdCardPath, 
                                       s.AdditionalDocsPath, s.user_id, s.Balance, s.Status, s.Info, s.UpdatedAt, s.IsDeleted
                              ORDER BY s.LastName, s.FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var student = MapStudentFromReader(reader);
                        student.GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName");
                        students.Add(student);
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// მოსწავლის სახელების სია (Id, FullName) - ComboBox-ისთვის
        /// </summary>
        public List<(int Id, string FullName)> GetStudentNames()
        {
            var names = new List<(int Id, string FullName)>();
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Id, CONCAT(FirstName, ' ', LastName) AS FullName FROM Students WHERE IsDeleted = 0 ORDER BY LastName, FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        names.Add((reader.GetInt32("Id"), reader.GetString("FullName")));
                    }
                }
            }
            return names;
        }

        /// <summary>
        /// გაუნაწილებელი მოსწავლეები (ჯგუფის გარეშე)
        /// </summary>
        public DataTable GetUnassignedStudents()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT s.Id, s.FirstName, s.LastName, s.PhoneNumber, s.Address, s.RegistrationDate, s.StudentCode
                              FROM Students s
                              LEFT JOIN StudentGroups sg ON s.Id = sg.StudentId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              WHERE s.IsDeleted = 0 AND sg.Id IS NULL
                              ORDER BY s.LastName, s.FirstName";

                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// მოსწავლეები DataTable-ად (ფორმისთვის)
        /// </summary>
        public DataTable GetAllStudentsFor()
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT s.Id, s.FirstName, s.LastName, s.PhoneNumber, s.Address, 
                              s.RegistrationDate, s.StudentCode, s.ParentName, s.Age, s.Balance
                              FROM Students s
                              WHERE s.IsDeleted = 0
                              ORDER BY s.LastName, s.FirstName";

                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// მოსწავლეების მცირე ინფორმაცია (StudentViewDto)
        /// </summary>
        public List<StudentViewDto> GetAllStudentsSomeInfo()
        {
            var students = new List<StudentViewDto>();
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                // GROUP BY და GROUP_CONCAT გამოყენება - მოსწავლე მხოლოდ ერთხელ ჩანს
                var query = @"SELECT s.Id, s.FirstName, s.LastName, s.StudentCode, s.PhoneNumber, s.Balance, s.Age,
                                     s.ParentName, s.Id_Numb, s.Address, s.RegistrationDate, s.Status,
                                     GROUP_CONCAT(DISTINCT g.Name ORDER BY g.Name SEPARATOR ', ') AS GroupName
                              FROM Students s
                              LEFT JOIN StudentGroups sg ON s.Id = sg.StudentId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE s.IsDeleted = 0
                              GROUP BY s.Id, s.FirstName, s.LastName, s.StudentCode, s.PhoneNumber, s.Balance, s.Age,
                                       s.ParentName, s.Id_Numb, s.Address, s.RegistrationDate, s.Status
                              ORDER BY s.LastName, s.FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new StudentViewDto
                        {
                            Id = reader.GetInt32("Id"),
                            FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString("FirstName"),
                            LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString("LastName"),
                            StudentCode = reader.IsDBNull(reader.GetOrdinal("StudentCode")) ? null : reader.GetString("StudentCode"),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString("PhoneNumber"),
                            Balance = reader.IsDBNull(reader.GetOrdinal("Balance")) ? 0 : reader.GetDecimal("Balance"),
                            Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? 0 : reader.GetInt32("Age"),
                            ParentName = reader.IsDBNull(reader.GetOrdinal("ParentName")) ? null : reader.GetString("ParentName"),
                            Id_Numb = reader.IsDBNull(reader.GetOrdinal("Id_Numb")) ? 0 : reader.GetInt64("Id_Numb"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                            RegistrationDate = reader.IsDBNull(reader.GetOrdinal("RegistrationDate")) ? (DateTime?)null : reader.GetDateTime("RegistrationDate"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? false : reader.GetBoolean("Status"),
                            GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName")
                        });
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// მოსწავლეები ჯგუფის მიხედვით
        /// </summary>
        public List<Student> GetStudentsByGroupId(int groupId)
        {
            var students = new List<Student>();
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT s.* FROM Students s
                              INNER JOIN StudentGroups sg ON s.Id = sg.StudentId
                              WHERE sg.GroupId = @GroupId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              AND s.IsDeleted = 0
                              ORDER BY s.LastName, s.FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(MapStudentFromReader(reader));
                        }
                    }
                }
            }
            return students;
        }

        /// <summary>
        /// მოსწავლის დეტალები ID-ით და GroupId-ით
        /// </summary>
        public Student GetStudentDetailsById(int studentId, int groupId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                
                string query;
                // თუ groupId არის 0 ან უარყოფითი, მხოლოდ studentId-ით ვეძებთ
                if (groupId <= 0)
                {
                    query = @"SELECT s.*, 
                                     MAX(sg.PaymentStatus) AS PaymentStatus, 
                                     MAX(sg.DateOfPayment) AS DateOfPayment, 
                                     MAX(sg.Price) AS GroupPrice, 
                                     MAX(sg.Discount) AS Discount, 
                                     GROUP_CONCAT(DISTINCT g.Name ORDER BY g.Name SEPARATOR ', ') AS GroupName
                              FROM Students s
                              LEFT JOIN StudentGroups sg ON s.Id = sg.StudentId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE s.Id = @StudentId AND s.IsDeleted = 0
                              GROUP BY s.Id, s.FirstName, s.LastName, s.Age, s.ParentName, s.PhoneNumber, 
                                       s.Id_Numb, s.Address, s.RegistrationDate, s.StudentCode, s.IdCardPath, 
                                       s.AdditionalDocsPath, s.user_id, s.Balance, s.Status, s.Info, s.UpdatedAt, s.IsDeleted";
                }
                else
                {
                    query = @"SELECT s.*, sg.PaymentStatus, sg.DateOfPayment, sg.Price AS GroupPrice, sg.Discount, g.Name AS GroupName
                              FROM Students s
                              INNER JOIN StudentGroups sg ON s.Id = sg.StudentId
                              INNER JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE s.Id = @StudentId AND sg.GroupId = @GroupId AND s.IsDeleted = 0";
                }

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    if (groupId > 0)
                        cmd.Parameters.AddWithValue("@GroupId", groupId);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var student = MapStudentFromReader(reader);
                            student.GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName");
                            
                            // DateOfPayment და PaymentStatus წაკითხვა (თუ არსებობს query-ში)
                            if (HasColumn(reader, "DateOfPayment"))
                                student.DateOfPayment = reader.IsDBNull(reader.GetOrdinal("DateOfPayment")) ? (DateTime?)null : reader.GetDateTime("DateOfPayment");
                            if (HasColumn(reader, "PaymentStatus"))
                                student.PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString("PaymentStatus");
                            if (HasColumn(reader, "GroupPrice"))
                                student.TuitionFee = reader.IsDBNull(reader.GetOrdinal("GroupPrice")) ? 0 : reader.GetDecimal("GroupPrice");
                            if (HasColumn(reader, "Discount"))
                                student.Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? 0 : reader.GetDouble("Discount");
                            
                            return student;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// შემოწმება - არსებობს თუ არა სვეტი DataReader-ში
        /// </summary>
        private bool HasColumn(MySqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// მოსწავლის სახელის მიღება
        /// </summary>
        public string GetStudentName(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT CONCAT(FirstName, ' ', LastName) AS FullName FROM Students WHERE Id = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        #endregion

        #region ==================== DUPLICATE CHECKS - დუბლიკატების შემოწმება ====================

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება სახელით და გვარით
        /// </summary>
        public bool ExistsByName(string firstName, string lastName)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Students WHERE FirstName = @FirstName AND LastName = @LastName AND IsDeleted = 0";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება სახელით, გვარით, მშობლით და მისამართით
        /// </summary>
        public bool ExistsByNameParentAddress(string firstName, string lastName, string parentName, string address)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(*) FROM Students 
                              WHERE FirstName = @FirstName AND LastName = @LastName 
                              AND ParentName = @ParentName AND Address = @Address AND IsDeleted = 0";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@ParentName", parentName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", address ?? (object)DBNull.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლეების რაოდენობა მისამართით
        /// </summary>
        public int CountByAddress(string address)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Students WHERE Address = @Address AND IsDeleted = 0";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Address", address ?? (object)DBNull.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion

        #region ==================== STUDENT GROUPS - StudentGroups ცხრილთან მუშაობა ====================

        /// <summary>
        /// მოსწავლე-ჯგუფის კავშირის არსებობა (აქტიური)
        /// </summary>
        public bool StudentGroupExists(int studentId, int groupId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(*) FROM StudentGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId 
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლე-ჯგუფის სტატუსის განახლება
        /// </summary>
        public bool UpdateStudentStatus(int studentId, int groupId, bool status)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups SET Status = @Status, UpdatedAt = @UpdatedAt 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლე-ჯგუფის ველების განახლება
        /// </summary>
        public void UpdateStudentGroupFields(StudentGroups original, StudentGroups updated)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups SET 
                              PaymentStatus = @PaymentStatus, DateOfPayment = @DateOfPayment, 
                              Price = @Price, Discount = @Discount, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PaymentStatus", updated.PaymentStatus ?? original.PaymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfPayment", updated.DateOfPayment ?? original.DateOfPayment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Price", updated.Price != 0 ? updated.Price : original.Price);
                    cmd.Parameters.AddWithValue("@Discount", updated.Discount != 0 ? updated.Discount : original.Discount);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", original.StudentId);
                    cmd.Parameters.AddWithValue("@GroupId", original.GroupId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// მოსწავლის ჯგუფის ID-ის განახლება
        /// </summary>
        public bool UpdateStudentGroupId(int studentId, int newGroupId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups SET GroupId = @NewGroupId, UpdatedAt = @UpdatedAt 
                              WHERE StudentId = @StudentId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@NewGroupId", newGroupId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ყველა ჯგუფიდან ამოღება (soft delete)
        /// </summary>
        public void RemoveStudentFromGroups(int studentId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                
                // StudentGroups soft delete
                var query1 = @"UPDATE StudentGroups SET Status = 0, IsDeleted = 1, UpdatedAt = @UpdatedAt 
                               WHERE StudentId = @StudentId";
                using (var cmd = new MySqlCommand(query1, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.ExecuteNonQuery();
                }

                // StudentSubGroups soft delete
                var query2 = @"UPDATE StudentSubGroups SET Status = 0, IsDeleted = 1, UpdatedAt = @UpdatedAt 
                               WHERE StudentId = @StudentId";
                using (var cmd = new MySqlCommand(query2, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// მოსწავლის კონკრეტული ჯგუფიდან ამოღება (soft delete)
        /// </summary>
        public void RemoveStudentFromGroup(int studentId, int groupId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                
                // StudentGroups soft delete
                var query1 = @"UPDATE StudentGroups SET Status = 0, IsDeleted = 1, UpdatedAt = @UpdatedAt 
                               WHERE StudentId = @StudentId AND GroupId = @GroupId";
                using (var cmd = new MySqlCommand(query1, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.ExecuteNonQuery();
                }

                // StudentSubGroups soft delete
                var query2 = @"UPDATE StudentSubGroups SET Status = 0, IsDeleted = 1, UpdatedAt = @UpdatedAt 
                               WHERE StudentId = @StudentId AND GroupId = @GroupId";
                using (var cmd = new MySqlCommand(query2, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region ==================== DELETE - მოსწავლის წაშლა (userId-ით) ====================

        /// <summary>
        /// მოსწავლის წაშლა userId-ით (Soft Delete - IsDeleted = 1)
        /// </summary>
        public bool DeleteStudent(int studentId, int userId)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE Students SET IsDeleted = 1, Status = 'Inactive', UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", studentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== MIGRATION - მიგრაცია ====================

        /// <summary>
        /// StudentGroups-ის მიგრაცია
        /// </summary>
        public void MigrateStudentGroups()
        {
            // მიგრაციის ლოგიკა თუ საჭიროა
            // ცარიელი იმპლემენტაცია - საჭიროების შემთხვევაში დაემატება
        }

        #endregion

        #region ==================== IMPORT - იმპორტისთვის საჭირო მეთოდები ====================

        /// <summary>
        /// მოსწავლის გასაღებების მიღება (იმპორტის დროს დუბლიკატების შესამოწმებლად)
        /// </summary>
        public List<StudentKey> GetStudentsForImport()
        {
            var keys = new List<StudentKey>();

            using (var connection = _dbHelper.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Id, FirstName, LastName, Id_Numb, Address, StudentCode FROM Students WHERE IsDeleted = 0";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        keys.Add(new StudentKey
                        {
                            Id = reader.GetInt32("Id"),
                            FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString("FirstName"),
                            LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString("LastName"),
                            IdNumb = reader.IsDBNull(reader.GetOrdinal("Id_Numb")) ? 0 : reader.GetInt64("Id_Numb"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                            StudentCode = reader.IsDBNull(reader.GetOrdinal("StudentCode")) ? null : reader.GetString("StudentCode")
                        });
                    }
                }
            }
            return keys;
        }

        #endregion

        #region ==================== HELPER - დამხმარე მეთოდები ====================

        /// <summary>
        /// Student ობიექტის შექმნა DataReader-დან
        /// </summary>
        private Student MapStudentFromReader(MySqlDataReader reader)
        {
            var statusValue = reader.IsDBNull(reader.GetOrdinal("Status")) ? false : reader.GetBoolean("Status");

            return new Student
            {
                Id = reader.GetInt32("Id"),
                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString("FirstName"),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString("LastName"),
                Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? 0 : reader.GetInt32("Age"),
                ParentName = reader.IsDBNull(reader.GetOrdinal("ParentName")) ? null : reader.GetString("ParentName"),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString("PhoneNumber"),
                Id_Numb = reader.IsDBNull(reader.GetOrdinal("Id_Numb")) ? 0 : reader.GetInt64("Id_Numb"),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                RegistrationDate = reader.IsDBNull(reader.GetOrdinal("RegistrationDate")) ? DateTime.MinValue : reader.GetDateTime("RegistrationDate"),
                StudentCode = reader.IsDBNull(reader.GetOrdinal("StudentCode")) ? null : reader.GetString("StudentCode"),
                Status = statusValue,
                Info = reader.IsDBNull(reader.GetOrdinal("Info")) ? null : reader.GetString("Info"),
                IdCardPath = reader.IsDBNull(reader.GetOrdinal("IdCardPath")) ? null : reader.GetString("IdCardPath"),
                AdditionalDocsPath = reader.IsDBNull(reader.GetOrdinal("AdditionalDocsPath")) ? null : reader.GetString("AdditionalDocsPath"),
                User_Id = reader.IsDBNull(reader.GetOrdinal("user_id")) ? 0 : reader.GetInt32("user_id"),
                Balance = reader.IsDBNull(reader.GetOrdinal("Balance")) ? 0 : reader.GetDecimal("Balance"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
            };
        }

        /// <summary>
        /// Student ობიექტის შექმნა DataReader-დან (ჯგუფის ინფორმაციით)
        /// </summary>
        private Student MapStudentWithGroupFromReader(MySqlDataReader reader)
        {
            var statusValue = reader.IsDBNull(reader.GetOrdinal("Status")) ? false : reader.GetBoolean("Status");

            return new Student
            {
                Id = reader.GetInt32("Id"),
                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString("FirstName"),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString("LastName"),
                Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? 0 : reader.GetInt32("Age"),
                ParentName = reader.IsDBNull(reader.GetOrdinal("ParentName")) ? null : reader.GetString("ParentName"),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString("PhoneNumber"),
                Id_Numb = reader.IsDBNull(reader.GetOrdinal("Id_Numb")) ? 0 : reader.GetInt64("Id_Numb"),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                RegistrationDate = reader.IsDBNull(reader.GetOrdinal("RegistrationDate")) ? DateTime.MinValue : reader.GetDateTime("RegistrationDate"),
                StudentCode = reader.IsDBNull(reader.GetOrdinal("StudentCode")) ? null : reader.GetString("StudentCode"),
                Status = statusValue,
                Info = reader.IsDBNull(reader.GetOrdinal("Info")) ? null : reader.GetString("Info"),
                IdCardPath = reader.IsDBNull(reader.GetOrdinal("IdCardPath")) ? null : reader.GetString("IdCardPath"),
                AdditionalDocsPath = reader.IsDBNull(reader.GetOrdinal("AdditionalDocsPath")) ? null : reader.GetString("AdditionalDocsPath"),
                User_Id = reader.IsDBNull(reader.GetOrdinal("user_id")) ? 0 : reader.GetInt32("user_id"),
                Balance = reader.IsDBNull(reader.GetOrdinal("Balance")) ? 0 : reader.GetDecimal("Balance"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt"),
                // ჯგუფის ინფორმაცია
                GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName")
            };
        }

        #endregion
    }
}


