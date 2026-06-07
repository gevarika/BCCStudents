using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    public class ApplicationLogFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string SourceType { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Username { get; set; }
        public string Operation { get; set; }
        public string SearchText { get; set; }
        public int Limit { get; set; } = 2000;
    }

    public interface IApplicationLogQueryService
    {
        Task<IReadOnlyList<ApplicationLogEntry>> GetLogsForCurrentUserAsync(ApplicationLogFilter filter, CancellationToken cancellationToken = default);
    }
}
