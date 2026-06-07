using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services.Logging
{
    public class ApplicationLogSyncService : IApplicationLogSyncService
    {
        private const int BatchSize = 100;
        private readonly IApplicationLogRepository _repository;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly IDatabaseConnectionChecker _connectionChecker;

        public ApplicationLogSyncService(
            IApplicationLogRepository repository,
            IDatabaseConnectionProvider connectionProvider,
            IDatabaseConnectionChecker connectionChecker)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
        }

        public async Task<int> SyncPendingToServerAsync(CancellationToken cancellationToken = default)
        {
            if (!_connectionChecker.CanConnectToServer())
                return 0;

            var batch = await _repository.GetUnsyncedBatchAsync(BatchSize, cancellationToken).ConfigureAwait(false);
            if (batch.Count == 0)
                return 0;

            using var connection = _connectionProvider.GetServerConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            const string sql = @"INSERT INTO ApplicationLogs
                (LogGuid, SourceType, Category, Level, Operation, Status, UserId, Username, MachineName,
                 PermissionScope, Message, Details, Exception, SourceContext, CreatedAt, SyncedToServerAt, Origin)
                VALUES
                (@LogGuid, @SourceType, @Category, @Level, @Operation, @Status, @UserId, @Username, @MachineName,
                 @PermissionScope, @Message, @Details, @Exception, @SourceContext, @CreatedAt, NULL, @Origin)
                ON DUPLICATE KEY UPDATE LogGuid = LogGuid;";

            using var command = new MySqlCommand(sql, connection);
            AddParameters(command);

            foreach (var entry in batch)
            {
                cancellationToken.ThrowIfCancellationRequested();
                FillParameters(command, entry);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            await _repository.MarkSyncedAsync(batch.Select(b => b.Id).ToList(), cancellationToken).ConfigureAwait(false);
            return batch.Count;
        }

        public async Task<int> DeleteByLogGuidsOnServerAsync(IReadOnlyList<string> logGuids, CancellationToken cancellationToken = default)
        {
            if (logGuids == null || logGuids.Count == 0)
                return 0;

            if (!_connectionChecker.CanConnectToServer())
                throw new InvalidOperationException("სერვერთან კავშირი არ არის.");

            using var connection = _connectionProvider.GetServerConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var totalDeleted = 0;
            foreach (var chunk in logGuids.Distinct(StringComparer.OrdinalIgnoreCase).Chunk(100))
            {
                var placeholders = string.Join(",", chunk.Select((_, index) => $"@g{index}"));
                var sql = $"DELETE FROM ApplicationLogs WHERE LogGuid IN ({placeholders});";
                using var command = new MySqlCommand(sql, connection);
                var index = 0;
                foreach (var guid in chunk)
                {
                    command.Parameters.AddWithValue($"@g{index}", guid);
                    index++;
                }

                totalDeleted += await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            return totalDeleted;
        }

        public async Task<int> DeleteAllOnServerAsync(CancellationToken cancellationToken = default)
        {
            if (!_connectionChecker.CanConnectToServer())
                throw new InvalidOperationException("სერვერთან კავშირი არ არის.");

            using var connection = _connectionProvider.GetServerConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            const string sql = "DELETE FROM ApplicationLogs;";
            using var command = new MySqlCommand(sql, connection);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private static void AddParameters(MySqlCommand command)
        {
            command.Parameters.Add("@LogGuid", MySqlDbType.VarChar);
            command.Parameters.Add("@SourceType", MySqlDbType.VarChar);
            command.Parameters.Add("@Category", MySqlDbType.VarChar);
            command.Parameters.Add("@Level", MySqlDbType.VarChar);
            command.Parameters.Add("@Operation", MySqlDbType.VarChar);
            command.Parameters.Add("@Status", MySqlDbType.VarChar);
            command.Parameters.Add("@UserId", MySqlDbType.Int32);
            command.Parameters.Add("@Username", MySqlDbType.VarChar);
            command.Parameters.Add("@MachineName", MySqlDbType.VarChar);
            command.Parameters.Add("@PermissionScope", MySqlDbType.VarChar);
            command.Parameters.Add("@Message", MySqlDbType.Text);
            command.Parameters.Add("@Details", MySqlDbType.Text);
            command.Parameters.Add("@Exception", MySqlDbType.Text);
            command.Parameters.Add("@SourceContext", MySqlDbType.VarChar);
            command.Parameters.Add("@CreatedAt", MySqlDbType.DateTime);
            command.Parameters.Add("@Origin", MySqlDbType.VarChar);
        }

        private static void FillParameters(MySqlCommand command, ApplicationLogEntry entry)
        {
            command.Parameters["@LogGuid"].Value = entry.LogGuid;
            command.Parameters["@SourceType"].Value = entry.SourceType;
            command.Parameters["@Category"].Value = (object)entry.Category ?? DBNull.Value;
            command.Parameters["@Level"].Value = entry.Level;
            command.Parameters["@Operation"].Value = (object)entry.Operation ?? DBNull.Value;
            command.Parameters["@Status"].Value = (object)entry.Status ?? DBNull.Value;
            command.Parameters["@UserId"].Value = entry.UserId.HasValue ? entry.UserId.Value : DBNull.Value;
            command.Parameters["@Username"].Value = (object)entry.Username ?? DBNull.Value;
            command.Parameters["@MachineName"].Value = (object)entry.MachineName ?? DBNull.Value;
            command.Parameters["@PermissionScope"].Value = (object)entry.PermissionScope ?? DBNull.Value;
            command.Parameters["@Message"].Value = (object)entry.Message ?? DBNull.Value;
            command.Parameters["@Details"].Value = (object)entry.Details ?? DBNull.Value;
            command.Parameters["@Exception"].Value = (object)entry.Exception ?? DBNull.Value;
            command.Parameters["@SourceContext"].Value = (object)entry.SourceContext ?? DBNull.Value;
            command.Parameters["@CreatedAt"].Value = entry.CreatedAt;
            command.Parameters["@Origin"].Value = entry.Origin ?? "Local";
        }
    }
}
