using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    }
}

