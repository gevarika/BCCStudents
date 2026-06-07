using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Sync
{
    /// <summary>
    /// პერიოდული სინქის <see cref="System.Threading.Timer"/> callback-ების უსაფრთხო გაშვება.
    /// </summary>
    /// <remarks>
    /// რატომ არ ვიყენებთ პირდაპირ: <c>new Timer(async _ =&gt; await Work())</c>
    /// <list type="bullet">
    /// <item><description>Timer არ ელოდება Task-ის დასრულებას — გამოტოვებული exception ხდება UnobservedTask / AppDomain Unhandled.</description></item>
    /// <item><description>MySql.Data SSL-ისას შიდა timeout-ის CancellationToken შეიძლება ისროლოს AggregateException ცალკე thread pool timer-ზე — ეს wrapper ამასაც ლოგში იჭერს.</description></item>
    /// </list>
    /// ConnectionMonitorService იყენებს WinForms Timer-ს (სხვა მექანიზმი).
    /// </remarks>
    internal static class SyncPeriodicTimerRunner
    {
        /// <summary>
        /// ქმნის Timer callback-ს, რომელიც ასინქრონულ სამუშაოს უსაფრთხოდ გაუშვებს.
        /// </summary>
        /// <param name="work">ერთი სინქრონიზაციის ციკლი (მაგ. ProcessPendingAsync).</param>
        /// <param name="logger">სინქის ლოგი.</param>
        /// <param name="operationName">ლოგისთვის (UpStream / DownStream).</param>
        public static TimerCallback CreateCallback(Func<Task> work, ISyncLogger logger, string operationName)
        {
            ArgumentNullException.ThrowIfNull(work);
            ArgumentNullException.ThrowIfNull(logger);
            if (string.IsNullOrWhiteSpace(operationName))
                throw new ArgumentException("Operation name is required.", nameof(operationName));

            return _ =>
            {
                // განზრახ fire-and-forget: Timer-ის ხაზზე await არ ვაკეთებთ, მაგრამ შეცდომას ვიჭერთ ქვედა Task-ში.
                _ = RunSafeAsync(work, logger, operationName);
            };
        }

        private static async Task RunSafeAsync(Func<Task> work, ISyncLogger logger, string operationName)
        {
            try
            {
                await work().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogTimerFailure(logger, operationName, ex);
            }
        }

        private static void LogTimerFailure(ISyncLogger logger, string operationName, Exception ex)
        {
            var throttleKey = $"sync-timer-{operationName.ToLowerInvariant()}";

            if (SyncConnectionHelper.IsLikelyConnectionError(ex))
            {
                SyncLogThrottle.TryError(logger, throttleKey,
                    $"{operationName} periodic timer: connection/SSL error (caught at timer boundary).",
                    ex,
                    SyncLogThrottle.DefaultInterval);
                return;
            }

            logger.Error($"{operationName} periodic timer: unexpected error (caught at timer boundary).", ex);
        }
    }
}
