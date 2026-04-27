using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// ადმინისტრატორის კოდის მენეჯერი
    /// </summary>
    public class AdminCodeManager
    {
        private readonly IConfigurationService _config;

        public AdminCodeManager(IConfigurationService config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Admin code რომელიც Properties.Settings-იდან update-დება და ინახება
        /// </summary>
        public string AdminCode
        {
            get
            {
                var code = _config.AdminCode;
                return string.IsNullOrWhiteSpace(code) ? "admin123" : code;
            }
        }
        /// <summary>
        /// ადმინისტრატორის კოდის შეცვლა
        /// </summary>
        /// <param name="newCode"></param>
        public void SetAdminCode(string newCode)
        {
            if (!string.IsNullOrWhiteSpace(newCode))
            {
                _config.AdminCode = newCode;
                _config.Save();
            }
        }
    }
}
