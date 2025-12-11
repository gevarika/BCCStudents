namespace BCCStudents.Domain.Interfaces
{
    public interface IBalanceRepository
    {
        decimal GetBalance(int studentId);
        void UpdateStudentBalance(int studentId, decimal newBalance);
    }
}


