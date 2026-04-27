namespace BCCStudents.Presentation
{
    /// <summary>
    /// გლობალური აპლიკაციის მდგომარეობა (მაგ. Offline რეჟიმი)
    /// </summary>
    public static class AppState
    {
        /// <summary>
        /// მიუთითებს, მუშაობს თუ არა აპლიკაცია შეზღუდულ/offline რეჟიმში
        /// </summary>
        public static bool IsOfflineMode { get; set; }
    }
}


