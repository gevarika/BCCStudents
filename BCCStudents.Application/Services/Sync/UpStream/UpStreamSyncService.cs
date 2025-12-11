using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Infrastructure.Data;
using MySql.Data.MySqlClient;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Services.Sync.UpStream
{
    /// <summary>
    /// áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ˜áƒ¡ áƒ‘áƒáƒ–áƒáƒ¨áƒ˜ áƒ£áƒ¨áƒ£áƒáƒšáƒ áƒ©áƒáƒ¬áƒ”áƒ áƒ (INSERT/UPDATE/DELETE) â€“ Students/Groups/SubGroups áƒ“áƒ áƒ¡áƒ®áƒ•áƒ áƒªáƒ®áƒ áƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡.
    /// </summary>
    public class UpStreamSyncService : IUpStreamSyncService
    {
        private readonly DatabaseHelper _databaseHelper;
        private readonly ISyncLogger _logger;

        public UpStreamSyncService(DatabaseHelper databaseHelper, ISyncLogger logger)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> TrySyncImmediatelyAsync(SyncChangePayload payload, CancellationToken cancellationToken = default)
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

                _logger.Info($"UpStream (Immediate) áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ“áƒ: {payload.TableName}/{payload.Operation}/{payload.RecordKey}");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.Error($"UpStream (Immediate) áƒ•áƒ”áƒ  áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ“áƒ: {payload.TableName}/{payload.Operation}/{payload.RecordKey}", ex);
                return Task.FromResult(false);
            }
        }

        private void ExecuteUpsert(SyncChangePayload payload)
        {
            var columns = payload.Data.Keys.ToList();
            if (!columns.Any())
            {
                throw new InvalidOperationException("Upsert áƒáƒžáƒ”áƒ áƒáƒªáƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ áƒáƒ£áƒªáƒ˜áƒšáƒ”áƒ‘áƒ”áƒšáƒ˜áƒ áƒ›áƒ˜áƒœáƒ˜áƒ›áƒ£áƒ› áƒ”áƒ áƒ—áƒ˜ áƒ•áƒ”áƒšáƒ˜.");
            }

            var columnList = string.Join(", ", columns.Select(EscapeColumn));
            var parameterList = string.Join(", ", columns.Select(c => $"@{c}"));
            var updateList = string.Join(", ", columns.Select(c => $"{EscapeColumn(c)} = VALUES({EscapeColumn(c)})"));
            var sql = new StringBuilder();
            sql.Append($"INSERT INTO {EscapeTable(payload.TableName)} ({columnList}) VALUES ({parameterList}) ");
            sql.Append($"ON DUPLICATE KEY UPDATE {updateList};");

            using (var connection = _databaseHelper.GetServerConnection())
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
            using (var connection = _databaseHelper.GetServerConnection())
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
                    throw new InvalidOperationException("Delete áƒáƒžáƒ”áƒ áƒáƒªáƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ áƒáƒ£áƒªáƒ˜áƒšáƒ”áƒ‘áƒ”áƒšáƒ˜áƒ RecordId áƒáƒœ KeyColumns.");
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



