using BCCStudents.Application.Interfaces;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// კავშირის სტატუსის სერვისი - იმპლემენტირებს IConnectionStatusService ინტერფეისს.
    /// იყენებს IDatabaseConnectionChecker-ს კავშირის შესამოწმებლად Clean Architecture-ის დაცვით.
    /// </summary>
    public class ConnectionStatusService : IConnectionStatusService
    {
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private bool _isConnected;

        /// <summary>
        /// კავშირის მიმდინარე სტატუსი.
        /// private set - მხოლოდ ამ კლასმა შეძლოს მისი განახლება.
        /// მნიშვნელობის ცვლილებისას ავტომატურად იძახებს ConnectionStatusChanged ივენთს.
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ივენთი, რომელიც იძახება კავშირის სტატუსის ცვლილებისას.
        /// </summary>
        public event EventHandler ConnectionStatusChanged;

        /// <summary>
        /// კონსტრუქტორი - იღებს IDatabaseConnectionChecker-ს Dependency Injection-ით.
        /// ახორციელებს საწყის კავშირის შემოწმებას.
        /// </summary>
        /// <param name="connectionChecker">IDatabaseConnectionChecker ინსტანსი კავშირის შესამოწმებლად</param>
        public ConnectionStatusService(IDatabaseConnectionChecker connectionChecker)
        {
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            // კავშირის სტატუსის საწყისი შემოწმება
            CheckConnection();
        }

        /// <summary>
        /// ამოწმებს კავშირს მონაცემთა ბაზასთან.
        /// იყენებს IDatabaseConnectionChecker.CanConnectToMySQL() მეთოდს.
        /// ანახლებს IsConnected თვისებას და იძახებს ConnectionStatusChanged ივენთს მნიშვნელობის ცვლილებისას.
        /// </summary>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        public bool CheckConnection()
        {
            try
            {
                // იყენებს IDatabaseConnectionChecker-ს კავშირის შესამოწმებლად
                // IsConnected-ის setter ავტომატურად იძახებს ConnectionStatusChanged ივენთს ცვლილებისას
                IsConnected = _connectionChecker.CanConnectToMySQL();
            }
            catch
            {
                IsConnected = false;
            }
            return IsConnected;
        }
    }
}

