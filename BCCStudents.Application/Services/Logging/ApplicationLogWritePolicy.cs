using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Logging
{
    /// <summary>
    /// Server-only: ლოგები იწერება სერვერის ApplicationLogs ცხრილში (რეპოზიტორიით).
    /// </summary>
    public sealed class ApplicationLogWritePolicy
    {
        private readonly ILogStorageSettings _settings;

        public ApplicationLogWritePolicy(ILogStorageSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>ოფციონალური ლოკალური ფაილები მხოლოდ „Local“ რეჟიმის UI ტოგლისას.</summary>
        public bool ShouldWriteLocalFiles => _settings.IsLocal;

        /// <summary>ყოველთვის true — GetLocalConnection = სერვერი.</summary>
        public bool ShouldWriteLocalDatabase => true;

        /// <summary>Deprecated — აღარ გამოიყენება ცალკე upload path.</summary>
        public bool ShouldWriteServer => false;
    }
}
