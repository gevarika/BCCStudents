using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services
{
    public class CleanupService : ICleanupService
    {
        private readonly ICleanupRepository _cleanupRepository;
        private readonly ILoggerRepository _logger;
        private readonly IUserRepository _userRepository;

        public CleanupService(ICleanupRepository cleanupRepository, ILoggerRepository logger, IUserRepository userRepository)
        {
            _cleanupRepository = cleanupRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        public void ResetAllData()
        {
            // იდენტიფიცირება და უფლებების შემოწმება
            var user = _userRepository.GetUserById(UserSession.Id);
            if (user == null || user.Role?.ToLower() != "administrator")
                throw new UnauthorizedAccessException("მხოლოდ ადმინისტრატორს აქვს მონაცემების განულების უფლება.");

            _cleanupRepository.ResetAllData();

            _logger.WriteLog("System Cleanup", "Success", $"All data reset by admin: {user.UserName}", user.UserName);
        }
    }

}



