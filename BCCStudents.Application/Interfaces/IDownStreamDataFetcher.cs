using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// DownStream მონაცემების მიმღების ინტერფეისი
    /// </summary>
    public interface IDownStreamDataFetcher
    {
        /// <summary>
        /// იღებს სტუდენტებს სერვერიდან
        /// </summary>
        Task<List<Student>> FetchStudentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს ჯგუფებს სერვერიდან
        /// </summary>
        Task<List<Group>> FetchGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს ქვეჯგუფებს სერვერიდან
        /// </summary>
        Task<List<SubGroup>> FetchSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს სტუდენტ-ჯგუფ კავშირებს სერვერიდან
        /// </summary>
        Task<List<StudentGroups>> FetchStudentGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს სტუდენტ-ქვეჯგუფ კავშირებს სერვერიდან
        /// </summary>
        Task<List<StudentSubGroups>> FetchStudentSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს გადახდებს სერვერიდან
        /// </summary>
        Task<List<Payment>> FetchPaymentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს წარუმატებელ გადახდებს სერვერიდან
        /// </summary>
        Task<List<FailedPayment>> FetchFailedPaymentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს იმპორტირებული გადახდების ლოგებს სერვერიდან
        /// </summary>
        Task<List<ImportedPaymentLog>> FetchImportedPaymentLogsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს მომხმარებლებს სერვერიდან
        /// </summary>
        Task<List<UserModel>> FetchUsersAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს სისტემურ კონფიგურაციებს სერვერიდან
        /// </summary>
        Task<List<SystemConfiguration>> FetchSystemConfigAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს PendingStudents სერვერიდან
        /// </summary>
        Task<List<PendingStudent>> FetchPendingStudentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს PendingStudentGroups სერვერიდან
        /// </summary>
        Task<List<PendingStudentGroup>> FetchPendingStudentGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// იღებს PendingStudentSubGroups სერვერიდან
        /// </summary>
        Task<List<PendingStudentSubGroup>> FetchPendingStudentSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);
    }
}

