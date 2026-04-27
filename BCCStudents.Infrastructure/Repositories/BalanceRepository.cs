using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class BalanceRepository : IBalanceRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public BalanceRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public decimal GetBalance(int studentId)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "SELECT Balance FROM Students WHERE StudentID = @StudentID";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    object result = cmd.ExecuteScalar(); // Executes the query and returns the first column of the first row.
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public void UpdateStudentBalance(int studentId, decimal amount)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                string query = "UPDATE Students SET Balance = Balance - @Amount WHERE StudentID = @StudentID";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    cmd.ExecuteNonQuery(); // Executes the update
                }
            }
        }

        public bool TransferBalance(int fromStudentId, int toStudentId, decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        decimal currentBalance;
                        using (var balanceCmd = new MySqlCommand("SELECT Balance FROM Students WHERE Id = @StudentId FOR UPDATE", conn, transaction))
                        {
                            balanceCmd.Parameters.AddWithValue("@StudentId", fromStudentId);
                            var result = balanceCmd.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                transaction.Rollback();
                                return false;
                            }
                            currentBalance = Convert.ToDecimal(result);
                        }

                        if (currentBalance < amount)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        using (var cmd = new MySqlCommand("UPDATE Students SET Balance = Balance - @Amount, UpdatedAt = @UpdatedAt WHERE Id = @StudentId", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@StudentId", fromStudentId);
                            if (cmd.ExecuteNonQuery() == 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        using (var cmd = new MySqlCommand("UPDATE Students SET Balance = Balance + @Amount, UpdatedAt = @UpdatedAt WHERE Id = @StudentId", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@StudentId", toStudentId);
                            if (cmd.ExecuteNonQuery() == 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

