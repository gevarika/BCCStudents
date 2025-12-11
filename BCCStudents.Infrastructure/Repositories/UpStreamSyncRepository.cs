using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Infrastructure.Data;
using MySql.Data.MySqlClient;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Infrastructure.Repositories.UpStream
{
    /// <summary>
    /// SyncOutbox áƒªáƒ®áƒ áƒ˜áƒšáƒ—áƒáƒœ áƒ›áƒ£áƒ¨áƒáƒáƒ‘áƒ (áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ áƒ˜áƒ’áƒ¨áƒ˜ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ, áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ˜áƒ¡ áƒšáƒáƒ’áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒ áƒ.áƒ¨.).
    /// </summary>
    public class UpStreamSyncRepository : IUpStreamSyncRepository
    {
        private readonly DatabaseHelper _databaseHelper;
        private readonly ISyncLogger _logger;
        private readonly object _schemaLock = new object();
        private bool _schemaEnsured;

        public UpStreamSyncRepository(DatabaseHelper databaseHelper, ISyncLogger logger)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// SyncOutbox-áƒ¨áƒ˜ áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ¡ áƒáƒ®áƒáƒš áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ¡ áƒ›áƒáƒ¨áƒ˜áƒœ, áƒ áƒáƒªáƒ áƒ›áƒáƒ›áƒ”áƒœáƒ¢áƒáƒšáƒ£áƒ áƒ˜ áƒ¡áƒ˜áƒœáƒ¥áƒ˜ áƒ•áƒ”áƒ  áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ“áƒ.
        /// </summary>
        public Task<long> EnqueueChangeAsync(SyncChangePayload payload, CancellationToken cancellationToken = default)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            cancellationToken.ThrowIfCancellationRequested();

            EnsureSchema();

            try
            {
                using (var connection = _databaseHelper.GetLocalConnection())
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
                        command.Parameters.AddWithValue("@OccurredAt", payload.CreatedAtUtc);

                        var result = command.ExecuteScalar();
                        var id = Convert.ToInt64(result);
                        _logger.Info($"SyncOutbox â†’ áƒ“áƒáƒ›áƒáƒ¢áƒ“áƒ áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜ #{id} ({payload.TableName}/{payload.Operation}/{payload.RecordKey}).");
                        return Task.FromResult(id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"SyncOutbox-áƒ¨áƒ˜ áƒ©áƒáƒ¬áƒ”áƒ áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ ({payload.TableName}/{payload.RecordKey}).", ex);
                throw;
            }
        }

        /// <summary>
        /// áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ¡ Pending (Status = 0) áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ”áƒ‘áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡ áƒ“áƒ áƒáƒ˜áƒ¡ áƒ›áƒ˜áƒ®áƒ”áƒ“áƒ•áƒ˜áƒ—.
        /// </summary>
        public async Task<IReadOnlyList<SyncOutboxItem>> GetPendingItemsAsync(int limit = 50, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureSchema();

            var items = new List<SyncOutboxItem>();

            try
            {
                using (var connection = _databaseHelper.GetLocalConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    var sql = @"SELECT Id, TableName, RecordId, RecordKey, Operation, PayloadJson, OccurredAt, Attempts, LastError
                                FROM SyncOutbox
                                WHERE Status = 0
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
                                    LastError = reader["LastError"] == DBNull.Value ? null : reader["LastError"].ToString()
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
        /// áƒ›áƒáƒœáƒ˜áƒ¨áƒ•áƒœáƒ˜áƒ¡áƒáƒ¡ Status = 1, áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜ áƒáƒ¦áƒáƒ áƒáƒ áƒ›áƒáƒ¡áƒ˜áƒ’áƒœáƒáƒšáƒ˜ retry-áƒ–áƒ”.
        /// </summary>
        public async Task MarkAsSuccessAsync(long outboxId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureSchema();

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var sql = @"UPDATE SyncOutbox
                            SET Status = 1, LastError = NULL
                            WHERE Id = @Id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", outboxId);
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// áƒ¬áƒáƒ áƒ£áƒ›áƒáƒ¢áƒ”áƒ‘áƒ”áƒšáƒ˜ áƒ›áƒªáƒ“áƒ”áƒšáƒáƒ‘áƒ˜áƒ¡ áƒœáƒ˜áƒ¨áƒœáƒáƒ“ Attempts áƒ˜áƒ–áƒ áƒ“áƒ”áƒ‘áƒ áƒ“áƒ áƒ˜áƒœáƒáƒ®áƒ”áƒ‘áƒ áƒ‘áƒáƒšáƒ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ.
        /// </summary>
        public async Task MarkAsFailedAsync(long outboxId, string errorMessage, bool giveUp, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureSchema();

            using (var connection = _databaseHelper.GetLocalConnection())
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                var sql = @"UPDATE SyncOutbox
                            SET Attempts = Attempts + 1,
                                LastError = @Error,
                                Status = @Status
                            WHERE Id = @Id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Error", (object)errorMessage ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", giveUp ? 2 : 0);
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

                using (var connection = _databaseHelper.GetLocalConnection())
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
                        INDEX idx_outbox_status (Status, OccurredAt),
                        INDEX idx_outbox_table (TableName, OccurredAt)
                    );";

                    using (var command = new MySqlCommand(outboxSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // áƒ—áƒ£ áƒ«áƒ•áƒ”áƒšáƒ˜ áƒ•áƒ”áƒ áƒ¡áƒ˜áƒ˜áƒ¡ SyncOutbox áƒªáƒ®áƒ áƒ˜áƒšáƒ˜ áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡ RecordKey-áƒ˜áƒ¡ áƒ’áƒáƒ áƒ”áƒ¨áƒ” â€“ áƒ“áƒáƒ•áƒáƒ›áƒáƒ¢áƒáƒ— áƒ¡áƒ•áƒ”áƒ¢áƒ˜
                    try
                    {
                        // áƒ¨áƒ”áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒáƒ—, áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡ áƒ—áƒ£ áƒáƒ áƒ RecordKey áƒ¡áƒ•áƒ”áƒ¢áƒ˜
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
                        // áƒ—áƒ£ áƒ áƒáƒ›áƒ” áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ›áƒáƒ®áƒ“áƒ, áƒ•áƒ£áƒ’áƒ£áƒšáƒ”áƒ‘áƒ”áƒšáƒ§áƒáƒ¤áƒ— (áƒ¡áƒ•áƒ”áƒ¢áƒ˜ áƒ¨áƒ”áƒ˜áƒ«áƒšáƒ”áƒ‘áƒ áƒ£áƒ™áƒ•áƒ” áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ“áƒ”áƒ¡)
                        _logger.Error("RecordKey áƒ¡áƒ•áƒ”áƒ¢áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ/áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ", ex);
                    }
                }

                _schemaEnsured = true;
            }
        }
    }
}



