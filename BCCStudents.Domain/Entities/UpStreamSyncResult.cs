namespace BCCStudents.Domain.Entities
{
    public sealed class UpStreamSyncResult
    {
        public bool Success { get; init; }
        public string ErrorMessage { get; init; }

        public static UpStreamSyncResult Ok()
        {
            return new UpStreamSyncResult { Success = true };
        }

        public static UpStreamSyncResult Fail(string errorMessage)
        {
            return new UpStreamSyncResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
