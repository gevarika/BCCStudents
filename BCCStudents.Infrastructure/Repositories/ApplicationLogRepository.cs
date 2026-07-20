using System.Data.Common;
using System.Text;
using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Logging;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class ApplicationLogRepository : IApplicationLogRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private static readonly object SchemaLock = new();
        private static bool _schemaEnsured;

        public ApplicationLogRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        public async Task InsertAsync(ApplicationLogEntry entry, CancellationToken cancellationToken = default)
        {
            await InsertBatchAsync(new[] { entry }, cancellationToken).ConfigureAwait(false);
        }

        public async Task InsertBatchAsync(IReadOnlyList<ApplicationLogEntry> entries, CancellationToken cancellationToken = default)
        {
            if (entries == null || entries.Count == 0)
                return;

            EnsureSchema();

            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            const string sql = @"INSERT INTO ApplicationLogs
                (LogGuid, SourceType, Category, Level, Operation, Status, UserId, Username, MachineName,
                 PermissionScope, Message, Details, Exception, SourceContext, CreatedAt, SyncedToServerAt, Origin)
                VALUES
                (@LogGuid, @SourceType, @Category, @Level, @Operation, @Status, @UserId, @Username, @MachineName,
                 @PermissionScope, @Message, @Details, @Exception, @SourceContext, @CreatedAt, @SyncedToServerAt, @Origin);";

            using var command = new MySqlCommand(sql, connection);
            AddInsertParameters(command);

            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                FillInsertParameters(command, entry);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IReadOnlyList<ApplicationLogEntry>> GetFilteredAsync(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            bool isAdmin,
            bool canViewSystemLogs,
            int currentUserId,
            IReadOnlyList<string> allowedPermissionScopes,
            IReadOnlyList<string> allowedCategories,
            int limit,
            CancellationToken cancellationToken = default)
        {
            EnsureSchema();

            var filter = CreateListFilter(
                from, to, sourceType, category, level, username, operation, searchText,
                isAdmin, canViewSystemLogs, currentUserId, allowedPermissionScopes, allowedCategories);

            var sql = new StringBuilder(@"SELECT Id, LogGuid, SourceType, Category, Level, Operation, Status, UserId, Username,
                               MachineName, PermissionScope, Message, Details, Exception, SourceContext,
                               CreatedAt, SyncedToServerAt, Origin
                        FROM ApplicationLogs
                        WHERE 1=1");
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using var command = new MySqlCommand(string.Empty, connection);

            AppendListFilter(sql, command, filter);
            sql.Append(" ORDER BY CreatedAt DESC LIMIT @Limit");
            command.Parameters.AddWithValue("@Limit", Math.Max(1, Math.Min(limit, 5000)));
            command.CommandText = sql.ToString();

            var results = new List<ApplicationLogEntry>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                results.Add(Map(reader));

            return results;
        }

        public async Task<IReadOnlyList<ApplicationLogEntry>> GetFilteredFromServerAsync(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            bool isAdmin,
            bool canViewSystemLogs,
            int currentUserId,
            IReadOnlyList<string> allowedPermissionScopes,
            IReadOnlyList<string> allowedCategories,
            int limit,
            CancellationToken cancellationToken = default)
        {
            var filter = CreateListFilter(
                from, to, sourceType, category, level, username, operation, searchText,
                isAdmin, canViewSystemLogs, currentUserId, allowedPermissionScopes, allowedCategories);

            var sql = new StringBuilder(@"SELECT Id, LogGuid, SourceType, Category, Level, Operation, Status, UserId, Username,
                               MachineName, PermissionScope, Message, Details, Exception, SourceContext,
                               CreatedAt, SyncedToServerAt, Origin
                        FROM ApplicationLogs
                        WHERE 1=1");
            using var connection = _connectionProvider.GetServerConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using var command = new MySqlCommand(string.Empty, connection);

            AppendListFilter(sql, command, filter);
            sql.Append(" ORDER BY CreatedAt DESC LIMIT @Limit");
            command.Parameters.AddWithValue("@Limit", Math.Max(1, Math.Min(limit, 5000)));
            command.CommandText = sql.ToString();

            var results = new List<ApplicationLogEntry>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                results.Add(Map(reader));

            return results;
        }

        public async Task<int> DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureSchema();
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            const string sql = "DELETE FROM ApplicationLogs;";
            using var command = new MySqlCommand(sql, connection);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> DeleteByIdsAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Count == 0)
                return 0;

            EnsureSchema();
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var sql = $"DELETE FROM ApplicationLogs WHERE Id IN ({string.Join(",", ids.Distinct())});";
            using var command = new MySqlCommand(sql, connection);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> DeleteFilteredAsync(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            CancellationToken cancellationToken = default)
        {
            EnsureSchema();

            var filter = CreateAdminListFilter(from, to, sourceType, category, level, username, operation, searchText);
            var sql = new StringBuilder("DELETE FROM ApplicationLogs WHERE 1=1");
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using var command = new MySqlCommand(string.Empty, connection);

            AppendListFilter(sql, command, filter);
            command.CommandText = sql.ToString();
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetLogGuidsByIdsAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Count == 0)
                return Array.Empty<string>();

            EnsureSchema();
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var sql = $"SELECT LogGuid FROM ApplicationLogs WHERE Id IN ({string.Join(",", ids.Distinct())});";
            using var command = new MySqlCommand(sql, connection);

            var results = new List<string>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var guid = ReadColumnString(reader, "LogGuid");
                if (!string.IsNullOrWhiteSpace(guid))
                    results.Add(guid);
            }

            return results;
        }

        public async Task<IReadOnlyList<string>> GetLogGuidsFilteredAsync(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            CancellationToken cancellationToken = default)
        {
            EnsureSchema();

            var filter = CreateAdminListFilter(from, to, sourceType, category, level, username, operation, searchText);
            var sql = new StringBuilder("SELECT LogGuid FROM ApplicationLogs WHERE 1=1");
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using var command = new MySqlCommand(string.Empty, connection);

            AppendListFilter(sql, command, filter);
            command.CommandText = sql.ToString();

            var results = new List<string>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var guid = ReadColumnString(reader, "LogGuid");
                if (!string.IsNullOrWhiteSpace(guid))
                    results.Add(guid);
            }

            return results;
        }

        public async Task<IReadOnlyList<ApplicationLogEntry>> GetUnsyncedBatchAsync(int limit, CancellationToken cancellationToken = default)
        {
            EnsureSchema();
            var batchSize = Math.Max(1, Math.Min(limit, 100));

            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            const string sql = @"SELECT Id, LogGuid, SourceType, Category, Level, Operation, Status, UserId, Username,
                                        MachineName, PermissionScope, Message, Details, Exception, SourceContext,
                                        CreatedAt, SyncedToServerAt, Origin
                                 FROM ApplicationLogs
                                 WHERE SyncedToServerAt IS NULL AND Origin = 'Local'
                                 ORDER BY Id ASC
                                 LIMIT @Limit;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Limit", batchSize);

            var results = new List<ApplicationLogEntry>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                results.Add(Map(reader));

            return results;
        }

        public async Task MarkSyncedAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Count == 0)
                return;

            EnsureSchema();
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var idList = string.Join(",", ids);
            var sql = $"UPDATE ApplicationLogs SET SyncedToServerAt = @SyncedAt WHERE Id IN ({idList});";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> DeleteOlderThanAsync(DateTime cutoff, CancellationToken cancellationToken = default)
        {
            EnsureSchema();
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            const string sql = "DELETE FROM ApplicationLogs WHERE CreatedAt < @Cutoff;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Cutoff", cutoff);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> InsertFromServerAsync(IReadOnlyList<ApplicationLogEntry> entries, CancellationToken cancellationToken = default)
        {
            if (entries == null || entries.Count == 0)
                return 0;

            EnsureSchema();
            using var connection = _connectionProvider.GetLocalConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var guids = entries
                .Select(e => e.LogGuid)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var existingGuids = await GetExistingLogGuidsAsync(connection, guids, cancellationToken).ConfigureAwait(false);

            var toInsert = entries
                .Where(e => !string.IsNullOrWhiteSpace(e.LogGuid) && !existingGuids.Contains(e.LogGuid))
                .ToList();
            if (toInsert.Count == 0)
                return 0;

            const string sql = @"INSERT INTO ApplicationLogs
                (LogGuid, SourceType, Category, Level, Operation, Status, UserId, Username, MachineName,
                 PermissionScope, Message, Details, Exception, SourceContext, CreatedAt, SyncedToServerAt, Origin)
                VALUES
                (@LogGuid, @SourceType, @Category, @Level, @Operation, @Status, @UserId, @Username, @MachineName,
                 @PermissionScope, @Message, @Details, @Exception, @SourceContext, @CreatedAt, @SyncedToServerAt, @Origin);";

            using var command = new MySqlCommand(sql, connection);
            AddInsertParameters(command);

            foreach (var entry in toInsert)
            {
                cancellationToken.ThrowIfCancellationRequested();
                entry.Origin = "Server";
                FillInsertParameters(command, entry);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            return toInsert.Count;
        }

        private static async Task<HashSet<string>> GetExistingLogGuidsAsync(
            MySqlConnection connection,
            IReadOnlyList<string> logGuids,
            CancellationToken cancellationToken)
        {
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (logGuids == null || logGuids.Count == 0)
                return existing;

            foreach (var chunk in logGuids.Chunk(100))
            {
                var placeholders = string.Join(",", chunk.Select((_, i) => $"@g{i}"));
                var sql = $"SELECT LogGuid FROM ApplicationLogs WHERE LogGuid IN ({placeholders});";
                using var command = new MySqlCommand(sql, connection);
                var index = 0;
                foreach (var guid in chunk)
                {
                    command.Parameters.AddWithValue($"@g{index}", guid);
                    index++;
                }

                using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (reader["LogGuid"] != DBNull.Value)
                        existing.Add(reader["LogGuid"].ToString());
                }
            }

            return existing;
        }

        private void EnsureSchema()
        {
            if (_schemaEnsured) return;
            lock (SchemaLock)
            {
                if (_schemaEnsured) return;
                EnsureTable(_connectionProvider);
            }
        }

        public static void EnsureTable(IDatabaseConnectionProvider connectionProvider)
        {
            lock (SchemaLock)
            {
                using var connection = connectionProvider.GetLocalConnection();
                connection.Open();
                const string sql = @"CREATE TABLE IF NOT EXISTS ApplicationLogs (
                    Id BIGINT NOT NULL AUTO_INCREMENT,
                    LogGuid CHAR(36) NOT NULL,
                    SourceType VARCHAR(32) NOT NULL,
                    Category VARCHAR(64) NULL,
                    Level VARCHAR(16) NOT NULL,
                    Operation VARCHAR(128) NULL,
                    Status VARCHAR(32) NULL,
                    UserId INT NULL,
                    Username VARCHAR(128) NULL,
                    MachineName VARCHAR(128) NULL,
                    PermissionScope VARCHAR(64) NULL,
                    Message TEXT NULL,
                    Details TEXT NULL,
                    Exception TEXT NULL,
                    SourceContext VARCHAR(128) NULL,
                    CreatedAt DATETIME(3) NOT NULL,
                    SyncedToServerAt DATETIME(3) NULL,
                    Origin VARCHAR(16) NOT NULL DEFAULT 'Local',
                    PRIMARY KEY (Id),
                    UNIQUE KEY uk_log_guid (LogGuid),
                    KEY idx_created (CreatedAt),
                    KEY idx_user_created (UserId, CreatedAt),
                    KEY idx_category (Category, CreatedAt),
                    KEY idx_unsynced (SyncedToServerAt, Id)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                using var command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();
                _schemaEnsured = true;
            }
        }

        private static void AddInsertParameters(MySqlCommand command)
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
            command.Parameters.Add("@SyncedToServerAt", MySqlDbType.DateTime);
            command.Parameters.Add("@Origin", MySqlDbType.VarChar);
        }

        private static void FillInsertParameters(MySqlCommand command, ApplicationLogEntry entry)
        {
            command.Parameters["@LogGuid"].Value = entry.LogGuid ?? Guid.NewGuid().ToString();
            command.Parameters["@SourceType"].Value = entry.SourceType ?? LogSourceType.App;
            command.Parameters["@Category"].Value = (object)entry.Category ?? DBNull.Value;
            command.Parameters["@Level"].Value = entry.Level ?? "Information";
            command.Parameters["@Operation"].Value = (object)entry.Operation ?? DBNull.Value;
            command.Parameters["@Status"].Value = (object)entry.Status ?? DBNull.Value;
            command.Parameters["@UserId"].Value = entry.UserId.HasValue ? entry.UserId.Value : DBNull.Value;
            command.Parameters["@Username"].Value = (object)entry.Username ?? DBNull.Value;
            command.Parameters["@MachineName"].Value = (object)entry.MachineName ?? DBNull.Value;
            command.Parameters["@PermissionScope"].Value = (object)entry.PermissionScope ?? DBNull.Value;
            command.Parameters["@Message"].Value = (object)LogMessageSanitizer.Sanitize(entry.Message) ?? DBNull.Value;
            command.Parameters["@Details"].Value = (object)LogMessageSanitizer.Sanitize(entry.Details) ?? DBNull.Value;
            command.Parameters["@Exception"].Value = (object)LogMessageSanitizer.Sanitize(entry.Exception) ?? DBNull.Value;
            command.Parameters["@SourceContext"].Value = (object)entry.SourceContext ?? DBNull.Value;
            command.Parameters["@CreatedAt"].Value = entry.CreatedAt == default ? DateTime.UtcNow : entry.CreatedAt;
            command.Parameters["@SyncedToServerAt"].Value = (object)entry.SyncedToServerAt ?? DBNull.Value;
            command.Parameters["@Origin"].Value = entry.Origin ?? "Local";
        }

        private static ApplicationLogEntry Map(DbDataReader reader)
        {
            return new ApplicationLogEntry
            {
                Id = Convert.ToInt64(reader["Id"]),
                LogGuid = ReadColumnString(reader, "LogGuid"),
                SourceType = reader["SourceType"]?.ToString(),
                Category = reader["Category"] == DBNull.Value ? null : reader["Category"]?.ToString(),
                Level = reader["Level"]?.ToString(),
                Operation = reader["Operation"] == DBNull.Value ? null : reader["Operation"]?.ToString(),
                Status = reader["Status"] == DBNull.Value ? null : reader["Status"]?.ToString(),
                UserId = reader["UserId"] == DBNull.Value ? null : Convert.ToInt32(reader["UserId"]),
                Username = reader["Username"] == DBNull.Value ? null : reader["Username"]?.ToString(),
                MachineName = reader["MachineName"] == DBNull.Value ? null : reader["MachineName"]?.ToString(),
                PermissionScope = reader["PermissionScope"] == DBNull.Value ? null : reader["PermissionScope"]?.ToString(),
                Message = reader["Message"] == DBNull.Value ? null : reader["Message"]?.ToString(),
                Details = reader["Details"] == DBNull.Value ? null : reader["Details"]?.ToString(),
                Exception = reader["Exception"] == DBNull.Value ? null : reader["Exception"]?.ToString(),
                SourceContext = reader["SourceContext"] == DBNull.Value ? null : reader["SourceContext"]?.ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                SyncedToServerAt = reader["SyncedToServerAt"] == DBNull.Value ? null : Convert.ToDateTime(reader["SyncedToServerAt"]),
                Origin = reader["Origin"]?.ToString()
            };
        }

        private static string ReadColumnString(DbDataReader reader, string column)
        {
            var value = reader[column];
            if (value == DBNull.Value || value == null)
                return null;

            return value switch
            {
                Guid guid => guid.ToString("D"),
                _ => value.ToString()
            };
        }

        private sealed class ApplicationLogListFilter
        {
            public DateTime? From { get; init; }
            public DateTime? To { get; init; }
            public string SourceType { get; init; }
            public string Category { get; init; }
            public string Level { get; init; }
            public string Username { get; init; }
            public string Operation { get; init; }
            public string SearchText { get; init; }
            public bool IsAdmin { get; init; }
            public bool CanViewSystemLogs { get; init; }
            public int CurrentUserId { get; init; }
            public IReadOnlyList<string> AllowedPermissionScopes { get; init; }
            public IReadOnlyList<string> AllowedCategories { get; init; }
            public bool IncludeDateFilter { get; init; } = true;
        }

        private static ApplicationLogListFilter CreateListFilter(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            bool isAdmin,
            bool canViewSystemLogs,
            int currentUserId,
            IReadOnlyList<string> allowedPermissionScopes,
            IReadOnlyList<string> allowedCategories,
            bool includeDateFilter = true)
        {
            return new ApplicationLogListFilter
            {
                From = from,
                To = to,
                SourceType = sourceType,
                Category = category,
                Level = level,
                Username = username,
                Operation = operation,
                SearchText = searchText,
                IsAdmin = isAdmin,
                CanViewSystemLogs = canViewSystemLogs,
                CurrentUserId = currentUserId,
                AllowedPermissionScopes = allowedPermissionScopes,
                AllowedCategories = allowedCategories,
                IncludeDateFilter = includeDateFilter
            };
        }

        private static ApplicationLogListFilter CreateAdminListFilter(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText)
        {
            return new ApplicationLogListFilter
            {
                From = from,
                To = to,
                SourceType = sourceType,
                Category = category,
                Level = level,
                Username = username,
                Operation = operation,
                SearchText = searchText,
                IsAdmin = true
            };
        }

        private static void AppendListFilter(StringBuilder sql, MySqlCommand command, ApplicationLogListFilter filter)
        {
            if (filter.IncludeDateFilter)
            {
                sql.Append(" AND CreatedAt >= @From AND CreatedAt < @To");
                command.Parameters.AddWithValue("@From", filter.From ?? DateTime.UtcNow.AddYears(-1));
                command.Parameters.AddWithValue("@To", (filter.To ?? DateTime.UtcNow).Date.AddDays(1));
            }

            if (!string.IsNullOrWhiteSpace(filter.SourceType))
            {
                sql.Append(" AND SourceType = @SourceType");
                command.Parameters.AddWithValue("@SourceType", filter.SourceType);
            }

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                sql.Append(" AND Category = @Category");
                command.Parameters.AddWithValue("@Category", filter.Category);
            }

            if (!string.IsNullOrWhiteSpace(filter.Level))
            {
                sql.Append(" AND Level = @Level");
                command.Parameters.AddWithValue("@Level", filter.Level);
            }

            if (!string.IsNullOrWhiteSpace(filter.Username))
            {
                sql.Append(" AND Username = @Username");
                command.Parameters.AddWithValue("@Username", filter.Username);
            }

            if (!string.IsNullOrWhiteSpace(filter.Operation))
            {
                sql.Append(" AND Operation LIKE @Operation");
                command.Parameters.AddWithValue("@Operation", "%" + filter.Operation + "%");
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                sql.Append(" AND (Message LIKE @Search OR Details LIKE @Search OR Exception LIKE @Search)");
                command.Parameters.AddWithValue("@Search", "%" + filter.SearchText + "%");
            }

            AppendPermissionFilter(sql, command, filter);
        }

        private static void AppendPermissionFilter(StringBuilder sql, MySqlCommand command, ApplicationLogListFilter filter)
        {
            if (filter.IsAdmin)
                return;

            var permIn = BuildInClause(filter.AllowedPermissionScopes);
            var catIn = BuildInClause(filter.AllowedCategories, "Cat");
            sql.Append(@" AND (
                    (SourceType = @AuditSource AND (
                        UserId = @CurrentUserId
                        OR PermissionScope IN (" + permIn + @")
                        OR Category IN (" + catIn + @")))
                    OR (@CanViewSystemLogs = 1 AND SourceType IN (@AppSource, @SyncSource, @ConnectionSource))
                )");

            command.Parameters.AddWithValue("@CurrentUserId", filter.CurrentUserId);
            command.Parameters.AddWithValue("@AuditSource", LogSourceType.Audit);
            command.Parameters.AddWithValue("@AppSource", LogSourceType.App);
            command.Parameters.AddWithValue("@SyncSource", LogSourceType.Sync);
            command.Parameters.AddWithValue("@ConnectionSource", LogSourceType.Connection);
            command.Parameters.AddWithValue("@CanViewSystemLogs", filter.CanViewSystemLogs ? 1 : 0);
            AddInParameters(command, filter.AllowedPermissionScopes, "Perm");
            AddInParameters(command, filter.AllowedCategories, "Cat");
        }

        private static string BuildInClause(IReadOnlyList<string> values, string prefix = "Perm")
        {
            if (values == null || values.Count == 0)
                return "''";

            return string.Join(",", values.Select((_, i) => $"@{prefix}{i}"));
        }

        private static void AddInParameters(MySqlCommand command, IReadOnlyList<string> values, string prefix = "Perm")
        {
            if (values == null || values.Count == 0)
                return;

            for (var i = 0; i < values.Count; i++)
                command.Parameters.AddWithValue($"@{prefix}{i}", values[i]);
        }
    }
}
