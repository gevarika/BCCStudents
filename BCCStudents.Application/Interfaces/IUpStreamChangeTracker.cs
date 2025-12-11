using System;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// UpStream ცვლილებების თრეკერის ინტერფეისი
    /// </summary>
    public interface IUpStreamChangeTracker
    {
        // Students
        Task TrackStudentChangeAsync(int studentId, SyncOperationType operation, Student snapshot, CancellationToken cancellationToken = default);
        Task TrackStudentChangeAsync(int studentId, string operation, Student snapshot, CancellationToken cancellationToken = default);
        void TrackStudentChange(int studentId, SyncOperationType operation, Student snapshot);
        void TrackStudentChange(int studentId, string operation, Student snapshot);

        // Groups
        Task TrackGroupChangeAsync(int groupId, SyncOperationType operation, Group snapshot, CancellationToken cancellationToken = default);
        Task TrackGroupChangeAsync(int groupId, string operation, Group snapshot, CancellationToken cancellationToken = default);
        void TrackGroupChange(int groupId, SyncOperationType operation, Group snapshot);
        void TrackGroupChange(int groupId, string operation, Group snapshot);

        // SubGroups
        Task TrackSubGroupChangeAsync(int subGroupId, SyncOperationType operation, SubGroup snapshot, CancellationToken cancellationToken = default);
        Task TrackSubGroupChangeAsync(int subGroupId, string operation, SubGroup snapshot, CancellationToken cancellationToken = default);
        void TrackSubGroupChange(int subGroupId, SyncOperationType operation, SubGroup snapshot);
        void TrackSubGroupChange(int subGroupId, string operation, SubGroup snapshot);

        // StudentGroups
        Task TrackStudentGroupChangeAsync(int recordId, SyncOperationType operation, StudentGroups snapshot, CancellationToken cancellationToken = default);
        Task TrackStudentGroupChangeAsync(int recordId, string operation, StudentGroups snapshot, CancellationToken cancellationToken = default);
        void TrackStudentGroupChange(int recordId, SyncOperationType operation, StudentGroups snapshot);
        void TrackStudentGroupChange(int recordId, string operation, StudentGroups snapshot);

        // StudentSubGroups
        Task TrackStudentSubGroupChangeAsync(int recordId, SyncOperationType operation, StudentSubGroups snapshot, CancellationToken cancellationToken = default);
        Task TrackStudentSubGroupChangeAsync(int recordId, string operation, StudentSubGroups snapshot, CancellationToken cancellationToken = default);
        void TrackStudentSubGroupChange(int recordId, SyncOperationType operation, StudentSubGroups snapshot);
        void TrackStudentSubGroupChange(int recordId, string operation, StudentSubGroups snapshot);

        // FailedPayments
        Task TrackFailedPaymentChangeAsync(int failedPaymentId, SyncOperationType operation, FailedPayment snapshot, CancellationToken cancellationToken = default);
        Task TrackFailedPaymentChangeAsync(int failedPaymentId, string operation, FailedPayment snapshot, CancellationToken cancellationToken = default);
        void TrackFailedPaymentChange(int failedPaymentId, SyncOperationType operation, FailedPayment snapshot);
        void TrackFailedPaymentChange(int failedPaymentId, string operation, FailedPayment snapshot);

        // ImportedPaymentsLog
        Task TrackImportedPaymentLogChangeAsync(int importedPaymentLogId, SyncOperationType operation, ImportedPaymentLog snapshot, CancellationToken cancellationToken = default);
        Task TrackImportedPaymentLogChangeAsync(int importedPaymentLogId, string operation, ImportedPaymentLog snapshot, CancellationToken cancellationToken = default);
        void TrackImportedPaymentLogChange(int importedPaymentLogId, SyncOperationType operation, ImportedPaymentLog snapshot);
        void TrackImportedPaymentLogChange(int importedPaymentLogId, string operation, ImportedPaymentLog snapshot);
    }
}

