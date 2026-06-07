using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    public interface IApplicationLogDeleteService
    {
        Task<int> DeleteAllAsync(CancellationToken cancellationToken = default);
        Task<int> DeleteByIdsAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default);
        Task<int> DeleteFilteredAsync(ApplicationLogFilter filter, CancellationToken cancellationToken = default);
    }
}
