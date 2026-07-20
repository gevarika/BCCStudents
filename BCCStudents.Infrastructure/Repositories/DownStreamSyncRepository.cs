using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class DownStreamSyncRepository : IDownStreamSyncRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly ISyncLogger _logger;
        private readonly IApplicationLogRepository _applicationLogRepository;
        private readonly object _schemaLock = new object();
        private bool _schemaEnsured;

        public DownStreamSyncRepository(
            IDatabaseConnectionProvider connectionProvider,
            ISyncLogger logger,
            IApplicationLogRepository applicationLogRepository)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationLogRepository = applicationLogRepository ?? throw new ArgumentNullException(nameof(applicationLogRepository));
        }

        public Task<int> UpsertApplicationLogsAsync(IReadOnlyList<ApplicationLogEntry> logs, CancellationToken cancellationToken = default)
        {
            return _applicationLogRepository.InsertFromServerAsync(logs, cancellationToken);
        }

        public Task<SyncStateRecord> GetSyncStateAsync(string tableName, CancellationToken cancellationToken = default)
        {
            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _connectionProvider.GetLocalConnection())
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

            using (var connection = _connectionProvider.GetLocalConnection())
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
                    command.Parameters.Add("@LastSyncedAt", MySqlDbType.DateTime).Value = lastSyncedAt;
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

            using (var connection = _connectionProvider.GetLocalConnection())
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

            using (var connection = _connectionProvider.GetLocalConnection())
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

            using (var connection = _connectionProvider.GetLocalConnection())
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

            using (var connection = _connectionProvider.GetLocalConnection())
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

            using (var connection = _connectionProvider.GetLocalConnection())
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

        public Task UpsertPaymentsAsync(IReadOnlyList<Payment> payments, CancellationToken cancellationToken = default)
        {
            if (payments == null || payments.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO Payments
                                (Id, StudentId, GroupId, Amount, PaymentDate, PaymentStatus, Description, PayerName, PersonalId, UpdatedAt, IsDeleted)
                                VALUES
                                (@Id, @StudentId, @GroupId, @Amount, @PaymentDate, @PaymentStatus, @Description, @PayerName, @PersonalId, @UpdatedAt, @IsDeleted)
                                ON DUPLICATE KEY UPDATE
                                 StudentId = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(StudentId), StudentId),
                                 GroupId = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(GroupId), GroupId),
                                 Amount = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(Amount), Amount),
                                 PaymentDate = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(PaymentDate), PaymentDate),
                                 PaymentStatus = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(PaymentStatus), PaymentStatus),
                                 Description = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(Description), Description),
                                 PayerName = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(PayerName), PayerName),
                                 PersonalId = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(PersonalId), PersonalId),
                                 UpdatedAt = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(UpdatedAt), UpdatedAt),
                                 IsDeleted = IF(COALESCE(VALUES(UpdatedAt), VALUES(PaymentDate)) > COALESCE(UpdatedAt, PaymentDate), VALUES(IsDeleted), IsDeleted);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PreparePaymentParameters(command);
                        foreach (var payment in payments)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillPaymentParameters(command, payment);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertFailedPaymentsAsync(IReadOnlyList<FailedPayment> payments, CancellationToken cancellationToken = default)
        {
            if (payments == null || payments.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO FailedPayments
                                (Id, RowNumber, PaymentDate, Amount, PersonalId, Description, Reason, CreatedAt)
                                VALUES
                                (@Id, @RowNumber, @PaymentDate, @Amount, @PersonalId, @Description, @Reason, @CreatedAt)
                                ON DUPLICATE KEY UPDATE
                                 RowNumber = VALUES(RowNumber),
                                 PaymentDate = VALUES(PaymentDate),
                                 Amount = VALUES(Amount),
                                 PersonalId = VALUES(PersonalId),
                                 Description = VALUES(Description),
                                 Reason = VALUES(Reason),
                                 CreatedAt = VALUES(CreatedAt);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareFailedPaymentParameters(command);
                        foreach (var payment in payments)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillFailedPaymentParameters(command, payment);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertImportedPaymentLogsAsync(IReadOnlyList<ImportedPaymentLog> logs, CancellationToken cancellationToken = default)
        {
            if (logs == null || logs.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO ImportedPaymentsLog
                                (Id, PaymentDate, Amount, PersonalId, Description, ImportSource, CreatedAt)
                                VALUES
                                (@Id, @PaymentDate, @Amount, @PersonalId, @Description, @ImportSource, @CreatedAt)
                                ON DUPLICATE KEY UPDATE
                                 PaymentDate = VALUES(PaymentDate),
                                 Amount = VALUES(Amount),
                                 PersonalId = VALUES(PersonalId),
                                 Description = VALUES(Description),
                                 ImportSource = VALUES(ImportSource),
                                 CreatedAt = VALUES(CreatedAt);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareImportedPaymentLogParameters(command);
                        foreach (var item in logs)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillImportedPaymentLogParameters(command, item);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertUsersAsync(IReadOnlyList<UserModel> users, CancellationToken cancellationToken = default)
        {
            if (users == null || users.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO Users
                                (Id, Username, FullName, Email, Password, Role, CreatedAt, LastLogin, UpdatedAt)
                                VALUES
                                (@Id, @Username, @FullName, @Email, @Password, @Role, @CreatedAt, @LastLogin, @UpdatedAt)
                                ON DUPLICATE KEY UPDATE
                                 Username = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(Username), Username),
                                 FullName = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(FullName), FullName),
                                 Email = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(Email), Email),
                                 Password = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(Password), Password),
                                 Role = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(Role), Role),
                                 CreatedAt = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(CreatedAt), CreatedAt),
                                 LastLogin = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(LastLogin), LastLogin),
                                 UpdatedAt = IF(VALUES(UpdatedAt) > UpdatedAt OR UpdatedAt IS NULL, VALUES(UpdatedAt), UpdatedAt);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareUserParameters(command);
                        foreach (var user in users)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillUserParameters(command, user);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertSystemConfigAsync(IReadOnlyList<SystemConfiguration> configs, CancellationToken cancellationToken = default)
        {
            if (configs == null || configs.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO SystemConfig
                                (Id, `Key`, `Value`, `Type`, `Description`, CreatedAt, UpdatedAt)
                                VALUES
                                (@Id, @Key, @Value, @Type, @Description, @CreatedAt, @UpdatedAt)
                                ON DUPLICATE KEY UPDATE
                                 `Key` = VALUES(`Key`),
                                 `Value` = VALUES(`Value`),
                                 `Type` = VALUES(`Type`),
                                 `Description` = VALUES(`Description`),
                                 CreatedAt = VALUES(CreatedAt),
                                 UpdatedAt = VALUES(UpdatedAt);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PrepareSystemConfigParameters(command);
                        foreach (var item in configs)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillSystemConfigParameters(command, item);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertPendingStudentsAsync(IReadOnlyList<PendingStudent> students, CancellationToken cancellationToken = default)
        {
            if (students == null || students.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO PendingStudents
                                (Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                                  IdCardPath, AdditionalDocsPath, user_id, CreatedAt)
                                VALUES
                                (@Id, @FirstName, @LastName, @Age, @ParentName, @PhoneNumber, @IdNumb, @Address,
                                  @IdCardPath, @AdditionalDocsPath, @UserId, @CreatedAt)
                                ON DUPLICATE KEY UPDATE
                                 FirstName = VALUES(FirstName),
                                 LastName = VALUES(LastName),
                                 Age = VALUES(Age),
                                 ParentName = VALUES(ParentName),
                                 PhoneNumber = VALUES(PhoneNumber),
                                 Id_Numb = VALUES(Id_Numb),
                                 Address = VALUES(Address),
                                 IdCardPath = VALUES(IdCardPath),
                                 AdditionalDocsPath = VALUES(AdditionalDocsPath),
                                 user_id = VALUES(user_id),
                                 CreatedAt = VALUES(CreatedAt);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PreparePendingStudentParameters(command);
                        foreach (var item in students)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillPendingStudentParameters(command, item);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertPendingStudentGroupsAsync(IReadOnlyList<PendingStudentGroup> groups, CancellationToken cancellationToken = default)
        {
            if (groups == null || groups.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO PendingStudentGroups
                                (Id, StudentId, GroupId)
                                VALUES
                                (@Id, @StudentId, @GroupId)
                                ON DUPLICATE KEY UPDATE
                                 StudentId = VALUES(StudentId),
                                 GroupId = VALUES(GroupId);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PreparePendingStudentGroupParameters(command);
                        foreach (var item in groups)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillPendingStudentGroupParameters(command, item);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        public Task UpsertPendingStudentSubGroupsAsync(IReadOnlyList<PendingStudentSubGroup> subGroups, CancellationToken cancellationToken = default)
        {
            if (subGroups == null || subGroups.Count == 0)
            {
                return Task.CompletedTask;
            }

            EnsureSchema();
            cancellationToken.ThrowIfCancellationRequested();

            const string sql = @"INSERT INTO PendingStudentSubGroups
                                (Id, StudentId, GroupId, SubGroupId)
                                VALUES
                                (@Id, @StudentId, @GroupId, @SubGroupId)
                                ON DUPLICATE KEY UPDATE
                                 StudentId = VALUES(StudentId),
                                 GroupId = VALUES(GroupId),
                                 SubGroupId = VALUES(SubGroupId);";

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new MySqlCommand(sql, connection, transaction))
                    {
                        PreparePendingStudentSubGroupParameters(command);
                        foreach (var item in subGroups)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            FillPendingStudentSubGroupParameters(command, item);
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

        private static void PrepareUserParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@Username", MySqlDbType.VarChar);
            command.Parameters.Add("@FullName", MySqlDbType.VarChar);
            command.Parameters.Add("@Email", MySqlDbType.VarChar);
            command.Parameters.Add("@Password", MySqlDbType.VarChar);
            command.Parameters.Add("@Role", MySqlDbType.VarChar);
            command.Parameters.Add("@CreatedAt", MySqlDbType.DateTime);
            command.Parameters.Add("@LastLogin", MySqlDbType.DateTime);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
        }

        private static void FillUserParameters(MySqlCommand command, UserModel user)
        {
            command.Parameters["@Id"].Value = user.Id;
            command.Parameters["@Username"].Value = user.UserName ?? (object)DBNull.Value;
            command.Parameters["@FullName"].Value = user.FullName ?? (object)DBNull.Value;
            command.Parameters["@Email"].Value = user.Email ?? (object)DBNull.Value;
            command.Parameters["@Password"].Value = user.Password ?? (object)DBNull.Value;
            command.Parameters["@Role"].Value = user.Role ?? (object)DBNull.Value;
            command.Parameters["@CreatedAt"].Value = user.CreatedAt ?? (object)DBNull.Value;
            command.Parameters["@LastLogin"].Value = user.LastLogin ?? (object)DBNull.Value;
            command.Parameters["@UpdatedAt"].Value = user.UpdatedAt;
        }

        private static void PrepareSystemConfigParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@Key", MySqlDbType.VarChar);
            command.Parameters.Add("@Value", MySqlDbType.Text);
            command.Parameters.Add("@Type", MySqlDbType.VarChar);
            command.Parameters.Add("@Description", MySqlDbType.VarChar);
            command.Parameters.Add("@CreatedAt", MySqlDbType.DateTime);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
        }

        private static void FillSystemConfigParameters(MySqlCommand command, SystemConfiguration config)
        {
            command.Parameters["@Id"].Value = config.Id;
            command.Parameters["@Key"].Value = config.Key ?? (object)DBNull.Value;
            command.Parameters["@Value"].Value = config.Value ?? (object)DBNull.Value;
            command.Parameters["@Type"].Value = config.Type ?? (object)DBNull.Value;
            command.Parameters["@Description"].Value = config.Description ?? (object)DBNull.Value;
            command.Parameters["@CreatedAt"].Value = config.CreatedAt;
            command.Parameters["@UpdatedAt"].Value = config.UpdatedAt ?? (object)DBNull.Value;
        }

        private static void PreparePendingStudentParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@FirstName", MySqlDbType.VarChar);
            command.Parameters.Add("@LastName", MySqlDbType.VarChar);
            command.Parameters.Add("@Age", MySqlDbType.Int32);
            command.Parameters.Add("@ParentName", MySqlDbType.VarChar);
            command.Parameters.Add("@PhoneNumber", MySqlDbType.VarChar);
            command.Parameters.Add("@IdNumb", MySqlDbType.Int64);
            command.Parameters.Add("@Address", MySqlDbType.VarChar);
            command.Parameters.Add("@IdCardPath", MySqlDbType.VarChar);
            command.Parameters.Add("@AdditionalDocsPath", MySqlDbType.VarChar);
            command.Parameters.Add("@UserId", MySqlDbType.Int32);
            command.Parameters.Add("@CreatedAt", MySqlDbType.DateTime);
        }

        private static void FillPendingStudentParameters(MySqlCommand command, PendingStudent student)
        {
            command.Parameters["@Id"].Value = student.Id;
            command.Parameters["@FirstName"].Value = student.FirstName ?? (object)DBNull.Value;
            command.Parameters["@LastName"].Value = student.LastName ?? (object)DBNull.Value;
            command.Parameters["@Age"].Value = student.Age;
            command.Parameters["@ParentName"].Value = student.ParentName ?? (object)DBNull.Value;
            command.Parameters["@PhoneNumber"].Value = student.PhoneNumber ?? (object)DBNull.Value;
            command.Parameters["@IdNumb"].Value = student.Id_Numb;
            command.Parameters["@Address"].Value = student.Address ?? (object)DBNull.Value;
            command.Parameters["@IdCardPath"].Value = student.IdCardPath ?? (object)DBNull.Value;
            command.Parameters["@AdditionalDocsPath"].Value = student.AdditionalDocsPath ?? (object)DBNull.Value;
            command.Parameters["@UserId"].Value = student.UserId;
            command.Parameters["@CreatedAt"].Value = student.CreatedAt;
        }

        private static void PreparePendingStudentGroupParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@StudentId", MySqlDbType.Int32);
            command.Parameters.Add("@GroupId", MySqlDbType.Int32);
        }

        private static void FillPendingStudentGroupParameters(MySqlCommand command, PendingStudentGroup item)
        {
            command.Parameters["@Id"].Value = item.Id;
            command.Parameters["@StudentId"].Value = item.StudentId;
            command.Parameters["@GroupId"].Value = item.GroupId;
        }

        private static void PreparePendingStudentSubGroupParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@StudentId", MySqlDbType.Int32);
            command.Parameters.Add("@GroupId", MySqlDbType.Int32);
            command.Parameters.Add("@SubGroupId", MySqlDbType.Int32);
        }

        private static void FillPendingStudentSubGroupParameters(MySqlCommand command, PendingStudentSubGroup item)
        {
            command.Parameters["@Id"].Value = item.Id;
            command.Parameters["@StudentId"].Value = item.StudentId;
            command.Parameters["@GroupId"].Value = item.GroupId;
            command.Parameters["@SubGroupId"].Value = item.SubGroupId;
        }

        private static void PreparePaymentParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@StudentId", MySqlDbType.Int32);
            command.Parameters.Add("@GroupId", MySqlDbType.Int32);
            command.Parameters.Add("@Amount", MySqlDbType.Decimal);
            command.Parameters.Add("@PaymentDate", MySqlDbType.DateTime);
            command.Parameters.Add("@PaymentStatus", MySqlDbType.VarChar);
            command.Parameters.Add("@Description", MySqlDbType.VarChar);
            command.Parameters.Add("@PayerName", MySqlDbType.VarChar);
            command.Parameters.Add("@PersonalId", MySqlDbType.Int64);
            command.Parameters.Add("@UpdatedAt", MySqlDbType.DateTime);
            command.Parameters.Add("@IsDeleted", MySqlDbType.Bit);
        }

        private static void FillPaymentParameters(MySqlCommand command, Payment payment)
        {
            command.Parameters["@Id"].Value = payment.Id;
            command.Parameters["@StudentId"].Value = payment.StudentId ?? (object)DBNull.Value;
            command.Parameters["@GroupId"].Value = payment.GroupId;
            command.Parameters["@Amount"].Value = payment.Amount;
            command.Parameters["@PaymentDate"].Value = payment.PaymentDate;
            command.Parameters["@PaymentStatus"].Value = payment.PaymentStatus ?? (object)DBNull.Value;
            command.Parameters["@Description"].Value = payment.Description ?? (object)DBNull.Value;
            command.Parameters["@PayerName"].Value = payment.PayerName ?? (object)DBNull.Value;
            command.Parameters["@PersonalId"].Value = payment.PersonalId ?? (object)DBNull.Value;
            command.Parameters["@UpdatedAt"].Value = payment.UpdatedAt ?? (object)DBNull.Value;
            command.Parameters["@IsDeleted"].Value = payment.IsDeleted;
        }

        private static void PrepareFailedPaymentParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@RowNumber", MySqlDbType.Int32);
            command.Parameters.Add("@PaymentDate", MySqlDbType.DateTime);
            command.Parameters.Add("@Amount", MySqlDbType.Decimal);
            command.Parameters.Add("@PersonalId", MySqlDbType.Int64);
            command.Parameters.Add("@Description", MySqlDbType.VarChar);
            command.Parameters.Add("@Reason", MySqlDbType.VarChar);
            command.Parameters.Add("@CreatedAt", MySqlDbType.DateTime);
        }

        private static void FillFailedPaymentParameters(MySqlCommand command, FailedPayment payment)
        {
            command.Parameters["@Id"].Value = payment.Id;
            command.Parameters["@RowNumber"].Value = payment.RowNumber;
            command.Parameters["@PaymentDate"].Value = payment.PaymentDate;
            command.Parameters["@Amount"].Value = payment.Amount;
            command.Parameters["@PersonalId"].Value = payment.PersonalId ?? (object)DBNull.Value;
            command.Parameters["@Description"].Value = payment.Description ?? (object)DBNull.Value;
            command.Parameters["@Reason"].Value = payment.Reason ?? (object)DBNull.Value;
            command.Parameters["@CreatedAt"].Value = payment.CreatedAt;
        }

        private static void PrepareImportedPaymentLogParameters(MySqlCommand command)
        {
            command.Parameters.Add("@Id", MySqlDbType.Int32);
            command.Parameters.Add("@PaymentDate", MySqlDbType.DateTime);
            command.Parameters.Add("@Amount", MySqlDbType.Decimal);
            command.Parameters.Add("@PersonalId", MySqlDbType.Int64);
            command.Parameters.Add("@Description", MySqlDbType.VarChar);
            command.Parameters.Add("@ImportSource", MySqlDbType.VarChar);
            command.Parameters.Add("@CreatedAt", MySqlDbType.DateTime);
        }

        private static void FillImportedPaymentLogParameters(MySqlCommand command, ImportedPaymentLog item)
        {
            command.Parameters["@Id"].Value = item.Id;
            command.Parameters["@PaymentDate"].Value = item.PaymentDate;
            command.Parameters["@Amount"].Value = item.Amount;
            command.Parameters["@PersonalId"].Value = item.PersonalId ?? (object)DBNull.Value;
            command.Parameters["@Description"].Value = item.Description ?? (object)DBNull.Value;
            command.Parameters["@ImportSource"].Value = item.ImportSource ?? (object)DBNull.Value;
            command.Parameters["@CreatedAt"].Value = item.CreatedAt;
        }

        #endregion

        private void EnsureSchema()
        {
            if (_schemaEnsured) return;

            lock (_schemaLock)
            {
                if (_schemaEnsured) return;

                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();
                    const string createSql = @"CREATE TABLE IF NOT EXISTS SyncState (
                                                   TableName VARCHAR(64) PRIMARY KEY,
                                                   LastSyncedAt DATETIME(3) NULL,
                                                   LastSyncedId INT NULL
                                                );";
                    using (var command = new MySqlCommand(createSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    const string migrateSql = @"ALTER TABLE SyncState
                                                MODIFY COLUMN LastSyncedAt DATETIME(3) NULL;";
                    using (var migrate = new MySqlCommand(migrateSql, connection))
                    {
                        migrate.ExecuteNonQuery();
                    }
                }

                _schemaEnsured = true;
            }
        }
    }
}



