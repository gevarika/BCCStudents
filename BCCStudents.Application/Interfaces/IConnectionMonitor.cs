using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// კავშირის მონიტორინგის სერვისის ინტერფეისი
    /// პერიოდულად ამოწმებს კავშირს და აცნობს ცვლილებებს
    /// </summary>
    public interface IConnectionMonitor
    {
        /// <summary>
        /// ლოკალური ბაზასთან კავშირის მიმდინარე სტატუსი
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// სერვერის ბაზასთან კავშირის მიმდინარე სტატუსი
        /// </summary>
        bool IsServerConnected { get; }

        /// <summary>
        /// ბოლო სერვერის კავშირის შეცდომა (თუ გათიშულია).
        /// </summary>
        ConnectionFailureInfo? LastServerConnectionFailure { get; }

        /// <summary>
        /// მონიტორინგის დაწყება
        /// </summary>
        void StartMonitoring();

        /// <summary>
        /// მონიტორინგის გაჩერება
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// ივენთი, რომელიც იძახება ლოკალური ბაზასთან კავშირის სტატუსის ცვლილებისას
        /// </summary>
        event EventHandler<bool> ConnectionStatusChanged;

        /// <summary>
        /// ივენთი, რომელიც იძახება სერვერის ბაზასთან კავშირის სტატუსის ცვლილებისას
        /// </summary>
        event EventHandler<ServerConnectionChangedEventArgs>? ServerConnectionStatusChanged;
    }
}

