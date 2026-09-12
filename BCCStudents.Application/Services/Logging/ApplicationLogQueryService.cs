using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Logging
{
    public class ApplicationLogQueryService : IApplicationLogQueryService
    {
        private readonly IApplicationLogRepository _repository;
        private readonly IUserContext _userContext;

        public ApplicationLogQueryService(
            IApplicationLogRepository repository,
            IUserContext userContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public Task<IReadOnlyList<ApplicationLogEntry>> GetLogsForCurrentUserAsync(
            ApplicationLogFilter filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new ApplicationLogFilter();

            var allowedScopes = ApplicationLogPermissionMapper.GetAllowedPermissionScopes(_userContext.GetAllPermissions());
            var allowedCategories = ApplicationLogPermissionMapper.GetAllowedCategories(_userContext.HasPermission);
            var canViewSystem = _userContext.HasPermission(Permission.CanViewSystemLogs);

            return _repository.GetFilteredAsync(
                filter.From,
                filter.To,
                filter.SourceType,
                filter.Category,
                filter.Level,
                filter.Username,
                filter.Operation,
                filter.SearchText,
                _userContext.IsAdmin,
                canViewSystem,
                _userContext.UserId,
                allowedScopes,
                allowedCategories,
                filter.Limit,
                cancellationToken);
        }
    }
}
