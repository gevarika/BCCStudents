using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// DownStream სინქრონიზაციის რეპოზიტორის ინტერფეისი
    /// </summary>
    public interface IDownStreamSyncRepository
    {
        /// <summary>
        /// იღებს სინქრონიზაციის სტატუსს ცხრილისთვის
        /// </summary>
        Task<SyncStateRecord> GetSyncStateAsync(string tableName, CancellationToken cancellationToken = default);

        /// <summary>
        /// განაახლებს სინქრონიზაციის სტატუსს
        /// </summary>
        Task UpdateSyncStateAsync(string tableName, DateTime lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს სტუდენტებს
        /// </summary>
        Task UpsertStudentsAsync(IReadOnlyList<Student> students, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს ჯგუფებს
        /// </summary>
        Task UpsertGroupsAsync(IReadOnlyList<Group> groups, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს ქვეჯგუფებს
        /// </summary>
        Task UpsertSubGroupsAsync(IReadOnlyList<SubGroup> subGroups, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს სტუდენტ-ჯგუფ კავშირებს
        /// </summary>
        Task UpsertStudentGroupsAsync(IReadOnlyList<StudentGroups> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს სტუდენტ-ქვეჯგუფ კავშირებს
        /// </summary>
        Task UpsertStudentSubGroupsAsync(IReadOnlyList<StudentSubGroups> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს გადახდებს
        /// </summary>
        Task UpsertPaymentsAsync(IReadOnlyList<Payment> payments, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს წარუმატებელ გადახდებს
        /// </summary>
        Task UpsertFailedPaymentsAsync(IReadOnlyList<FailedPayment> payments, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს იმპორტირებული გადახდების ლოგებს
        /// </summary>
        Task UpsertImportedPaymentLogsAsync(IReadOnlyList<ImportedPaymentLog> logs, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს მომხმარებლებს
        /// </summary>
        Task UpsertUsersAsync(IReadOnlyList<UserModel> users, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს სისტემურ კონფიგურაციებს
        /// </summary>
        Task UpsertSystemConfigAsync(IReadOnlyList<SystemConfiguration> configs, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს PendingStudents
        /// </summary>
        Task UpsertPendingStudentsAsync(IReadOnlyList<PendingStudent> students, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს PendingStudentGroups
        /// </summary>
        Task UpsertPendingStudentGroupsAsync(IReadOnlyList<PendingStudentGroup> groups, CancellationToken cancellationToken = default);

        /// <summary>
        /// ამატებს ან განაახლებს PendingStudentSubGroups
        /// </summary>
        Task UpsertPendingStudentSubGroupsAsync(IReadOnlyList<PendingStudentSubGroup> subGroups, CancellationToken cancellationToken = default);

        Task<int> UpsertApplicationLogsAsync(IReadOnlyList<ApplicationLogEntry> logs, CancellationToken cancellationToken = default);
    }
}

