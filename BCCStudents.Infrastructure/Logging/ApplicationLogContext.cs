namespace BCCStudents.Infrastructure.Logging
{
    /// <summary>
    /// Thread-local user context for Serilog enrichment (set after login).
    /// </summary>
    public static class ApplicationLogContext
    {
        private static readonly AsyncLocal<int?> UserIdHolder = new();
        private static readonly AsyncLocal<string> UsernameHolder = new();

        public static int? UserId
        {
            get => UserIdHolder.Value;
            set => UserIdHolder.Value = value;
        }

        public static string Username
        {
            get => UsernameHolder.Value;
            set => UsernameHolder.Value = value;
        }

        public static void Set(int? userId, string username)
        {
            UserId = userId;
            Username = username;
        }

        public static void Clear()
        {
            UserId = null;
            Username = null;
        }
    }
}
