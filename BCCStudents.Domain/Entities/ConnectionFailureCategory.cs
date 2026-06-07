namespace BCCStudents.Domain.Entities
{
    public enum ConnectionFailureCategory
    {
        Unknown,
        Dns,
        Refused,
        Timeout,
        Ssl,
        Authentication,
        Configuration,
        Network
    }
}
