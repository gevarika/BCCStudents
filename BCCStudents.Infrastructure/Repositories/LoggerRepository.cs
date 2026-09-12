using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Logging;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Logging;
using Serilog;

namespace BCCStudents.Infrastructure.Repositories
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly IApplicationLogRepository _applicationLogRepository;
        private readonly ApplicationLogWritePolicy _writePolicy;

        public LoggerRepository(
            IApplicationLogRepository applicationLogRepository,
            ApplicationLogWritePolicy writePolicy)
        {
            _applicationLogRepository = applicationLogRepository ?? throw new ArgumentNullException(nameof(applicationLogRepository));
            _writePolicy = writePolicy ?? throw new ArgumentNullException(nameof(writePolicy));
        }

        public void WriteLog(string operationType, string status, string details, string user = "System")
        {
            WriteAudit("operation_log.txt", operationType, status, details, user);
        }

        public void LogStudentAction(string action, string status, string details, string user = "System")
        {
            WriteAudit("students_log.txt", action, status, details, user);
        }

        public void LogGroupAction(string action, string status, string details, string user = "System")
        {
            WriteAudit("groups_log.txt", action, status, details, user);
        }

        public void LogPaymentAction(string action, string status, string details, string user = "System")
        {
            WriteAudit("payments_log.txt", action, status, details, user);
        }

        public void LogSMSAction(string action, string status, string details, string user = "System")
        {
            WriteAudit("sms_log.txt", action, status, details, user);
        }

        public void LogImportAction(string action, string status, string details, string user = "System")
        {
            WriteAudit("Import_log.txt", action, status, details, user);
        }

        private void WriteAudit(string fileName, string operationType, string status, string details, string user)
        {
            if (_writePolicy.ShouldWriteLocalFiles)
            {
                try
                {
                    AuditLogFactory.GetLogger(fileName)
                        .ForContext("Operation", operationType ?? string.Empty)
                        .ForContext("Status", status ?? string.Empty)
                        .ForContext("User", string.IsNullOrWhiteSpace(user) ? "System" : user)
                        .ForContext("Details", details ?? string.Empty)
                        .Information("Audit");
                }
                catch
                {
                    // არ უნდა დაბლოკოს აპლიკაცია.
                }
            }

            try
            {
                var entry = CreateEntry(fileName, operationType, status, details, user);

                if (_writePolicy.ShouldWriteLocalDatabase)
                {
                    _applicationLogRepository.InsertAsync(entry).GetAwaiter().GetResult();
                }
            }
            catch
            {
                // DB write failure must not block business operations.
            }
        }

        private static ApplicationLogEntry CreateEntry(string fileName, string operationType, string status, string details, string user)
        {
            var category = ApplicationLogPermissionMapper.GetCategoryForAuditFile(fileName);
            var level = MapAuditLevel(status);
            var userId = ApplicationLogContext.UserId ?? (UserSession.Id > 0 ? UserSession.Id : (int?)null);
            var username = string.IsNullOrWhiteSpace(user) ? ApplicationLogContext.Username ?? "System" : user;

            return new ApplicationLogEntry
            {
                LogGuid = Guid.NewGuid().ToString(),
                SourceType = LogSourceType.Audit,
                Category = category,
                Level = level,
                Operation = operationType,
                Status = status,
                UserId = userId,
                Username = username,
                MachineName = Environment.MachineName,
                PermissionScope = ApplicationLogPermissionMapper.GetPermissionScopeForCategory(category),
                Message = operationType,
                Details = details,
                CreatedAt = DateTime.UtcNow,
                Origin = "Local"
            };
        }

        private static string MapAuditLevel(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Information";

            var normalized = status.Trim().ToLowerInvariant();
            if (normalized.Contains("error") || normalized.Contains("fail"))
                return "Error";
            if (normalized.Contains("warn"))
                return "Warning";

            return "Information";
        }
    }
}
