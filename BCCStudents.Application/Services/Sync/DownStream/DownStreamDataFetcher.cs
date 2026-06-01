using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;


namespace BCCStudents.Application.Services.Sync.DownStream
{

    /// <summary>
    /// იღებს მონაცემებს სერვერის MySQL ბაზიდან UpdatedAt + Id ფილტრის მიხედვით.
    /// </summary>
    /// [System.Runtime.Versioning.SupportedOSPlatform("windows")
    /// 
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class DownStreamDataFetcher : IDownStreamDataFetcher
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly ISyncLogger _logger;

        public DownStreamDataFetcher(IDatabaseConnectionProvider connectionProvider, ISyncLogger logger)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<List<Student>> FetchStudentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                                        RegistrationDate, StudentCode, Info, user_id, Balance, UpdatedAt
                                 FROM Students
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("Students", sql, lastSyncedAt, lastSyncedId, MapStudent, cancellationToken));
        }

        public Task<List<Group>> FetchGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, Name, Price, Teacher, Status, StudentCount, MaxStudents,
                                        ContractTemplatePath, UpdatedAt
                                 FROM `Groups`
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("Groups", sql, lastSyncedAt, lastSyncedId, MapGroup, cancellationToken));
        }

        public Task<List<SubGroup>> FetchSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            // სერვერზე SubGroups ცხრილს არ აქვს ActiveStatus სვეტი, ამიტომ მხოლოდ Status-ს ვკითხულობთ
            const string sql = @"SELECT Id, Name, GroupId, TuitionFee, StudentCount, MaxStudents, Status, UpdatedAt
                                 FROM SubGroups
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("SubGroups", sql, lastSyncedAt, lastSyncedId, MapSubGroup, cancellationToken));
        }

        public Task<List<StudentGroups>> FetchStudentGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT ID AS Id, StudentId, GroupId, PaymentStatus, DateOfPayment,
                                        Price, Discount, Status, UpdatedAt
                                 FROM StudentGroups
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND ID > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, ID ASC;";
            return Task.FromResult(ExecuteReader("StudentGroups", sql, lastSyncedAt, lastSyncedId, MapStudentGroup, cancellationToken));
        }

        public Task<List<StudentSubGroups>> FetchStudentSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, StudentId, GroupId, SubGroupId, Status, PaymentStatus,
                                        DateOfPayment, Price, discount AS Discount, UpdatedAt
                                 FROM StudentSubGroups
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("StudentSubGroups", sql, lastSyncedAt, lastSyncedId, MapStudentSubGroup, cancellationToken));
        }

        public Task<List<Payment>> FetchPaymentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, StudentId, GroupId, Amount, PaymentDate, PaymentStatus, Description, PayerName, PersonalId, UpdatedAt, IsDeleted
                                 FROM Payments
                                 WHERE (@LastSyncedAt IS NULL AND @LastSyncedId = 0)
                                    OR (COALESCE(UpdatedAt, PaymentDate) > @LastSyncedAt)
                                    OR (COALESCE(UpdatedAt, PaymentDate) = @LastSyncedAt AND Id > @LastSyncedId)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY COALESCE(UpdatedAt, PaymentDate) ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("Payments", sql, lastSyncedAt, lastSyncedId, MapPayment, cancellationToken));
        }

        public Task<List<FailedPayment>> FetchFailedPaymentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, RowNumber, PaymentDate, Amount, PersonalId, Description, Reason, CreatedAt
                                 FROM FailedPayments
                                 WHERE (@LastSyncedAt IS NULL AND @LastSyncedId = 0)
                                    OR (CreatedAt > @LastSyncedAt)
                                    OR (CreatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY CreatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("FailedPayments", sql, lastSyncedAt, lastSyncedId, MapFailedPayment, cancellationToken));
        }

        public Task<List<ImportedPaymentLog>> FetchImportedPaymentLogsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, PaymentDate, Amount, PersonalId, Description, ImportSource, CreatedAt
                                 FROM ImportedPaymentsLog
                                 WHERE (@LastSyncedAt IS NULL AND @LastSyncedId = 0)
                                    OR (CreatedAt > @LastSyncedAt)
                                    OR (CreatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY CreatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("ImportedPaymentsLog", sql, lastSyncedAt, lastSyncedId, MapImportedPaymentLog, cancellationToken));
        }

        public Task<List<UserModel>> FetchUsersAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, Username, FullName, Email, Password, Role, CreatedAt, LastLogin
                                 FROM Users
                                 WHERE (@LastSyncedAt IS NULL AND @LastSyncedId = 0)
                                    OR (COALESCE(LastLogin, CreatedAt) > @LastSyncedAt)
                                    OR (COALESCE(LastLogin, CreatedAt) = @LastSyncedAt AND Id > @LastSyncedId)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY COALESCE(LastLogin, CreatedAt) ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("Users", sql, lastSyncedAt, lastSyncedId, MapUser, cancellationToken));
        }

        public Task<List<SystemConfiguration>> FetchSystemConfigAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, `Key`, `Value`, `Type`, `Description`, CreatedAt, UpdatedAt
                                 FROM SystemConfig
                                 WHERE (@LastSyncedAt IS NULL AND @LastSyncedId = 0)
                                    OR (COALESCE(UpdatedAt, CreatedAt) > @LastSyncedAt)
                                    OR (COALESCE(UpdatedAt, CreatedAt) = @LastSyncedAt AND Id > @LastSyncedId)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY COALESCE(UpdatedAt, CreatedAt) ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("SystemConfig", sql, lastSyncedAt, lastSyncedId, MapSystemConfiguration, cancellationToken));
        }

        public Task<List<PendingStudent>> FetchPendingStudentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                                         IdCardPath, AdditionalDocsPath, user_id, CreatedAt
                                 FROM PendingStudents
                                 WHERE (@LastSyncedAt IS NULL AND @LastSyncedId = 0)
                                    OR (CreatedAt > @LastSyncedAt)
                                    OR (CreatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY CreatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("PendingStudents", sql, lastSyncedAt, lastSyncedId, MapPendingStudent, cancellationToken));
        }

        public Task<List<PendingStudentGroup>> FetchPendingStudentGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, StudentId, GroupId
                                 FROM PendingStudentGroups
                                 WHERE (@LastSyncedId = 0)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY Id ASC;";
            return Task.FromResult(ExecuteReader("PendingStudentGroups", sql, lastSyncedAt, lastSyncedId, MapPendingStudentGroup, cancellationToken));
        }

        public Task<List<PendingStudentSubGroup>> FetchPendingStudentSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, StudentId, GroupId, SubGroupId
                                 FROM PendingStudentSubGroups
                                 WHERE (@LastSyncedId = 0)
                                    OR (Id > @LastSyncedId)
                                 ORDER BY Id ASC;";
            return Task.FromResult(ExecuteReader("PendingStudentSubGroups", sql, lastSyncedAt, lastSyncedId, MapPendingStudentSubGroup, cancellationToken));
        }

        private List<T> ExecuteReader<T>(string tableName, string sql, DateTime? lastSyncedAt, int lastSyncedId, Func<MySqlDataReader, T> mapper, CancellationToken cancellationToken)
        {
            var results = new List<T>();
            try
            {
                using (var connection = _connectionProvider.GetServerConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@LastSyncedAt", (object)lastSyncedAt ?? DBNull.Value);
                        command.Parameters.AddWithValue("@LastSyncedId", lastSyncedId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                results.Add(mapper(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"DownStreamDataFetcher query failed for table {tableName}.", ex);
                throw;
            }

            return results;
        }

        #region Mappers

        private static Student MapStudent(MySqlDataReader reader)
        {
            return new Student
            {
                Id = reader.GetInt32("Id"),
                FirstName = reader["FirstName"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                Age = reader["Age"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Age"]),
                ParentName = reader["ParentName"]?.ToString(),
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                Id_Numb = reader["Id_Numb"] == DBNull.Value ? 0 : Convert.ToInt64(reader["Id_Numb"]),
                Address = reader["Address"]?.ToString(),
                RegistrationDate = reader["RegistrationDate"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["RegistrationDate"]),
                StudentCode = reader["StudentCode"]?.ToString(),
                Info = reader["Info"]?.ToString(),
                User_Id = reader["user_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["user_id"]),
                Balance = reader["Balance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Balance"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static Group MapGroup(MySqlDataReader reader)
        {
            return new Group
            {
                Id = reader.GetInt32("Id"),
                Name = reader["Name"]?.ToString(),
                Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]),
                Teacher = reader["Teacher"]?.ToString(),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                StudentCount = reader["StudentCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentCount"]),
                ContractTemplatePath = reader["ContractTemplatePath"]?.ToString(),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static SubGroup MapSubGroup(MySqlDataReader reader)
        {
            return new SubGroup
            {
                Id = reader.GetInt32("Id"),
                Name = reader["Name"]?.ToString(),
                GroupId = reader["GroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GroupId"]),
                TuitionFee = reader["TuitionFee"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TuitionFee"]),
                StudentCount = reader["StudentCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentCount"]),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                //Status = reader["ActiveStatus"] != DBNull.Value && Convert.ToBoolean(reader["ActiveStatus"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static StudentGroups MapStudentGroup(MySqlDataReader reader)
        {
            return new StudentGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader["StudentId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentId"]),
                GroupId = reader["GroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GroupId"]),
                PaymentStatus = reader["PaymentStatus"]?.ToString(),
                DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateOfPayment"]),
                Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]),
                Discount = reader["Discount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Discount"]),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static StudentSubGroups MapStudentSubGroup(MySqlDataReader reader)
        {
            return new StudentSubGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader["StudentId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentId"]),
                GroupId = reader["GroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GroupId"]),
                SubGroupId = reader["SubGroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SubGroupId"]),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                PaymentStatus = reader["PaymentStatus"] == DBNull.Value ? null : reader.GetString("PaymentStatus"),
                DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateOfPayment"]),
                Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]),
                Discount = reader["Discount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Discount"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static Payment MapPayment(MySqlDataReader reader)
        {
            return new Payment
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader["StudentId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["StudentId"]),
                GroupId = reader["GroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GroupId"]),
                Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"]),
                PaymentDate = reader["PaymentDate"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["PaymentDate"]),
                PaymentStatus = reader["PaymentStatus"]?.ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"]?.ToString(),
                PayerName = reader["PayerName"] == DBNull.Value ? null : reader["PayerName"]?.ToString(),
                PersonalId = reader["PersonalId"] == DBNull.Value ? (long?)null : Convert.ToInt64(reader["PersonalId"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"]),
                IsDeleted = reader["IsDeleted"] != DBNull.Value && Convert.ToBoolean(reader["IsDeleted"])
            };
        }

        private static FailedPayment MapFailedPayment(MySqlDataReader reader)
        {
            return new FailedPayment
            {
                Id = reader.GetInt32("Id"),
                RowNumber = reader.GetInt32("RowNumber"),
                PaymentDate = reader["PaymentDate"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["PaymentDate"]),
                Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"]),
                PersonalId = reader["PersonalId"] == DBNull.Value ? (long?)null : Convert.ToInt64(reader["PersonalId"]),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"]?.ToString(),
                Reason = reader["Reason"]?.ToString(),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["CreatedAt"])
            };
        }

        private static ImportedPaymentLog MapImportedPaymentLog(MySqlDataReader reader)
        {
            return new ImportedPaymentLog
            {
                Id = reader.GetInt32("Id"),
                PaymentDate = reader["PaymentDate"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["PaymentDate"]),
                Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"]),
                PersonalId = reader["PersonalId"] == DBNull.Value ? (long?)null : Convert.ToInt64(reader["PersonalId"]),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"]?.ToString(),
                ImportSource = reader["ImportSource"] == DBNull.Value ? null : reader["ImportSource"]?.ToString(),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["CreatedAt"])
            };
        }

        private static UserModel MapUser(MySqlDataReader reader)
        {
            return new UserModel
            {
                Id = reader.GetInt32("Id"),
                UserName = reader["Username"]?.ToString(),
                Password = reader["Password"] == DBNull.Value ? null : reader["Password"]?.ToString(),
                FullName = reader["FullName"] == DBNull.Value ? null : reader["FullName"]?.ToString(),
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"]?.ToString(),
                Role = reader["Role"] == DBNull.Value ? null : reader["Role"]?.ToString(),
                Permissions = null,
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedAt"]),
                LastLogin = reader["LastLogin"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastLogin"])
            };
        }

        private static SystemConfiguration MapSystemConfiguration(MySqlDataReader reader)
        {
            return new SystemConfiguration
            {
                Id = reader.GetInt32("Id"),
                Key = reader["Key"]?.ToString(),
                Value = reader["Value"]?.ToString(),
                Type = reader["Type"]?.ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"]?.ToString(),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static PendingStudent MapPendingStudent(MySqlDataReader reader)
        {
            return new PendingStudent
            {
                Id = reader.GetInt32("Id"),
                FirstName = reader["FirstName"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                Age = reader["Age"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Age"]),
                ParentName = reader["ParentName"] == DBNull.Value ? null : reader["ParentName"]?.ToString(),
                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : reader["PhoneNumber"]?.ToString(),
                Id_Numb = reader["Id_Numb"] == DBNull.Value ? 0 : Convert.ToInt64(reader["Id_Numb"]),
                Address = reader["Address"] == DBNull.Value ? null : reader["Address"]?.ToString(),
                IdCardPath = reader["IdCardPath"] == DBNull.Value ? null : reader["IdCardPath"]?.ToString(),
                AdditionalDocsPath = reader["AdditionalDocsPath"] == DBNull.Value ? null : reader["AdditionalDocsPath"]?.ToString(),
                UserId = reader["user_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["user_id"]),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["CreatedAt"])
            };
        }

        private static PendingStudentGroup MapPendingStudentGroup(MySqlDataReader reader)
        {
            return new PendingStudentGroup
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader.GetInt32("StudentId"),
                GroupId = reader.GetInt32("GroupId")
            };
        }

        private static PendingStudentSubGroup MapPendingStudentSubGroup(MySqlDataReader reader)
        {
            return new PendingStudentSubGroup
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader.GetInt32("StudentId"),
                GroupId = reader.GetInt32("GroupId"),
                SubGroupId = reader.GetInt32("SubGroupId")
            };
        }

        #endregion
    }
}


