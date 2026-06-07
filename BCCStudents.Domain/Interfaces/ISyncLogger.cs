namespace BCCStudents.Domain.Interfaces
{
    public interface ISyncLogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception? exception = null);
    }
}
