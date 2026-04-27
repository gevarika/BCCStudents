namespace BCCStudents.Application.Services
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string Status { get; set; }
        public List<string> Logs { get; set; }

        public PaymentResult(bool success, string message, List<string> logs)
        {
            IsSuccess = success;
            Status = message;
            Logs = logs;
        }
    }
}

