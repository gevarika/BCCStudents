using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services.Sync.UpStream
{
    /// <summary>
    /// SyncOutbox რეპოზიტორი (ჩანაწერების ლოკალური დამატება, შეცდომების დაფიქსირება და ა.შ.).
    /// </summary>
    public class UpStreamSyncRepository : IUpStreamSyncRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly ISyncLogger _logger;
        private readonly object _schemaLock = new object();
        private bool _schemaEnsured;

        public UpStreamSyncRepository(IDatabaseConnectionProvider connectionProvider, ISyncLogger logger)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// SyncOutbox-ში ახალი ჩანაწერის დამატება, რომელიც სინქრონიზაციისთვის მომზადებული ცხრილისთვის აღნიშნავს.
        /// </summary>
        public Task<long> EnqueueChangeAsync(SyncChangePayload payload, CancellationToken cancellationToken = default)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            cancellationToken.ThrowIfCancellationRequested();

            EnsureSchema();

            try
            {
                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();
                    var sql = @"INSERT INTO SyncOutbox (TableName, RecordId, RecordKey, Operation, PayloadJson, OccurredAt, Attempts, Status) 
                                VALUES (@TableName, @RecordId, @RecordKey, @Operation, @PayloadJson, @OccurredAt, 0, 0);
                                SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TableName", payload.TableName);
                        command.Parameters.AddWithValue("@RecordId", payload.RecordId.HasValue ? (object)payload.RecordId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@RecordKey", payload.RecordKey);
                        command.Parameters.AddWithValue("@Operation", payload.Operation.ToString().ToUpperInvariant());
                        command.Parameters.AddWithValue("@PayloadJson", payload.PayloadJson);
                        command.Parameters.AddWithValue("@OccurredAt", payload.CreatedAt);

                        var result = command.ExecuteScalar();
                        var id = Convert.ToInt64(result);
                        _logger.Info($"SyncOutbox → დაემატა ჩანაწერი #{id} ({payload.TableName}/{payload.Operation}/{payload.RecordKey}).");
                        return Task.FromResult(id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"SyncOutbox-ში ჩანაწერის დამატება ვერ მოხდა ({payload.TableName}/{payload.RecordKey}).", ex);
                throw;
            }
        }

        /// <summary>
        /// აბრუნებს Pending (Status = 0) ჩანაწერებს სინქრონიზაციისთვის დასამუშავებლად.
        /// </summary>
        public async Task<IReadOnlyList<SyncOutboxItem>> GetPendingItemsAsync(int limit = 50, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureSchema();

            var items = new List<SyncOutboxItem>();

            try
            {
                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    var sql = @"SELECT Id, TableName, RecordId, RecordKey, Operation, PayloadJson, OccurredAt, Attempts, LastError
                                FROM SyncOutbox
                                WHERE Status = 0 
                                   OR (Status = 2 AND (LastRetryTime IS NULL OR LastRetryTime < DATE_SUB(NOW(), INTERVAL 2 MINUTE)))
                                ORDER BY OccurredAt ASC
                                LIMIT @Limit;";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Limit", limit);
                        using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                items.Add(new SyncOutboxItem
                                {
                                    Id = Convert.ToInt64(reader["Id"]),
                                    TableName = reader["TableName"]?.ToString(),
                                    RecordId = reader["RecordId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["RecordId"]),
                                    RecordKey = reader["RecordKey"] == DBNull.Value ? null : reader["RecordKey"].ToString(),
                                    Operation = Enum.TryParse(reader["Operation"]?.ToString(), true, out SyncOperationType op) ? op : SyncOperationType.Update,
                                    PayloadJson = reader["PayloadJson"] == DBNull.Value ? null : reader["PayloadJson"].ToString(),
                                    OccurredAt = Convert.ToDateTime(reader["OccurredAt"]),
                                    Attempts = reader["Attempts"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Attempts"]),
                                    LastError = reader["LastError"] == DBNull.Value ? null : reader["LastError"].ToString(),
                                    //LastRetryTime = reader["LastRetryTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastRetryTime"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SyncOutbox pending items query failed.", ex);
                throw;
            }

            return items;
        }

        /// <summary>
        /// წარმატებით გაგზავნილი ჩანაწერის წაშლა SyncOutbox-დან.
        /// </summary>
        public async Task MarkAsSuccessAsync(long outboxId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureSchema();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var sql = @"DELETE FROM SyncOutbox WHERE Id = @Id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", outboxId);
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// წარუმატებლობის მონიშვნა. გაზრდის Attempts-ს და იმ შემთხვევაში აღნიშნავს შეცდომას.
        /// </summary>
        public async Task MarkAsFailedAsync(long outboxId, string errorMessage, bool giveUp, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureSchema();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var sql = @"UPDATE SyncOutbox
                            SET Attempts = Attempts + 1,
                                LastError = @Error,
                                Status = @Status,
                                LastRetryTime = CASE WHEN @GiveUp = 1 THEN NOW() ELSE LastRetryTime END
                            WHERE Id = @Id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Error", (object)errorMessage ?? DBNull.Value);
                    // თუ giveUp = true, Status = 0 დავტოვოთ (არა 2), რომ retry განაგრძოს 2 წუთიანი interval-ით
                    command.Parameters.AddWithValue("@Status", 0); // ყოველთვის 0, რადგან Status = 2 ჩანაწერებიც retry-ს ექვემდებარება
                    command.Parameters.AddWithValue("@GiveUp", giveUp ? 1 : 0);
                    command.Parameters.AddWithValue("@Id", outboxId);
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private void EnsureSchema()
        {
            if (_schemaEnsured) return;

            lock (_schemaLock)
            {
                if (_schemaEnsured) return;

                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();

                    var outboxSql = @"CREATE TABLE IF NOT EXISTS SyncOutbox (
                        Id BIGINT AUTO_INCREMENT PRIMARY KEY,
                        TableName VARCHAR(64) NOT NULL,
                        RecordId INT NULL,
                        RecordKey VARCHAR(128) NULL,
                        Operation VARCHAR(16) NOT NULL,
                        PayloadJson MEDIUMTEXT NULL,
                        OccurredAt DATETIME NOT NULL,
                        Attempts INT NOT NULL DEFAULT 0,
                        LastError TEXT NULL,
                        Status TINYINT NOT NULL DEFAULT 0,
                        LastRetryTime DATETIME NULL,
                        INDEX idx_outbox_status (Status, OccurredAt),
                        INDEX idx_outbox_table (TableName, OccurredAt),
                        INDEX idx_outbox_retry (Status, LastRetryTime)
                    );";

                    using (var command = new MySqlCommand(outboxSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // თუ ადრე არ არსებობდა SyncOutbox ცხრილში RecordKey-ის სვეტი – დაემატა
                    try
                    {
                        // შემოწმება, არსებობს თუ არა RecordKey სვეტი
                        var checkColumnSql = @"
                            SELECT COUNT(*) 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_SCHEMA = DATABASE() 
                            AND TABLE_NAME = 'SyncOutbox' 
                            AND COLUMN_NAME = 'RecordKey';";

                        using (var checkCmd = new MySqlCommand(checkColumnSql, connection))
                        {
                            var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                            if (!exists)
                            {
                                const string alterSql = @"
                                    ALTER TABLE SyncOutbox
                                    ADD COLUMN RecordKey VARCHAR(128) NULL AFTER RecordId;";
                                using (var alter = new MySqlCommand(alterSql, connection))
                                {
                                    alter.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // თუ შეცდომა მოხდა, ჩავწეროთ (სვეტი შეიძლება უკვე არსებობდეს)
                        _logger.Error("RecordKey სვეტის შექმნა/დამატება ვერ მოხდა", ex);
                    }

                    // LastRetryTime სვეტის შემოწმება და დამატება
                    try
                    {
                        var checkLastRetryTimeSql = @"
                            SELECT COUNT(*) 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_SCHEMA = DATABASE() 
                            AND TABLE_NAME = 'SyncOutbox' 
                            AND COLUMN_NAME = 'LastRetryTime';";

                        using (var checkCmd = new MySqlCommand(checkLastRetryTimeSql, connection))
                        {
                            var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                            if (!exists)
                            {
                                const string alterSql = @"
                                    ALTER TABLE SyncOutbox
                                    ADD COLUMN LastRetryTime DATETIME NULL AFTER Status;";
                                using (var alter = new MySqlCommand(alterSql, connection))
                                {
                                    alter.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("LastRetryTime სვეტის შექმნა/დამატება ვერ მოხდა", ex);
                    }
                }

                _schemaEnsured = true;
            }
        }
    }
}
