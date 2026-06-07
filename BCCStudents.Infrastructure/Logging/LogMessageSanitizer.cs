using System.Text.RegularExpressions;

namespace BCCStudents.Infrastructure.Logging
{
    internal static class LogMessageSanitizer
    {
        private static readonly Regex PasswordRegex = new(
            @"(password\s*=\s*)([^;'""\s]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Sanitize(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            return PasswordRegex.Replace(message, "$1***");
        }
    }
}
