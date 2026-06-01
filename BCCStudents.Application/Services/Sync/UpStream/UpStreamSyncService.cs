using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Text;

namespace BCCStudents.Application.Services.Sync.UpStream
{
    /// <summary>
    /// სერვერზე პოსტების შეცვლა (INSERT/UPDATE/DELETE) – Students/Groups/SubGroups და სხვა ცხრილები.
    /// </summary>
    public class UpStreamSyncService : IUpStreamSyncService
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly ISyncLogger _logger;

        public UpStreamSyncService(IDatabaseConnectionProvider connectionProvider, ISyncLogger logger)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<UpStreamSyncResult> TrySyncImmediatelyAsync(SyncChangePayload payload, CancellationToken cancellationToken = default)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                switch (payload.Operation)
                {
                    case SyncOperationType.Insert:
                    case SyncOperationType.Update:
                        ExecuteUpsert(payload);
                        break;
                    case SyncOperationType.Delete:
                        ExecuteDelete(payload);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported sync operation: {payload.Operation}");
                }

                _logger.Info($"UpStream (Immediate) წარმატებით ატვირთული: {payload.TableName}/{payload.Operation}/{payload.RecordKey}");
                return Task.FromResult(UpStreamSyncResult.Ok());
            }
            catch (Exception ex)
            {
                _logger.Error($"UpStream (Immediate) შეცდომა ატვირთვის: {payload.TableName}/{payload.Operation}/{payload.RecordKey}", ex);
                return Task.FromResult(UpStreamSyncResult.Fail(ex.Message));
            }
        }

        private void ExecuteUpsert(SyncChangePayload payload)
        {
            var columns = payload.Data.Keys.ToList();
            if (!columns.Any())
            {
                throw new InvalidOperationException("Upsert ოპერაციისთვის არ აქვს სვეტები.");
            }

            var columnList = string.Join(", ", columns.Select(EscapeColumn));
            var parameterList = string.Join(", ", columns.Select(c => $"@{c}"));
            var updateList = string.Join(", ", columns.Select(c => $"{EscapeColumn(c)} = VALUES({EscapeColumn(c)})"));
            var sql = new StringBuilder();
            sql.Append($"INSERT INTO {EscapeTable(payload.TableName)} ({columnList}) VALUES ({parameterList}) ");
            sql.Append($"ON DUPLICATE KEY UPDATE {updateList};");

            using (var connection = _connectionProvider.GetServerConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(sql.ToString(), connection))
                {
                    foreach (var column in columns)
                    {
                        var value = payload.Data[column];
                        command.Parameters.AddWithValue($"@{column}", value ?? DBNull.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteDelete(SyncChangePayload payload)
        {
            using (var connection = _connectionProvider.GetServerConnection())
            {
                connection.Open();

                if (payload.RecordId.HasValue)
                {
                    var sql = $"DELETE FROM {EscapeTable(payload.TableName)} WHERE Id = @Id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", payload.RecordId.Value);
                        command.ExecuteNonQuery();
                    }
                    return;
                }

                if (payload.KeyColumns.Count == 0)
                {
                    throw new InvalidOperationException("Delete ოპერაციისთვის არ აქვს RecordId ან KeyColumns.");
                }

                var conditions = new List<string>();
                foreach (var kv in payload.KeyColumns)
                {
                    conditions.Add($"{EscapeColumn(kv.Key)} = @{kv.Key}");
                }
                var whereClause = string.Join(" AND ", conditions);
                var deleteSql = $"DELETE FROM {EscapeTable(payload.TableName)} WHERE {whereClause}";

                using (var command = new MySqlCommand(deleteSql, connection))
                {
                    foreach (var kv in payload.KeyColumns)
                    {
                        command.Parameters.AddWithValue($"@{kv.Key}", kv.Value ?? DBNull.Value);
                    }
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string EscapeTable(string tableName)
        {
            return $"`{tableName}`";
        }

        private static string EscapeColumn(string columnName)
        {
            return $"`{columnName}`";
        }
    }
}
