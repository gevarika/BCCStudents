namespace BCCStudents.Domain.Entities
{
    public class SmsSendResult
    {
        public bool Success { get; set; }
        public string Status { get; set; }         // eg: accepted
        public string Message { get; set; }        // eg: accepted
        public int ErrorCode { get; set; }         // eg: 0
        public string RawResponse { get; set; }    // full JSON
    }
}

