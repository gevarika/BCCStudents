using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class DownStreamSyncRepository : IDownStreamSyncRepository
    {
        private readonly DatabaseHelper _databaseHelper;
        private readonly ISyncLogger _logger;
        private readonly object _schemaLock = new object();
        private bool _schemaEnsured;

        public DownStreamSyncRepository(DatabaseHelper databaseHelper, ISyncLogger logger)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<SyncStateRecord> GetSyncStateAsync(string tableName, CancellationToken cancellationToken = default)
        {
            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                connection.Open();
                const string sql = @"SELECT TableName, LastSyncedAt, LastSyncedId
                                     FROM SyncState
                                     WHERE TableName = @TableName;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Task.FromResult(new SyncStateRecord
                            {
                                TableName = reader.GetString("TableName"),
                                LastSyncedAt = reader["LastSyncedAt"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime("LastSyncedAt"),
                                LastSyncedId = reader["LastSyncedId"] == DBNull.Value ? 0 : reader.GetInt32("LastSyncedId")
                            });
                        }
                    }
                }
            }

            return Task.FromResult<SyncStateRecord>(null);
        }

        public Task UpdateSyncStateAsync(string tableName, DateTime lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                connection.Open();
                const string sql = @"INSERT INTO SyncState (TableName, LastSyncedAt, LastSyncedId)
                                     VALUES (@TableName, @LastSyncedAt, @LastSyncedId)
                                     ON DUPLICATE KEY UPDATE
                                        LastSyncedAt = VALUES(LastSyncedAt),
                                        LastSyncedId = VALUES(LastSyncedId);";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.Parameters.AddWithValue("@LastSyncedAt", lastSyncedAt);
                    command.Parameters.AddWithValue("@LastSyncedId", lastSyncedId);
                    command.ExecuteNonQuery();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertStudentsAsync(IReadOnlyList<Student> students, CancellationToken cancellationToken = default)
        {
            if (students == null || students.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO Students
                                (Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                                 RegistrationDate, StudentCode, Info, user_id, Balance, UpdatedAt)
                                VALUES
                                (@Id, @FirstName, @LastName, @Age, @ParentName, @PhoneNumber, @Id_Numb, @Address,
                                 @RegistrationDate, @StudentCode, @Info, @UserId, @Balance, @UpdatedAt)
                                ON DUPLICATE KEY UPDATE
                                 FirstName = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(FirstName), FirstName),
                                 LastName = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(LastName), LastName),
                                 Age = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Age), Age),
                                 ParentName = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(ParentName), ParentName),
                                 PhoneNumber = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(PhoneNumber), PhoneNumber),
                                 Id_Numb = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Id_Numb), Id_Numb),
                                 Address = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Address), Address),
                                 RegistrationDate = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(RegistrationDate), RegistrationDate),
                                 StudentCode = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(StudentCode), StudentCode),
                                 Info = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Info), Info),
                                 user_id = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(user_id), user_id),
                                 Balance = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Balance), Balance),
                                 UpdatedAt = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(UpdatedAt), UpdatedAt);";

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareStudentParameters(command);
                        foreach (var student in students)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillStudentParameters(command, student);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertGroupsAsync(IReadOnlyList<Group> groups, CancellationToken cancellationToken = default)
        {
            if (groups == null || groups.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO `Groups`
                                (Id, Name, Price, Teacher, ContractTemplatePath, Status, StudentCount, UpdatedAt)
                                VALUES
                                (@Id, @Name, @Price, @Teacher, @ContractTemplatePath, @Status, @StudentCount, @UpdatedAt)
                                ON DUPLICATE KEY UPDATE
                                 Name = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Name), Name),
                                 Price = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Price), Price),
                                 Teacher = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Teacher), Teacher),
                                 ContractTemplatePath = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(ContractTemplatePath), ContractTemplatePath),
                                 Status = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Status), Status),
                                 StudentCount = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(StudentCount), StudentCount),
                                 UpdatedAt = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(UpdatedAt), UpdatedAt);";

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareGroupParameters(command);
                        foreach (var group in groups)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillGroupParameters(command, group);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertSubGroupsAsync(IReadOnlyList<SubGroup> subGroups, CancellationToken cancellationToken = default)
        {
            if (subGroups == null || subGroups.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO SubGroups
                                (Id, Name, GroupId, TuitionFee, StudentCount, Status, UpdatedAt)
                                VALUES
                                (@Id, @Name, @GroupId, @TuitionFee, @StudentCount, @Status, @UpdatedAt)
                                ON DUPLICATE KEY UPDATE
                                 Name = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Name), Name),
                                 GroupId = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(GroupId), GroupId),
                                 TuitionFee = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(TuitionFee), TuitionFee),
                                 StudentCount = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(StudentCount), StudentCount),
                                 Status = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Status), Status),
                                 UpdatedAt = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(UpdatedAt), UpdatedAt);";

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareSubGroupParameters(command);
                        foreach (var subGroup in subGroups)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillSubGroupParameters(command, subGroup);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertStudentGroupsAsync(IReadOnlyList<StudentGroups> items, CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO StudentGroups
                                (ID, StudentId, GroupId, PaymentStatus, DateOfPayment, Price, Discount, Status, UpdatedAt, IsDeleted)
                                VALUES
                                (@Id, @StudentId, @GroupId, @PaymentStatus, @DateOfPayment, @Price, @Discount, @Status, @UpdatedAt, 0)
                                ON DUPLICATE KEY UPDATE
                                 PaymentStatus = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(PaymentStatus), PaymentStatus),
                                 DateOfPayment = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(DateOfPayment), DateOfPayment),
                                 Price = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Price), Price),
                                 Discount = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Discount), Discount),
                                 Status = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Status), Status),
                                 UpdatedAt = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(UpdatedAt), UpdatedAt),
                                 IsDeleted = 0;";

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareStudentGroupParameters(command);
                        foreach (var item in items)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillStudentGroupParameters(command, item);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertStudentSubGroupsAsync(IReadOnlyList<StudentSubGroups> items, CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO StudentSubGroups
                                (Id, StudentId, GroupId, SubGroupId, PaymentStatus, DateOfPayment, Price, Discount, Status, UpdatedAt, IsDeleted)
                                VALUES
                                (@Id, @StudentId, @GroupId, @SubGroupId, @PaymentStatus, @DateOfPayment, @Price, @Discount, @Status, @UpdatedAt, 0)
                                ON DUPLICATE KEY UPDATE
                                 PaymentStatus = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(PaymentStatus), PaymentStatus),
                                 DateOfPayment = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(DateOfPayment), DateOfPayment),
                                 Price = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Price), Price),
                                 Discount = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Discount), Discount),
                                 Status = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(Status), Status),
                                 UpdatedAt = IF(VALUES(UpdatedAt) > UpdatedAt, VALUES(UpdatedAt), UpdatedAt),
                                 IsDeleted = 0;";

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareStudentSubGroupParameters(command);
                        foreach (var item in items)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillStudentSubGroupParameters(command, item);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        #region Parameter helpers

        private static void PrepareStudentParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@FirstName", MySqlDbType.VarChar);
            command.Parameters.Add("@LastName", MySqlDbType.VarChar);
            command.Parameters.Add("@Age", MySqlDbType.Int32);
            command.Parameters.Add("@ParentName", MySqlDbType.VarChar);
            command.Parameters.Add("@PhoneNumber", MySqlDbType.VarChar);
            command.Parameters.Add("@Id_Numb", MySqlDbType.Int64);
            command.Parameters.Add("@Address", MySqlDbType.VarChar);
            command.Parameters.Add("@RegistrationDate", MySqlDbType.DateTime);
            command.Parameters.Add("@StudentCode", MySqlDbType.VarChar);
            command.Parameters.Add("@Info", MySqlDbType.VarChar);
            command.Parameters.Add("@UserId", MySqlDbType.Int32);
            command.Parameters.Add("@Balance", MySqlDbType.Decimal);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
        }

        private static void FillStudentParameters(MySqlCommand command, Student student)
        {
            command.Parameters["@Id"].Value = student.Id;
            command.Parameters["@FirstName"].Value = student.FirstName ?? string.Empty;
            command.Parameters["@LastName"].Value = student.LastName ?? string.Empty;
            command.Parameters["@Age"].Value = student.Age;
            command.Parameters["@ParentName"].Value = student.ParentName ?? string.Empty;
            command.Parameters["@PhoneNumber"].Value = student.PhoneNumber ?? string.Empty;
            command.Parameters["@Id_Numb"].Value = student.Id_Numb;
            command.Parameters["@Address"].Value = student.Address ?? string.Empty;
            command.Parameters["@RegistrationDate"].Value = student.RegistrationDate;
            command.Parameters["@StudentCode"].Value = student.StudentCode ?? (object)DBNull.Value;
            command.Parameters["@Info"].Value = student.Info ?? (object)DBNull.Value;
            command.Parameters["@UserId"].Value = student.User_Id;
            command.Parameters["@Balance"].Value = student.Balance;
            command.Parameters["@UpdatedAt"].Value = student.UpdatedAt;
        }

        private static void PrepareGroupParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@Name", MySqlDbType.VarChar);
            command.Parameters.Add("@Price", MySqlDbType.Decimal);
            command.Parameters.Add("@Teacher", MySqlDbType.VarChar);
            command.Parameters.Add("@ContractTemplatePath", MySqlDbType.VarChar);
            command.Parameters.Add("@Status", MySqlDbType.Bit);
            command.Parameters.Add("@StudentCount", MySqlDbType.Int32);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
        }

        private static void FillGroupParameters(MySqlCommand command, Group group)
        {
            command.Parameters["@Id"].Value = group.Id;
            command.Parameters["@Name"].Value = group.Name ?? string.Empty;
            command.Parameters["@Price"].Value = group.Price;
            command.Parameters["@Teacher"].Value = group.Teacher ?? (object)DBNull.Value;
            command.Parameters["@ContractTemplatePath"].Value = group.ContractTemplatePath ?? (object)DBNull.Value;
            command.Parameters["@Status"].Value = group.Status;
            command.Parameters["@StudentCount"].Value = group.StudentCount;
            command.Parameters["@UpdatedAt"].Value = group.UpdatedAt;
        }

        private static void PrepareSubGroupParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@Name", MySqlDbType.VarChar);
            command.Parameters.Add("@GroupId", MySqlDbType.Int32);
            command.Parameters.Add("@TuitionFee", MySqlDbType.Decimal);
            command.Parameters.Add("@StudentCount", MySqlDbType.Int32);
            command.Parameters.Add("@Status", MySqlDbType.Bit);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
        }

        private static void FillSubGroupParameters(MySqlCommand command, SubGroup subGroup)
        {
            command.Parameters["@Id"].Value = subGroup.Id;
            command.Parameters["@Name"].Value = subGroup.Name ?? string.Empty;
            command.Parameters["@GroupId"].Value = subGroup.GroupId;
            command.Parameters["@TuitionFee"].Value = subGroup.TuitionFee;
            command.Parameters["@StudentCount"].Value = subGroup.StudentCount;
            command.Parameters["@Status"].Value = subGroup.Status;
            //command.Parameters["@ActiveStatus"].Value = subGroup.ActiveStatus;
            command.Parameters["@UpdatedAt"].Value = subGroup.UpdatedAt;
        }

        private static void PrepareStudentGroupParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@StudentId", MySqlDbType.Int32);
            command.Parameters.Add("@GroupId", MySqlDbType.Int32);
            command.Parameters.Add("@PaymentStatus", MySqlDbType.VarChar);
            command.Parameters.Add("@DateOfPayment", MySqlDbType.DateTime);
            command.Parameters.Add("@Price", MySqlDbType.Decimal);
            command.Parameters.Add("@Discount", MySqlDbType.Double);
            command.Parameters.Add("@Status", MySqlDbType.Bit);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
        }

        private static void FillStudentGroupParameters(MySqlCommand command, StudentGroups item)
        {
            command.Parameters["@Id"].Value = item.Id;
            command.Parameters["@StudentId"].Value = item.StudentId;
            command.Parameters["@GroupId"].Value = item.GroupId;
            command.Parameters["@PaymentStatus"].Value = item.PaymentStatus ?? string.Empty;
            command.Parameters["@DateOfPayment"].Value = item.DateOfPayment ?? (object)DBNull.Value;
            command.Parameters["@Price"].Value = item.Price;
            command.Parameters["@Discount"].Value = item.Discount;
            command.Parameters["@Status"].Value = item.Status;
            command.Parameters["@UpdatedAt"].Value = item.UpdatedAt;
        }

        private static void PrepareStudentSubGroupParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@StudentId", MySqlDbType.Int32);
            command.Parameters.Add("@GroupId", MySqlDbType.Int32);
            command.Parameters.Add("@SubGroupId", MySqlDbType.Int32);
            command.Parameters.Add("@PaymentStatus", MySqlDbType.VarChar);
            command.Parameters.Add("@DateOfPayment", MySqlDbType.DateTime);
            command.Parameters.Add("@Price", MySqlDbType.Decimal);
            command.Parameters.Add("@Discount", MySqlDbType.Double);
            command.Parameters.Add("@Status", MySqlDbType.Bit);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
        }

        private static void FillStudentSubGroupParameters(MySqlCommand command, StudentSubGroups item)
        {
            command.Parameters["@Id"].Value = item.Id;
            command.Parameters["@StudentId"].Value = item.StudentId;
            command.Parameters["@GroupId"].Value = item.GroupId;
            command.Parameters["@SubGroupId"].Value = item.SubGroupId;
            command.Parameters["@PaymentStatus"].Value = item.PaymentStatus ?? (object)DBNull.Value;
            command.Parameters["@DateOfPayment"].Value = item.DateOfPayment ?? (object)DBNull.Value;
            command.Parameters["@Price"].Value = item.Price;
            command.Parameters["@Discount"].Value = item.Discount;
            command.Parameters["@Status"].Value = item.Status;
            command.Parameters["@UpdatedAt"].Value = item.UpdatedAt;
        }

        #endregion

        private void EnsureSchema()
        {
            if (_schemaEnsured) return;

            lock (_schemaLock)
            {
                if (_schemaEnsured) return;

                using (var connection = _databaseHelper.GetLocalConnection())
                {
                    connection.Open();
                    const string sql = @"CREATE TABLE IF NOT EXISTS SyncState (
                                            TableName VARCHAR(64) PRIMARY KEY,
                                            LastSyncedAt DATETIME NULL,
                                            LastSyncedId INT NULL
                                         );";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                _schemaEnsured = true;
            }
        }
    }
}



