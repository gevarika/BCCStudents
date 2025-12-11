using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    }
}

