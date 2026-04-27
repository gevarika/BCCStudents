using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Sync.UpStream
{
    /// <summary>
    /// ცვლილებების დაფიქსირება და დაუყოვნებლივი/გადავადებული გაგზავნა სერვერზე.
    /// </summary>
    public class UpStreamChangeTracker : IUpStreamChangeTracker
    {
        private readonly IUpStreamPayloadBuilder _payloadBuilder;
        private readonly IUpStreamSyncService _syncService;
        private readonly IUpStreamSyncRepository _repository;
        private readonly ISyncLogger _logger;

        public UpStreamChangeTracker(
            IUpStreamPayloadBuilder payloadBuilder,
            IUpStreamSyncService syncService,
            IUpStreamSyncRepository repository,
            ISyncLogger logger)
        {
            _payloadBuilder = payloadBuilder ?? throw new ArgumentNullException(nameof(payloadBuilder));
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Students

        public Task TrackStudentChangeAsync(int studentId, SyncOperationType operation, Student snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildStudentPayload(studentId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackStudentChangeAsync(int studentId, string operation, Student snapshot, CancellationToken cancellationToken = default)
        {
            return TrackStudentChangeAsync(studentId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackStudentChange(int studentId, SyncOperationType operation, Student snapshot)
        {
            _ = TrackStudentChangeAsync(studentId, operation, snapshot);
        }

        public void TrackStudentChange(int studentId, string operation, Student snapshot)
        {
            _ = TrackStudentChangeAsync(studentId, operation, snapshot);
        }

        #endregion

        #region Groups

        public Task TrackGroupChangeAsync(int groupId, SyncOperationType operation, Group snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildGroupPayload(groupId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackGroupChangeAsync(int groupId, string operation, Group snapshot, CancellationToken cancellationToken = default)
        {
            return TrackGroupChangeAsync(groupId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackGroupChange(int groupId, SyncOperationType operation, Group snapshot)
        {
            _ = TrackGroupChangeAsync(groupId, operation, snapshot);
        }

        public void TrackGroupChange(int groupId, string operation, Group snapshot)
        {
            _ = TrackGroupChangeAsync(groupId, operation, snapshot);
        }

        #endregion

        #region SubGroups

        public Task TrackSubGroupChangeAsync(int subGroupId, SyncOperationType operation, SubGroup snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildSubGroupPayload(subGroupId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackSubGroupChangeAsync(int subGroupId, string operation, SubGroup snapshot, CancellationToken cancellationToken = default)
        {
            return TrackSubGroupChangeAsync(subGroupId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackSubGroupChange(int subGroupId, SyncOperationType operation, SubGroup snapshot)
        {
            _ = TrackSubGroupChangeAsync(subGroupId, operation, snapshot);
        }

        public void TrackSubGroupChange(int subGroupId, string operation, SubGroup snapshot)
        {
            _ = TrackSubGroupChangeAsync(subGroupId, operation, snapshot);
        }

        #endregion

        #region StudentGroups

        public Task TrackStudentGroupChangeAsync(int recordId, SyncOperationType operation, StudentGroups snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildStudentGroupPayload(recordId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackStudentGroupChangeAsync(int recordId, string operation, StudentGroups snapshot, CancellationToken cancellationToken = default)
        {
            return TrackStudentGroupChangeAsync(recordId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackStudentGroupChange(int recordId, SyncOperationType operation, StudentGroups snapshot)
        {
            _ = TrackStudentGroupChangeAsync(recordId, operation, snapshot);
        }

        public void TrackStudentGroupChange(int recordId, string operation, StudentGroups snapshot)
        {
            _ = TrackStudentGroupChangeAsync(recordId, operation, snapshot);
        }

        #endregion

        #region StudentSubGroups

        public Task TrackStudentSubGroupChangeAsync(int recordId, SyncOperationType operation, StudentSubGroups snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildStudentSubGroupPayload(recordId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackStudentSubGroupChangeAsync(int recordId, string operation, StudentSubGroups snapshot, CancellationToken cancellationToken = default)
        {
            return TrackStudentSubGroupChangeAsync(recordId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackStudentSubGroupChange(int recordId, SyncOperationType operation, StudentSubGroups snapshot)
        {
            _ = TrackStudentSubGroupChangeAsync(recordId, operation, snapshot);
        }

        public void TrackStudentSubGroupChange(int recordId, string operation, StudentSubGroups snapshot)
        {
            _ = TrackStudentSubGroupChangeAsync(recordId, operation, snapshot);
        }

        #endregion

        #region FailedPayments

        public Task TrackFailedPaymentChangeAsync(int failedPaymentId, SyncOperationType operation, FailedPayment snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildFailedPaymentPayload(failedPaymentId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackFailedPaymentChangeAsync(int failedPaymentId, string operation, FailedPayment snapshot, CancellationToken cancellationToken = default)
        {
            return TrackFailedPaymentChangeAsync(failedPaymentId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackFailedPaymentChange(int failedPaymentId, SyncOperationType operation, FailedPayment snapshot)
        {
            _ = TrackFailedPaymentChangeAsync(failedPaymentId, operation, snapshot);
        }

        public void TrackFailedPaymentChange(int failedPaymentId, string operation, FailedPayment snapshot)
        {
            _ = TrackFailedPaymentChangeAsync(failedPaymentId, operation, snapshot);
        }

        #endregion

        #region ImportedPaymentsLog

        public Task TrackImportedPaymentLogChangeAsync(int importedPaymentLogId, SyncOperationType operation, ImportedPaymentLog snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildImportedPaymentLogPayload(importedPaymentLogId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackImportedPaymentLogChangeAsync(int importedPaymentLogId, string operation, ImportedPaymentLog snapshot, CancellationToken cancellationToken = default)
        {
            return TrackImportedPaymentLogChangeAsync(importedPaymentLogId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackImportedPaymentLogChange(int importedPaymentLogId, SyncOperationType operation, ImportedPaymentLog snapshot)
        {
            _ = TrackImportedPaymentLogChangeAsync(importedPaymentLogId, operation, snapshot);
        }

        public void TrackImportedPaymentLogChange(int importedPaymentLogId, string operation, ImportedPaymentLog snapshot)
        {
            _ = TrackImportedPaymentLogChangeAsync(importedPaymentLogId, operation, snapshot);
        }

        #endregion

        #region Payments

        public Task TrackPaymentChangeAsync(int paymentId, SyncOperationType operation, Payment snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildPaymentPayload(paymentId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackPaymentChangeAsync(int paymentId, string operation, Payment snapshot, CancellationToken cancellationToken = default)
        {
            return TrackPaymentChangeAsync(paymentId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackPaymentChange(int paymentId, SyncOperationType operation, Payment snapshot)
        {
            _ = TrackPaymentChangeAsync(paymentId, operation, snapshot);
        }

        public void TrackPaymentChange(int paymentId, string operation, Payment snapshot)
        {
            _ = TrackPaymentChangeAsync(paymentId, operation, snapshot);
        }

        #endregion

        #region Users

        public Task TrackUserChangeAsync(int userId, SyncOperationType operation, UserModel snapshot, CancellationToken cancellationToken = default)
        {
            var payload = _payloadBuilder.BuildUserPayload(userId, operation, snapshot);
            return ProcessPayloadAsync(payload, cancellationToken);
        }

        public Task TrackUserChangeAsync(int userId, string operation, UserModel snapshot, CancellationToken cancellationToken = default)
        {
            return TrackUserChangeAsync(userId, ParseOperation(operation), snapshot, cancellationToken);
        }

        public void TrackUserChange(int userId, SyncOperationType operation, UserModel snapshot)
        {
            _ = TrackUserChangeAsync(userId, operation, snapshot);
        }

        public void TrackUserChange(int userId, string operation, UserModel snapshot)
        {
            _ = TrackUserChangeAsync(userId, operation, snapshot);
        }

        #endregion

        private async Task ProcessPayloadAsync(SyncChangePayload payload, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var sent = await _syncService.TrySyncImmediatelyAsync(payload, cancellationToken).ConfigureAwait(false);
                if (!sent)
                {
                    await _repository.EnqueueChangeAsync(payload, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"ცვლილებების Track ვერ მოხერხდა ({payload.TableName}/{payload.RecordKey}). SyncOutbox-ში ინახება.", ex);
                await _repository.EnqueueChangeAsync(payload, cancellationToken).ConfigureAwait(false);
            }
        }

        private static SyncOperationType ParseOperation(string operation)
        {
            if (string.IsNullOrWhiteSpace(operation))
            {
                return SyncOperationType.Update;
            }

            switch (operation.Trim().ToUpperInvariant())
            {
                case "INSERT":
                    return SyncOperationType.Insert;
                case "DELETE":
                    return SyncOperationType.Delete;
                case "UPDATE":
                default:
                    return SyncOperationType.Update;
            }
        }
    }
}



