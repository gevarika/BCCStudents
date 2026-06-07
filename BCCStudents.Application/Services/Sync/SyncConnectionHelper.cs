using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services.Sync
{
    public static class SyncConnectionHelper
    {
        /// <summary>
        /// სერვერთან/ქსელთან დაკავშირებული შეცდომაა თუ არა (სინქის ლოგის დედუპლიკაციისთვის).
        /// მათ შორის MySql SSL timeout-ის AggregateException (app-*.log FTL-ის ტიპური შემთხვევა).
        /// </summary>
        public static bool IsLikelyConnectionError(Exception? exception)
        {
            if (exception is AggregateException aggregate)
            {
                if (aggregate.InnerExceptions.Any(IsLikelyConnectionError))
                    return true;

                return IsLikelyConnectionError(aggregate.InnerException);
            }

            for (var ex = exception; ex != null; ex = ex.InnerException)
            {
                if (ex is MySqlException or System.Net.Sockets.SocketException or TimeoutException or IOException)
                    return true;

                if (IsLikelyConnectionErrorMessage(ex.Message))
                    return true;
            }

            return false;
        }

        public static bool IsLikelyConnectionErrorMessage(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            return message.Contains("Unable to connect", StringComparison.OrdinalIgnoreCase)
                || message.Contains("No such host", StringComparison.OrdinalIgnoreCase)
                || message.Contains("actively refused", StringComparison.OrdinalIgnoreCase)
                || message.Contains("Connection timed out", StringComparison.OrdinalIgnoreCase)
                || message.Contains("I/O error occurred", StringComparison.OrdinalIgnoreCase)
                || message.Contains("Authentication to host", StringComparison.OrdinalIgnoreCase)
                || message.Contains("SSL", StringComparison.OrdinalIgnoreCase)
                || message.Contains("კავშირი", StringComparison.OrdinalIgnoreCase)
                || message.Contains("connect to any of the specified", StringComparison.OrdinalIgnoreCase);
        }
    }
}
