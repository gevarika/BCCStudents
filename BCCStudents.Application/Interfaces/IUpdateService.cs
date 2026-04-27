using BCCStudents.Application.Services.Update;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// განახლების სერვისის ინტერფეისი
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// აბრუნებს მიმდინარე ვერსიას
        /// </summary>
        Version GetCurrentVersion();

        /// <summary>
        /// იღებს განახლების მანიფესტს
        /// </summary>
        Task<UpdateManifest> GetManifestAsync(CancellationToken ct);

        /// <summary>
        /// ამოწმებს არის თუ არა ახალი ვერსია უფრო ახალი
        /// </summary>
        bool IsNewer(Version latest, Version current);

        /// <summary>
        /// ამოწმებს არის თუ არა განახლება აუცილებელი
        /// </summary>
        bool IsUpdateRequired(Version latest, Version current);

        /// <summary>
        /// ჩამოტვირთავს განახლებას
        /// </summary>
        Task<string> DownloadAsync(UpdateManifest manifest, IProgress<(long current, long total)> progress, CancellationToken ct);

        /// <summary>
        /// აგეგმავს განახლების გამოყენებას და აპლიკაციის გადატვირთვას
        /// </summary>
        Task ScheduleApplyAndRestartAsync(string zipPath, Form owner);
    }
}

