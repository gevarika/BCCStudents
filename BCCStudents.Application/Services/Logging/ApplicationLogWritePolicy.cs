using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Logging
{
    public sealed class ApplicationLogWritePolicy
    {
        private readonly ILogStorageSettings _settings;

        public ApplicationLogWritePolicy(ILogStorageSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public bool ShouldWriteLocalFiles => _settings.IsLocal;
        public bool ShouldWriteLocalDatabase => _settings.IsLocal;
        public bool ShouldWriteServer => _settings.IsServer;
    }
}
