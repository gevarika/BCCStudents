namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// გლობალური აპლიკაციის სტატუსი (DB/ავტორიზაცია)
    /// </summary>
    public interface IApplicationStatus
    {
        /// <summary>
        /// მიუთითებს, არის თუ არა მონაცემთა ბაზასთან კავშირი ხელმისაწვდომი.
        /// </summary>
        bool IsDatabaseOnline { get; set; }

        /// <summary>
        /// მიუთითებს, არის თუ არა მომხმარებელი ავტორიზებული (წარმატებული Login-ის შემდეგ).
        /// </summary>
        bool IsAuthenticated { get; set; }
    }
}


