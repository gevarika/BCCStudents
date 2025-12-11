using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    public class DownStreamSyncService : IDownStreamSyncService
    {
        private static readonly string[] DefaultTables = new[]
        {
            "Students",
            "Groups",
            "SubGroups",
            "StudentGroups",
            "StudentSubGroups"
        };

        private readonly IDownStreamSyncRepository _repository;
        private readonly IDownStreamDataFetcher _dataFetcher;
        private readonly IDownStreamConflictResolver _conflictResolver;
        private readonly ISyncLogger _logger;

        public DownStreamSyncService(
            IDownStreamSyncRepository repository,
            IDownStreamDataFetcher dataFetcher,
            IDownStreamConflictResolver conflictResolver,
            ISyncLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dataFetcher = dataFetcher ?? throw new ArgumentNullException(nameof(dataFetcher));
            _conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SyncResult> SyncFromServerAsync(CancellationToken cancellationToken = default)
        {
            var result = new SyncResult();

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
                _logger.Error("DownStream sync failed.", ex);
                result.AddError(ex.Message);
            }

            return result;
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
                default:
                    return TableSyncResult.Failed(tableName ?? "<unknown>", "áƒ£áƒªáƒœáƒáƒ‘áƒ˜ áƒªáƒ®áƒ áƒ˜áƒšáƒ˜áƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜");
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
                _logger.Error("Students downstream sync failed.", ex);
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
                _logger.Error("Groups downstream sync failed.", ex);
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
                _logger.Error("SubGroups downstream sync failed.", ex);
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
                _logger.Error("StudentGroups downstream sync failed.", ex);
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
                _logger.Error("StudentSubGroups downstream sync failed.", ex);
                return TableSyncResult.Failed(tableName, ex.Message);
            }
        }
    }
}




