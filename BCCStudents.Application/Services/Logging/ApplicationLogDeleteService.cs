using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Logging
{
    public class ApplicationLogDeleteService : IApplicationLogDeleteService
    {
        private readonly IApplicationLogRepository _repository;
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private readonly IUserContext _userContext;

        public ApplicationLogDeleteService(
            IApplicationLogRepository repository,
            IDatabaseConnectionChecker connectionChecker,
            IUserContext userContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public async Task<int> DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAdmin();
            EnsureServerConnection();
            return await _repository.DeleteAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> DeleteByIdsAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default)
        {
            EnsureAdmin();
            if (ids == null || ids.Count == 0)
                return 0;

            EnsureServerConnection();
            return await _repository.DeleteByIdsAsync(ids, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> DeleteFilteredAsync(ApplicationLogFilter filter, CancellationToken cancellationToken = default)
        {
            EnsureAdmin();
            filter ??= new ApplicationLogFilter();
            EnsureServerConnection();

            return await _repository.DeleteFilteredAsync(
                filter.From,
                filter.To,
                filter.SourceType,
                filter.Category,
                filter.Level,
                filter.Username,
                filter.Operation,
                filter.SearchText,
                cancellationToken).ConfigureAwait(false);
        }

        private void EnsureAdmin()
        {
            if (!_userContext.IsAdmin)
                throw new UnauthorizedAccessException("ლოგების წაშლა მხოლოდ ადმინისტრატორისთვისაა ხელმისაწვდომი.");
        }

        private void EnsureServerConnection()
        {
            if (!_connectionChecker.CanConnectToServer())
                throw new InvalidOperationException("სერვერთან კავშირი არ არის. წაშლა გაუქმებულია.");
        }
    }
}
