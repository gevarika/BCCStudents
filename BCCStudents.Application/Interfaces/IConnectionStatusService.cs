namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// კავშირის სტატუსის სერვისის ინტერფეისი
    /// წარმოადგენს ბიზნესის კონტრაქტს კავშირის სტატუსის მართვისთვის
    /// </summary>
    public interface IConnectionStatusService
    {
        /// <summary>
        /// აბრუნებს კავშირის მიმდინარე სტატუსს
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// მეთოდი, რომელიც ამოწმებს კავშირს
        /// </summary>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        bool CheckConnection();

        /// <summary>
        /// ივენთი, რომელიც იძახება კავშირის სტატუსის ცვლილებისას
        /// </summary>
        event EventHandler ConnectionStatusChanged;
    }
}

