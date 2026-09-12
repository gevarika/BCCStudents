using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services
{
    /// <summary>
    /// ქსელური/MySQL კავშირის შეცდომების ამოცნობა (გლობალური exception handlers).
    /// </summary>
    public static class ConnectionErrorHelper
    {
        public static bool IsLikelyConnectionError(Exception? ex)
        {
            while (ex != null)
            {
                if (IsLikelyConnectionErrorMessage(ex.Message) || ex is MySqlException || ex is TimeoutException || ex is IOException)
                    return true;
                ex = ex.InnerException;
            }
            return false;
        }

        public static bool IsLikelyConnectionErrorMessage(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            var m = message.ToLowerInvariant();
            return m.Contains("unable to connect")
                   || m.Contains("timeout")
                   || m.Contains("ssl")
                   || m.Contains("connection")
                   || m.Contains("network")
                   || m.Contains("host")
                   || m.Contains("refused")
                   || m.Contains("unreachable");
        }
    }
}
