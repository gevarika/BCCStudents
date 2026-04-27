using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// IApplicationStatus-ის საბაზო იმპლემენტაცია, რეგისტრირებული Singleton-ად.
    /// </summary>
    public class ApplicationStatus : IApplicationStatus
    {
        public bool IsDatabaseOnline { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}


