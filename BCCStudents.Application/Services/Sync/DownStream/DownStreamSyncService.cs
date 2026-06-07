using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class DownStreamSyncService : IDownStreamSyncService
    {
        private static readonly string[] DefaultTables = new[]
        {
            "Students",
            "Groups",
            "SubGroups",
            "StudentGroups",
            "StudentSubGroups",
            "Payments",
            "FailedPayments",
            "ImportedPaymentsLog",
            "Users",
            "SystemConfig",
            "PendingStudents",
            "PendingStudentGroups",
            "PendingStudentSubGroups",
            "ApplicationLogs"
        };

        private readonly IDownStreamSyncRepository _repository;
        private readonly IDownStreamDataFetcher _dataFetcher;
        private readonly IDownStreamConflictResolver _conflictResolver;
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private readonly ISyncLogger _logger;

        public DownStreamSyncService(
            IDownStreamSyncRepository repository,
            IDownStreamDataFetcher dataFetcher,
            IDownStreamConflictResolver conflictResolver,
            IDatabaseConnectionChecker connectionChecker,
            ISyncLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dataFetcher = dataFetcher ?? throw new ArgumentNullException(nameof(dataFetcher));
            _conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SyncResult> SyncFromServerAsync(CancellationToken cancellationToken = default)
        {
            var result = new SyncResult();

            var serverCheck = _connectionChecker.CheckServerConnection();
            if (!serverCheck.IsConnected)
            {
                var userMessage = serverCheck.Failure?.UserMessage ?? "სერვერთან კავშირი ვერ დამყარდა.";
                var logDetail = serverCheck.Failure?.LogDetail ?? userMessage;
                SyncLogThrottle.TryWarn(_logger, "downstream-server-offline",
                    $"DownStream sync გამოტოვებულია. {logDetail}", SyncLogThrottle.DefaultInterval);
                result.AddError(userMessage);
                return result;
            }

            try
            {
                var tables = await GetTablesToSyncAsync().ConfigureAwait(false);
                foreach (var table in tables)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var tableResult = await SyncTableAsync(table, cancellationToken).ConfigureAwait(false);
                    result.AddTableResult(tableResult);
                    if (!tableResult.Success && !string.IsNullOrWhiteSpace(tableResult.Error))
                    {
                        result.AddError($"{table}: {tableResult.Error}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogDownStreamFailure("DownStream sync failed.", ex);
                result.AddError(ex.Message);
            }

            LogSyncResult(result);
            return result;
        }

        private void LogSyncResult(SyncResult result)
        {
            if (result == null)
            {
                return;
            }

            foreach (var table in result.Tables.Where(t => t.Success && t.RecordsSynced > 0))
            {
                _logger.Info($"DownStream (Pull) წარმატებით ჩამოტვირთული: {table.TableName}/{table.RecordsSynced}");
            }

            if (!result.Success)
            {
                LogDownStreamResultErrors(result);
                return;
            }

            var totalRecords = result.Tables.Sum(t => t.RecordsSynced);
            if (totalRecords == 0)
                return;

            var changedTableCount = result.Tables.Count(t => t.RecordsSynced > 0);
            _logger.Info($"DownStream sync დასრულდა: სულ {totalRecords} ჩანაწერი ({changedTableCount} ცხრილი)");
        }

        private void LogDownStreamResultErrors(SyncResult result)
        {
            var failedTables = result.Tables.Where(t => !t.Success).ToList();
            var connectionFailures = failedTables
                .Where(t => SyncConnectionHelper.IsLikelyConnectionErrorMessage(t.Error))
                .ToList();

            if (connectionFailures.Count > 0)
            {
                var sample = connectionFailures[0].Error ?? string.Empty;
                var detail = SyncConnectionHelper.IsLikelyConnectionErrorMessage(sample)
                    ? sample
                    : $"{connectionFailures.Count} ცხრილი";
                SyncLogThrottle.TryWarn(_logger, "downstream-connection",
                    $"DownStream sync: სერვერთან კავშირი ვერ დამყარდა ({connectionFailures.Count} ცხრილი). Sample: {detail}",
                    SyncLogThrottle.DefaultInterval);
            }

            foreach (var table in failedTables.Except(connectionFailures))
            {
                _logger.Error($"DownStream (Pull) შეცდომა: {table.TableName} — {table.Error}");
            }

            if (connectionFailures.Count == 0)
            {
                var errorSummary = result.Errors.Count > 0
                    ? string.Join("; ", result.Errors)
                    : "უცნობი შეცდომა";
                _logger.Warn($"DownStream sync დასრულდა შეცდომებით: {errorSummary}");
            }
        }

        private void LogDownStreamFailure(string message, Exception ex)
        {
            if (SyncConnectionHelper.IsLikelyConnectionError(ex))
            {
                SyncLogThrottle.TryError(_logger, "downstream-connection", message, ex, SyncLogThrottle.DefaultInterval);
                return;
            }

            _logger.Error(message, ex);
        }

        public async Task<TableSyncResult> SyncTableAsync(string tableName, CancellationToken cancellationToken = default)
        {
            switch (tableName?.Trim().ToUpperInvariant())
            {
                case "STUDENTS":
                    return await SyncStudentsAsync(cancellationToken).ConfigureAwait(false);
                case "GROUPS":
                    return await SyncGroupsAsync(cancellationToken).ConfigureAwait(false);
                case "SUBGROUPS":
                    return await SyncSubGroupsAsync(cancellationToken).ConfigureAwait(false);
                case "STUDENTGROUPS":
                    return await SyncStudentGroupsAsync(cancellationToken).ConfigureAwait(false);
                case "STUDENTSUBGROUPS":
                    return await SyncStudentSubGroupsAsync(cancellationToken).ConfigureAwait(false);
                case "PAYMENTS":
                    return await SyncPaymentsAsync(cancellationToken).ConfigureAwait(false);
                case "FAILEDPAYMENTS":
                    return await SyncFailedPaymentsAsync(cancellationToken).ConfigureAwait(false);
                case "IMPORTEDPAYMENTSLOG":
                    return await SyncImportedPaymentLogsAsync(cancellationToken).ConfigureAwait(false);
                case "USERS":
                    return await SyncUsersAsync(cancellationToken).ConfigureAwait(false);
                case "SYSTEMCONFIG":
                    return await SyncSystemConfigAsync(cancellationToken).ConfigureAwait(false);
                case "PENDINGSTUDENTS":
                    return await SyncPendingStudentsAsync(cancellationToken).ConfigureAwait(false);
                case "PENDINGSTUDENTGROUPS":
                    return await SyncPendingStudentGroupsAsync(cancellationToken).ConfigureAwait(false);
                case "PENDINGSTUDENTSUBGROUPS":
                    return await SyncPendingStudentSubGroupsAsync(cancellationToken).ConfigureAwait(false);
                case "APPLICATIONLOGS":
                    return await SyncApplicationLogsAsync(cancellationToken).ConfigureAwait(false);
                default:
                    return TableSyncResult.Failed(tableName ?? "<unknown>", "ცხრილი არ მოიძებნა");
            }
        }

        public Task<IReadOnlyList<string>> GetTablesToSyncAsync()
        {
            return Task.FromResult<IReadOnlyList<string>>(DefaultTables);
        }

        private async Task<TableSyncResult> SyncStudentsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "Students";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchStudentsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }
                await _repository.UpsertStudentsAsync(serverData, cancellationToken).ConfigureAwait(false);
                var maxUpdatedAt = serverData.Max(s => s.UpdatedAt);
                var maxId = serverData.Where(s => s.UpdatedAt == maxUpdatedAt).Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxUpdatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncGroupsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "Groups";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchGroupsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }
                await _repository.UpsertGroupsAsync(serverData, cancellationToken).ConfigureAwait(false);
                var maxUpdatedAt = serverData.Max(s => s.UpdatedAt);
                var maxId = serverData.Where(s => s.UpdatedAt == maxUpdatedAt).Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxUpdatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncSubGroupsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "SubGroups";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchSubGroupsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }
                await _repository.UpsertSubGroupsAsync(serverData, cancellationToken).ConfigureAwait(false);
                var maxUpdatedAt = serverData.Max(s => s.UpdatedAt);
                var maxId = serverData.Where(s => s.UpdatedAt == maxUpdatedAt).Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxUpdatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncStudentGroupsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "StudentGroups";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchStudentGroupsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }
                await _repository.UpsertStudentGroupsAsync(serverData, cancellationToken).ConfigureAwait(false);
                var maxUpdatedAt = serverData.Max(s => s.UpdatedAt);
                var maxId = serverData.Where(s => s.UpdatedAt == maxUpdatedAt).Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxUpdatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncStudentSubGroupsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "StudentSubGroups";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchStudentSubGroupsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }
                await _repository.UpsertStudentSubGroupsAsync(serverData, cancellationToken).ConfigureAwait(false);
                var maxUpdatedAt = serverData.Max(s => s.UpdatedAt);
                var maxId = serverData.Where(s => s.UpdatedAt == maxUpdatedAt).Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxUpdatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncPaymentsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "Payments";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchPaymentsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertPaymentsAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxSyncedAt = serverData.Max(p => p.UpdatedAt ?? p.PaymentDate);
                var maxId = serverData.Where(p => (p.UpdatedAt ?? p.PaymentDate) == maxSyncedAt).Max(p => p.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxSyncedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncFailedPaymentsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "FailedPayments";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchFailedPaymentsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertFailedPaymentsAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxCreatedAt = serverData.Max(p => p.CreatedAt);
                var maxId = serverData.Where(p => p.CreatedAt == maxCreatedAt).Max(p => p.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxCreatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncImportedPaymentLogsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "ImportedPaymentsLog";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchImportedPaymentLogsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertImportedPaymentLogsAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxCreatedAt = serverData.Max(p => p.CreatedAt);
                var maxId = serverData.Where(p => p.CreatedAt == maxCreatedAt).Max(p => p.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxCreatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncUsersAsync(CancellationToken cancellationToken)
        {
            const string tableName = "Users";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchUsersAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertUsersAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxUpdatedAt = serverData.Max(u => u.UpdatedAt);
                var maxId = serverData.Where(u => u.UpdatedAt == maxUpdatedAt).Max(u => u.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxUpdatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncSystemConfigAsync(CancellationToken cancellationToken)
        {
            const string tableName = "SystemConfig";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchSystemConfigAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertSystemConfigAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxSyncedAt = serverData.Max(c => c.UpdatedAt ?? c.CreatedAt);
                var maxId = serverData.Where(c => (c.UpdatedAt ?? c.CreatedAt) == maxSyncedAt).Max(c => c.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxSyncedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncPendingStudentsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "PendingStudents";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchPendingStudentsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertPendingStudentsAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxCreatedAt = serverData.Max(s => s.CreatedAt);
                var maxId = serverData.Where(s => s.CreatedAt == maxCreatedAt).Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, maxCreatedAt, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncPendingStudentGroupsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "PendingStudentGroups";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchPendingStudentGroupsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertPendingStudentGroupsAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxId = serverData.Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, DateTime.UtcNow, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncPendingStudentSubGroupsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "PendingStudentSubGroups";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchPendingStudentSubGroupsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var conflicts = await _conflictResolver.DetectConflictsAsync(tableName, serverData, cancellationToken).ConfigureAwait(false);
                foreach (var conflict in conflicts)
                {
                    await _conflictResolver.ResolveConflictAsync(conflict, null, cancellationToken).ConfigureAwait(false);
                }

                await _repository.UpsertPendingStudentSubGroupsAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxId = serverData.Max(s => s.Id);
                await _repository.UpdateSyncStateAsync(tableName, DateTime.UtcNow, maxId, cancellationToken).ConfigureAwait(false);

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = serverData.Count,
                    ConflictsResolved = conflicts.Count
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }

        private async Task<TableSyncResult> SyncApplicationLogsAsync(CancellationToken cancellationToken)
        {
            const string tableName = "ApplicationLogs";
            try
            {
                var state = await _repository.GetSyncStateAsync(tableName, cancellationToken).ConfigureAwait(false);
                var serverData = await _dataFetcher.FetchApplicationLogsAsync(state?.LastSyncedAt, state?.LastSyncedId ?? 0, cancellationToken).ConfigureAwait(false);
                if (serverData == null || serverData.Count == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                var insertedCount = await _repository.UpsertApplicationLogsAsync(serverData, cancellationToken).ConfigureAwait(false);

                var maxCreatedAt = serverData.Max(l => l.CreatedAt);
                var maxId = (int)Math.Min(int.MaxValue, serverData.Where(l => l.CreatedAt == maxCreatedAt).Max(l => l.Id));
                await _repository.UpdateSyncStateAsync(tableName, maxCreatedAt, maxId, cancellationToken).ConfigureAwait(false);

                if (insertedCount == 0)
                {
                    return TableSyncResult.NoChanges(tableName);
                }

                return new TableSyncResult
                {
                    TableName = tableName,
                    Success = true,
                    RecordsSynced = insertedCount,
                    ConflictsResolved = 0
                };
            }
            catch (Exception ex)
            {
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }
    }
}
