using BCCStudents.Domain.Entities;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace BCCStudents.Infrastructure.Services
{
    internal static class ConnectionFailureParser
    {
        public static ConnectionFailureInfo Parse(Exception exception, string target = "server")
        {
            var root = GetRootException(exception);
            var message = Sanitize(root.Message);
            var category = Classify(root, message);
            var mySqlNumber = (root as MySqlException)?.Number;

            return new ConnectionFailureInfo
            {
                Category = category,
                UserMessage = BuildUserMessage(category, mySqlNumber),
                LogDetail = BuildLogDetail(target, category, root, message, mySqlNumber),
                MySqlErrorNumber = mySqlNumber,
                ExceptionType = root.GetType().Name
            };
        }

        private static ConnectionFailureCategory Classify(Exception ex, string message)
        {
            if (ex is MySqlException mysql)
            {
                return mysql.Number switch
                {
                    1045 or 1044 => ConnectionFailureCategory.Authentication,
                    1049 => ConnectionFailureCategory.Configuration,
                    2003 or 1042 or 0 => ClassifyByMessage(message),
                    _ => ClassifyByMessage(message)
                };
            }

            if (ex is System.Net.Sockets.SocketException)
                return ClassifyByMessage(message);

            if (ex is TimeoutException)
                return ConnectionFailureCategory.Timeout;

            if (message.Contains("SSL", StringComparison.OrdinalIgnoreCase)
                || message.Contains("TLS", StringComparison.OrdinalIgnoreCase))
                return ConnectionFailureCategory.Ssl;

            if (message.Contains("connection string", StringComparison.OrdinalIgnoreCase)
                || message.Contains("not configured", StringComparison.OrdinalIgnoreCase))
                return ConnectionFailureCategory.Configuration;

            return ClassifyByMessage(message);
        }

        private static ConnectionFailureCategory ClassifyByMessage(string message)
        {
            if (message.Contains("No such host", StringComparison.OrdinalIgnoreCase)
                || message.Contains("getaddrinfo", StringComparison.OrdinalIgnoreCase))
                return ConnectionFailureCategory.Dns;

            if (message.Contains("actively refused", StringComparison.OrdinalIgnoreCase)
                || message.Contains("connection refused", StringComparison.OrdinalIgnoreCase))
                return ConnectionFailureCategory.Refused;

            if (message.Contains("timed out", StringComparison.OrdinalIgnoreCase)
                || message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
                return ConnectionFailureCategory.Timeout;

            if (message.Contains("Unable to connect", StringComparison.OrdinalIgnoreCase)
                || message.Contains("connect to any of the specified", StringComparison.OrdinalIgnoreCase))
                return ConnectionFailureCategory.Network;

            return ConnectionFailureCategory.Unknown;
        }

        private static string BuildUserMessage(ConnectionFailureCategory category, int? mySqlNumber)
        {
            return category switch
            {
                ConnectionFailureCategory.Dns => "სერვერი ვერ მოიძებნა (DNS)",
                ConnectionFailureCategory.Refused => "სერვერი არ პასუხობს",
                ConnectionFailureCategory.Timeout => "კავშირის ვადა ამოიწურა",
                ConnectionFailureCategory.Ssl => "უსაფრთხო კავშირის შეცდომა",
                ConnectionFailureCategory.Authentication => "ავტორიზაცია ვერ მოხერხდა",
                ConnectionFailureCategory.Configuration => "სერვერის პარამეტრები არ არის",
                ConnectionFailureCategory.Network => "ქსელის შეცდომა",
                _ => mySqlNumber is > 0
                    ? $"სერვერთან კავშირი ვერ დამყარდა ({mySqlNumber})"
                    : "სერვერთან კავშირი ვერ დამყარდა"
            };
        }

        private static string BuildLogDetail(
            string target,
            ConnectionFailureCategory category,
            Exception root,
            string message,
            int? mySqlNumber)
        {
            var parts = new List<string>
            {
                $"Target={target}",
                $"Category={category}",
                $"Type={root.GetType().Name}"
            };

            if (mySqlNumber is > 0)
                parts.Add($"MySqlError={mySqlNumber}");

            if (!string.IsNullOrWhiteSpace(message))
                parts.Add($"Message={message}");

            return string.Join("; ", parts);
        }

        private static Exception GetRootException(Exception exception)
        {
            var current = exception;
            while (current.InnerException != null)
                current = current.InnerException;
            return current;
        }

        private static string Sanitize(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var sanitized = Regex.Replace(text, @"(pwd|password)\s*=\s*[^;'\s]+", "$1=***", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, @"User ID\s*=\s*[^;]+", "User ID=***", RegexOptions.IgnoreCase);
            return sanitized.Trim();
        }
    }
}
