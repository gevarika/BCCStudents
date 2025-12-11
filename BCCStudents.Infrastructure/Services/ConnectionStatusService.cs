using BCCStudents.Application.Interfaces;
using BCCStudents.Infrastructure.Data;
using System;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// კავშირის სტატუსის სერვისი - იმპლემენტირებს IConnectionStatusService ინტერფეისს.
    /// იყენებს DatabaseHelper-ს კავშირის შესამოწმებლად Clean Architecture-ის დაცვით.
    /// </summary>
    public class ConnectionStatusService : IConnectionStatusService
    {
        private readonly DatabaseHelper _databaseHelper;
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
        /// კონსტრუქტორი - იღებს DatabaseHelper-ს Dependency Injection-ით.
        /// ახორციელებს საწყის კავშირის შემოწმებას.
        /// </summary>
        /// <param name="databaseHelper">DatabaseHelper ინსტანსი კავშირის შესამოწმებლად</param>
        public ConnectionStatusService(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
            // კავშირის სტატუსის საწყისი შემოწმება
            CheckConnection();
        }

        /// <summary>
        /// ამოწმებს კავშირს მონაცემთა ბაზასთან.
        /// იყენებს DatabaseHelper.CanConnectToMySQL() მეთოდს.
        /// ანახლებს IsConnected თვისებას და იძახებს ConnectionStatusChanged ივენთს მნიშვნელობის ცვლილებისას.
        /// </summary>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        public bool CheckConnection()
        {
            try
            {
                // იყენებს DatabaseHelper-ს კავშირის შესამოწმებლად
                // IsConnected-ის setter ავტომატურად იძახებს ConnectionStatusChanged ივენთს ცვლილებისას
                IsConnected = _databaseHelper.CanConnectToMySQL();
            }
            catch
            {
                IsConnected = false;
            }
            return IsConnected;
        }
    }
}
