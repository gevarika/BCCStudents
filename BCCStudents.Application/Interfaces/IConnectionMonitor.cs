namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// კავშირის მონიტორინგის სერვისის ინტერფეისი
    /// პერიოდულად ამოწმებს კავშირს და აცნობს ცვლილებებს
    /// </summary>
    public interface IConnectionMonitor
    {
        /// <summary>
        /// კავშირის მიმდინარე სტატუსი
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// მონიტორინგის დაწყება
        /// </summary>
        void StartMonitoring();

        /// <summary>
        /// მონიტორინგის გაჩერება
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// ივენთი, რომელიც იძახება კავშირის სტატუსის ცვლილებისას
        /// </summary>
        event EventHandler<bool> ConnectionStatusChanged;
    }
}

